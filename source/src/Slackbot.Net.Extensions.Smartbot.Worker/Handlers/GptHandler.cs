using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.Endpoints.Models.Interactive.MessageActions;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Models.Requests.ChatPostMessage;
using Smartbot.Utilities.Handlers.Sanity;
using JsonSerializer = RestSharp.Serialization.Json.JsonSerializer;
using Message = Slackbot.Net.SlackClients.Http.Models.Responses.ConversationsRepliesResponse.Message;
using User = Slackbot.Net.SlackClients.Http.Models.Responses.UsersList.User;

namespace Smartbot.Utilities.Handlers;

public class GptHandler : IHandleMessageActions, INoOpAppMentions
{
    private readonly OpenAIClient _client;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GptHandler> _logger;
    private readonly ISlackClient _slackClient;
    private User[] _users;
    private readonly SanityHelper _sanity;

    public GptHandler(ISlackClient slackClient, IHttpClientFactory factory, ILogger<GptHandler> logger)
    {
        _slackClient = slackClient;
        _logger = logger;
        var key = Environment.GetEnvironmentVariable("SMARTBOT_OPENAI_KEY");
        _client = new OpenAIClient(key);
        _httpClient = new HttpClient();
        _users = Array.Empty<User>();
        _sanity = new SanityHelper(factory);
    }

    public async Task<EventHandledResponse> Handle(MessageActionInteraction @event)
    {
        _logger.LogInformation(
            $"{@event.Callback_Id} req from {@event.User.Username} on text '{@event.Message.User}:{@event.Message.Text}'");
        Task.Run(async () =>
        {
            try
            {
                await ForwardToGpt(@event);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        });

        return new EventHandledResponse("NON-BG");
    }

    public async Task<EventHandledResponse> Handle(EventMetaData metadata, AppMentionEvent appMention)
    {
        Task.Run(async () =>
        {
            try
            {
                await AnswerDirectly(appMention);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        });

        return new EventHandledResponse("NON-BG");
    }

    private async Task AnswerDirectly(AppMentionEvent appMention)
    {
        var users = await FetchUserFromSlackOrCache();
        var media = (await _sanity.GetMoviesAndSeries()).Take(10);
 
        var userName = users.First(u => u.Id == @appMention.User).Name;

        
        string smdbSetup = "";
        if (media.Any())
        {
            var mediaEntries = string.Join("\n", media.Select(m =>
            {
                return $"| {m.Title} | {m.SanityType} |  {m.Description} | {m.IMDBUrl} | {SmdbUrl(m)} | {m.Year} | {m.StreamingService?.Name} | {m.StreamUrl} | {Quotes(m)} |";

                string SmdbUrl(MovieOrSeries m)
                {
                    return m.SanityType switch
                    {
                        "movie" or "series" => $"https://smdb.app/{m.SanityType}/{m.Slug.Current}",
                        _ => "https://smdb.app"
                    };
                }

                string Quotes(MovieOrSeries m)
                {
                    if (m.Quotes.Any())
                        return string.Join("\n", m.Quotes.Select(c => $" {c.Author.Nickname}: '{c.Text}'"));
                    return "";
                }
            }));
            smdbSetup =
$"""

The users have compiled a list of movies and tv-shows that can be used for movie or series recommendations. 
The name of this list is SMDB, or the 'Smarting Movie Database'. 
User reviews can be either positive, neutral, or negative to the show.
When you recommend any movies or tv series, you only list items from SMDB:

SMDB contents:  

| Title | Series or Movie | Description | IMDBURL | SMDBURL | Year | StreamingService | StreamUrl | User reviews |
| ---   | ---             | ---         | ---     | ---     | ---  | ---              | ---       | ---          |
{mediaEntries}

""";    
        }
        else
        {
            _logger.LogInformation("No media");
        }

        
        var userList = string.Join("\n", users.Select(
            u => $"| <@{u.Id}> | {u.Real_name} | {u.Name} | {(u.Is_Bot ? "bot" : "human")} |"));

        var setup =
$"""
You are a Slackbot named @smartbot in the Slack workspace "Smarteboka. You provide helpful replies, but never questions. Your capabilities are:

- Answer when @smartbot is mentioned in a slack message
- Manually triggered context menu commands named:  "tldr", "orakel" or "kritisk blikk";

The context menu are:
- "tldr": summerizes a slack thread
- "kritisk blikk": provides a snappy reply
- "orakel": recommendations or answers questions

{smdbSetup}


Each user in the Slack workspace is called a 'smarting'. The full list of smartinger are:

| userId | real name | username | bot or human |
| --- | --- | --- | --- |
{userList}

Your replies always answer humans back in norwegian.
Your replies never provide the userId in replies.
If adressing a user, always adress them on format:  @username
Your replies never begin with the text smartbot or oldbot or your own name.

""";

        
        

        Console.WriteLine(setup);



        var prompts = new List<ChatPrompt>();
        prompts.Add(new ChatPrompt("system", setup));
        var replies = Array.Empty<Message>();
        if (appMention.Thread_Ts is { })
        {
            var repliesResponse =  await _slackClient.ConversationsReplies(@appMention.Channel, @appMention.Thread_Ts);
            replies = repliesResponse.Messages;
        }

        Console.WriteLine($"There are {replies.Length} messages in thread");

        foreach (var threadMessage in replies)
        {
            var matchingUser = users.First(u => u.Id == threadMessage.User);
            prompts.Add(new ChatPrompt(matchingUser.Is_Bot ? "assistant": "user", $"{(!matchingUser.Is_Bot ? $"{matchingUser.Name}: " : "")}{threadMessage.Text}"));
        }
        
        prompts.Add(new ChatPrompt("user", $"{userName} : '{appMention.Text}'"));
        Console.WriteLine("***");
        Console.WriteLine(JsonConvert.SerializeObject(prompts.ToArray()[1..]));
        Console.WriteLine("***");
        var ctoken = new CancellationTokenSource(20000);
        try
        {
            var res = await _client.ChatEndpoint.GetCompletionAsync(new ChatRequest(prompts),
                ctoken.Token);

            var completions = res.FirstChoice.Message.Content;

            await _slackClient.ChatPostMessage(new ChatPostMessageRequest
            {
                Channel = appMention.Channel,
                thread_ts = appMention.Ts,
                Text = completions,
                Link_Names = true
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            var txt = e switch
            {
                TaskCanceledException or { InnerException: TaskCanceledException } =>
                    "⏳ Oops, nå brukte jeg litt lang tid her. Prøv igjen litt senere, a.",
                _ => "Der gikk noe i stå-bot. Smartbot må nok inn på sørvis."
            };
            
            await _slackClient.ChatPostMessage(new ChatPostMessageRequest
            {
                Channel = appMention.Channel,
                thread_ts = appMention.Ts,
                Text = "Oops, dette tok for lang tid for OpenAI. Prøv igjen senere",
            });
        }
    }

    private async Task ForwardToGpt(MessageActionInteraction @event)
    {
        var completionPrompt = @event.Callback_Id switch
        {
            "gpt_critico" => await ElCritico(@event.Message.Text, @event.Message.User),
            "gpt_tldr" => await ThreadTLDR(@event.Channel.Id, @event.Message_Ts),
            "gpt_oracle" => await AnswerForwardedQuestion(@event.Message.Text, @event.Message.User,
                @event.User.Username),
            _ => null
        };

        bool? broadcast = @event.Callback_Id switch
        {
            "gpt_tldr" => true,
            _ => null
        };

        if (completionPrompt != null)
        {
            var ctoken = new CancellationTokenSource(20000);
            try
            {
                var res = await _client.ChatEndpoint.GetCompletionAsync(new ChatRequest(completionPrompt),
                    ctoken.Token);
                _logger.LogInformation(res.ProcessingTime.ToPrettyFormat());
                var completions = res.FirstChoice.Message.Content;

                await _slackClient.ChatPostMessage(new ChatPostMessageRequest
                {
                    Channel = @event.Channel.Id,
                    thread_ts = @event.Message_Ts,
                    Text = completions,
                    Link_Names = true,
                    Reply_Broadcast = broadcast
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                var txt = e switch
                {
                    TaskCanceledException or { InnerException: TaskCanceledException } =>
                        "⏳ Oops, nå brukte jeg litt lang tid her. Prøv igjen litt senere, a.",
                    _ => "Der gikk noe i stå-bot. Smartbot må nok inn på sørvis."
                };
                await _httpClient.PostAsJsonAsync(@event.Response_Url, new
                {
                    text = txt
                });
            }
        }
        else
        {
            await _httpClient.PostAsJsonAsync(@event.Response_Url, new
            {
                text = "Denne funksjonen støttes ikke ennå :/"
            });
            _logger.LogInformation($"Not doing anything. Unknown callback_id {@event.Callback_Id}");
        }
    }

    private async Task<IEnumerable<ChatPrompt>> ElCritico(string text, string userId)
    {
        var usersInReplies = await FetchUserFromSlackOrCache();
        ;
        var userForId = usersInReplies.FirstOrDefault(u => u.Id == userId);
        var p1 = new ChatPrompt("system", "Du er en satiriker som liker å bruke ironi og sarkasme i dialog. " +
                                          "Du liker å bruke ungdommelig språk og starter og avslutter alle setninger med emojiis. " +
                                          "Spesielt hodeskalle-emojii. Du bruker maaaaange hodeskalle-emojii " +
                                          "Du bruker max 80 ord i svarene dine. " +
                                          "Du liker å sammenligne ting mot de største hendelsene i verdenshistorien." +
                                          "Du er som regel negativ og et dystert syn på alle tema du får servert. ");
        var p2 = new ChatPrompt("user", $"{userForId} sier:{text}");
        return new[] { p1, p2 };
    }

    private async Task<User[]> FetchUserFromSlackOrCache()
    {
        if (!_users.Any())
        {
            var userRes = await _slackClient.UsersList();
            if (userRes.Ok)
            {
                _users = userRes.Members.Where(u => !u.Deleted && u.Id != "USLACKBOT").ToArray();
            }
            else
            {
                if (!userRes.Ok)
                    throw new Exception("aiaiai, klarte ikke hente ut noe greier fra slack api");
            }
        }

        return _users;
    }

    private async Task<IEnumerable<ChatPrompt>> ThreadTLDR(string channel, string ts)
    {
        _logger.LogInformation("kom hit1 ");
        var replies = await _slackClient.ConversationsReplies(channel, ts);
        var allMembers = await FetchUserFromSlackOrCache();
        _logger.LogInformation("kom hit2");
        var chatDialog = replies.Messages.Select(c =>
            $"{allMembers.FirstOrDefault(a => a.Id == c.User)?.Name ?? "inntrenger"} : '{c.Text}'\n");
        _logger.LogInformation(string.Join("\n", chatDialog));
        _logger.LogInformation("kom hit3");
        var p1 = new ChatPrompt("system",
            "Du er en filmkritiker som elsker å anmelde meldingsutvekslinger mellom venner med satire. " +
            "Du bruker sarkasme, ironi, språklige bilder og metaforer.");
        var p2 = new ChatPrompt("user",
            // "Du er en filmkritiker som elsker å anmelde meldingsutvekslinger mellom venner med satire.\n"+
            "Jeg vil at du skal tolke en dialog mellom venner\n." +
            "Oppsummer samtalen i mellom disse vennene i et referat på maks 80 ord. " +
            "Svar på norsk. " +
            "Ikke bruk ordene \"slack-tråd\", \"slack\", \"manus\", \"film\" eller \"dialog\"" +
            "Oppsummeringen må inneholde følgende:\n" +
            "1) Start med å skryte av den første karakteren som starter dialogen. " +
            "Alltid nevn navnet på den første karakteren. " +
            "Fortell hvor bra første melding er. Dette er alltid en bra påstand eller et godt spørsmål. " +
            "Bruk superlativer når du beskriver den første meldingen eller beskriver den første karakteren. " +
            "Du er glad for at noen har dette som tema.\n" +
            "2) Fortell, bruk sarkasme, ironi og språklige bilder, metaforer for å oppsummere resten av meldingene. " +
            "Oppsummeringen må fortelle litt ekstra om karakterene som deltar." +
            "Gi til slutt et lavt terningkast. Eksempler:" +
            " \"Terningkast 0\".\n" +
            " \"Terningkast 1\".\n" +
            " \"Terningkast 3\".\n" +
            "Dialog:\n\n" +
            $"{string.Join("\n", chatDialog)}");

        return new[] { p1, p2 };
    }

    private async Task<IEnumerable<ChatPrompt>> AnswerForwardedQuestion(string text, string userId, string triggerName)
    {
        var users = await FetchUserFromSlackOrCache();

        var username = users.FirstOrDefault(m => m.Id == userId) != null
            ? users.First(m => m.Id == userId).Name
            : "Ukjent";

        var setup = "You are an helpful assistant. " +
                    $"I will provide you with a dialogue between two friends named, {username} and {triggerName}." +
                    "I want you to reply with some helpful insights" +
                    "and recommeded approaches or products. " +
                    "Reply with recommended products or services. " +
                    "If I ask for product recommendations, provide pros and cons of each product and a reasoning behind " +
                    "the recommendation. " +
                    "Answer in norwegian language. " +
                    "Use maximum 200 words in your replies. " +
                    "Never use wish anyone good luck or use phrases like \"Lykke til\". " +
                    "Never ask follow up questions. " +
                    "If you don't know of any recommendations, just reply with funny excuses about being a robot." +
                    $"Always directly address the first person {triggerName} in the dialogue with a short thanks." +
                    $"Recommendations must be directed to {username}";

        var p1 = new ChatPrompt("system", setup);


        var priming = "Under kommer en dialog mellom to venner som ønsker din mening eller anbefalinger. " +
                      $"Start alltid svaret ditt med en takk til {triggerName} for å inkludere deg i samtalen" +
                      $"Svar så med å anbefale {username} noe." +
                      "Ikke bruk frasen \"Lykke til\".";
        var chat1 = $"{username}: '{text}'\n\n";
        var chat2 = $"{triggerName}: 'Hva synes du?'\n\n";

        var full = $"{priming}\n\n Dialog:\n\n{chat1}{chat2}";
        Console.WriteLine(full);

        var p2 = new ChatPrompt("user", full);
        return new[] { p1, p2 };
    }
}
