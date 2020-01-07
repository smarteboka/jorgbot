using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smartbot.Utilities.Storage.Events
{
    public interface IInvitationsStorage
    {
        Task<InvitationsEntity> Save(InvitationsEntity invite);
        Task<InvitationsEntity> Update(string inviteId, string rsvp, DateTimeOffset? rsvpTime = null);
        Task<InvitationsEntity> GetById(string inviteId);
        Task<IEnumerable<InvitationsEntity>> GetInvitations(string eventId);
        Task<List<EventEntity>> GetInvitations();
    }
}