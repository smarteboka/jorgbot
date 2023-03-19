using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Interactive.MessageActions;
using Slackbot.Net.SlackClients.Http;
using Slackbot.Net.SlackClients.Http.Models.Requests.ChatPostMessage;

namespace Smartbot.Utilities.Handlers;

public class MessageActionsHandler : IHandleMessageActions
{
    private readonly ISlackClient _slackClient;
    private readonly ILogger<MessageActionsHandler> _logger;

    public MessageActionsHandler(ISlackClient slackClient, ILogger<MessageActionsHandler> logger)
    {
        _slackClient = slackClient;
        _logger = logger;
    }
    
    public Task<EventHandledResponse> Handle(MessageActionInteraction @event)
    {
        _logger.LogInformation(JsonConvert.SerializeObject(@event));
        Task.Run(async () =>
        {
            var key = Environment.GetEnvironmentVariable("SMARTBOT_OPENAI_KEY");
            var client = new OpenAIClient(key);

            var completionPrompt = @event.Callback_Id switch
            {
                "gpt_critico" => ElCritico(@event.Message.Text),
                "gpt_tldr" => await ThreadTLDR(@event.Channel.Id, @event.Message_Ts),
                _ => null
            };

            if (completionPrompt != null)
            {
                var res = await client.ChatEndpoint.GetCompletionAsync(new ChatRequest(completionPrompt));
                var completions = res.FirstChoice.Message.Content;

                await _slackClient.ChatPostMessage(new ChatPostMessageRequest
                {
                    Channel = @event.Channel.Id,
                    thread_ts = @event.Message_Ts,
                    Text = completions,
                    Link_Names = true,
                    Reply_Broadcast = true
                });         
            }
            else
            {
                _logger.LogInformation($"Not doing anything. Unknown callback_id {@event.Callback_Id}");
            }
        });

        return Task.FromResult(new EventHandledResponse("OK"));
    }

    private static IEnumerable<ChatPrompt> ElCritico(string text)
    {
        yield return new ChatPrompt("user", $"Jeg vil at du skal late som du er en satiriker. Jeg vil gi deg" +
                                            $"en tekst, som du skal forklare i en sarkastisk tone og belyse ulike temaer" +
                                            $"i lys av dagens tid. Dra paraleller til store historiske hendelser og personer. Bruk maks 100 ord. \n\n" +
                                            $"Teksten du skal forklare: \n" +
                                            $"{text}");
    }
    
    private async Task<IEnumerable<ChatPrompt>> ThreadTLDR(string channel, string ts)
    {
        _logger.LogInformation("kom hit1 ");
        var replies = await _slackClient.ConversationsReplies(channel, ts);
        var usersInReplies = await _slackClient.UsersList();
        var allMembers = usersInReplies.Members;
        _logger.LogInformation("kom hit2");
        var chatDialog = replies.Messages.Select(c => $"{allMembers.FirstOrDefault(a => a.Id == c.User)?.Name ?? "En smarting"} : '{c.Text}'\n");
        _logger.LogInformation(string.Join("\n", chatDialog));
        _logger.LogInformation("kom hit3");
        var p1 = new ChatPrompt("system", "Du er en filmkritiker som elsker å anmelde meldingsutvekslinger mellom venner med satire");
        var p2 = new ChatPrompt("user",
            "Du er en filmkritiker som elsker å anmelde meldingsutvekslinger mellom venner med satire.\n"+
            "Jeg vil at du skal tolke en dialog mellom venner\n." +
            "Oppsummer samtalen i mellom disse vennene i et referat på maks 80 ord. " +
            "Svar på norsk. " +
            "Ikke bruk ordene \"slack-tråd\", \"slack\", \"manus\", \"film\" eller \"dialog\""+
            "Oppsummeringen må inneholde følgende:\n" +
            "1) Start med å skryte av den første karakteren som starter dialogen. " +
            "Alltid nevn navnet på den første karakteren. " +
            "Fortell hvor bra første melding er. Dette er alltid en bra påstand eller et godt spørsmål. " +
            "Bruk superlativer når du beskriver den første meldingen eller beskriver den første karakteren. "+
            "Du er glad for at noen har dette som tema.\n" +
            "2) Fortell, bruk sarkasme, ironi og språklige bilder, metaforer for å oppsummere resten av meldingene. " +
            "Oppsummeringen må fortelle litt ekstra om karakterene som deltar."+
            "Gi til slutt et lavt terningkast. Eksempler:" +
            " \"Terningkast 0\".\n"+
            " \"Terningkast 1\".\n"+
            " \"Terningkast 3\".\n"+
            "Film-manus-dialog:\n\n"+
            $"{string.Join("\n", chatDialog)}");

        return new[] { p1, p2 };
    }
}
