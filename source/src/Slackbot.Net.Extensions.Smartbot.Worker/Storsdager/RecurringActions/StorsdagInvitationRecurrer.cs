using System.Threading;
using System.Threading.Tasks;
using CronBackgroundServices;

namespace Smartbot.Utilities.Storsdager.RecurringActions
{
    public class StorsdagInvitationRecurrer : IRecurringAction
    {
        private readonly StorsdagInviter _inviter;
    
        public StorsdagInvitationRecurrer(StorsdagInviter inviter)
        {
            _inviter = inviter;
        }
    
        public async Task Process(CancellationToken token)
        {
            await _inviter.InviteNextStorsdag();
        }

        public string Cron => Crons.ThirdSaturdayOfMonth;
    }
}