namespace Smartbot.Data.Storage.Events
{
    public static class RsvpValues
    {
        public const string Invited = "invited";
        public const string Attending = "attending";
        public const string Maybe = "maybe";
        public const string NotAttending = "no";
    }

    public static class RsvpActionIds
    {
        public const string Attending = "storsdag-rsvp-attending";
        public const string Maybe = "storsdag-rsvp-maybe";
        public const string NotAttending = "storsdag-rsvp-not-attending";
    }
}