# About

A framework for hosting a Slack bot in a .NET Core 2.2 host

# Host setup

Sample setup, using .NET Core DI extensions:
```
services.AddSlackbot(o => { o.Slackbot_SlackApiKey_BotUser = "sometoken"  })
    .AddPublisher<SlackPublisher>()
    .AddPublisher<LoggerPublisher>()
    .AddPublisher<MyCustomPublisher>()
    .AddRecurring<MyEightOClockRecurrer>(c => c.Cron = "0 0 8 * * *")
    .AddHandler<MyHandlerOfIncomingMessages>()
})

```

# Features

## Recurring actions
If you want to run code snippets at recurring intervals (every day at 8AM, or every minute), register an implementation of `RecurringAction` using `.AddRecurring<T>(..)` and configure the interval using a Cron expression. This is not tied to Slack in any way, but you may for example post a message to Slack at these intervals if you want.

## Execute a handler
If you want to run code given a certain message was posted to Slack, register an implementation of `IHandleMessages` using `.AddHandler<T>`.

The `IHandleMessages` interface requires that you provide when the handler should execute (`ShouldHandle`), and what code it should run if (`Handle`)

```
public interface IHandleMessages
{
      Task<HandleResponse> Handle(SlackMessage message);
      bool ShouldHandle(SlackMessage message);
}
```

Sample implementation:

```
  public async Task<HandleResponse> Handle(SlackMessage message)
  {
      // run any code
      return new HandleResponse("OK");
  }

  public bool ShouldHandle(SlackMessage message)
  {
      return message.MentionsBot;
  }
```

## Publishers
The library does not force you to use them, but if you want you can make use of included `IPublish` implementations:

* `SlackPublisher`
* `LoggerPublisher`

These are available via DI and can be used in any `IHandle` or `RecurringAction` implementation. You can also create your own.

Sample handler using `IPublisher` registered:

```
public class SampleHandler : IHandleMessages
{
    private readonly IEnumerable<IPublisher> _publishers;

    public SampleHandler(IEnumerable<IPublisher> publishers)
    {
        _publishers = publishers;
    }

    public async Task<HandleResponse> Handle(SlackMessage message)
    {
        foreach (var publisher in _publishers)
        {
            var notification = new Notification
            {
                Msg = aggr,
                Channel = message.ChatHub.Id //here: replying to the same channel
            };
            await publisher.Publish(notification);
        }
        return new HandleResponse("OK");
    }

    public bool ShouldHandle(SlackMessage message)
    {
        return message.MentionsBot
    }
}
```

Similarly in an `RecurringAction`:

```
public class SampleRecurringAction : RecurringAction
{
    private readonly IEnumerable<IPublisher> _publishers;

    public SampleRecurringAction(IEnumerable<IPublisher> publishers,
        ILogger<SampleRecurringAction> logger,
        IOptionsSnapshot<CronOptions> options)
        : base(options,logger)
    {
        _publishers = publishers;
    }

    public override async Task Process()
    {
        foreach (var p in _publishers)
        {
            var notification = new Notification
            {
                Msg = $"Sample message",
                IconEmoji = ":cake:",
                Channel = "#somechannel"
            };
            await p.Publish(notification);
        }
    }
}
```

