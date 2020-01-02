using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slackbot.Net;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInvitationRecurrer : RecurringAction
    {
        private readonly StorsdagInviter _inviter;

        public StorsdagInvitationRecurrer(StorsdagInviter inviter, ILogger<StorsdagInvitationRecurrer> logger): base(Crons.ThirdSaturdayOfMonth, logger)
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