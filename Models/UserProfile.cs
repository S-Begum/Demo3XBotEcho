namespace XBotEcho.Models
{
    public class UserProfile
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string TimeZone { get; set; }
    }
}
