using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net.Workers;
using Slackbot.Net.Workers.Configuration;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInvitationRecurrer : RecurringAction
    {
        private readonly StorsdagInviter _inviter;

        public StorsdagInvitationRecurrer(IOptionsSnapshot<CronOptions> options, StorsdagInviter inviter, ILogger<StorsdagInvitationRecurrer> logger): base(options, logger)
        {
            _inviter = inviter;
        }

        public StorsdagInvitationRecurrer(string cron, StorsdagInviter inviter, ILogger<StorsdagInvitationRecurrer> logger): base(cron, logger)
        {
            _inviter = inviter;
        }

        public override async Task Process()
        {
            await _inviter.InviteNextStorsdag();
        }
    }
}