using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInvitationRecurrer : IRecurringAction
    {
        private readonly StorsdagInviter _inviter;
    
        public StorsdagInvitationRecurrer(StorsdagInviter inviter)
        {
            _inviter = inviter;
        }
    
        public async Task Process()
        {
            await _inviter.InviteNextStorsdag();
        }

        public string Cron => Crons.ThirdSaturdayOfMonth;
    }
}