namespace Domain.Entities
{
    public class UserInfo : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Browser { get; set; }
        public string? OS { get; set; }
        public string? Device { get; set; }
        public string? Locale { get; set; }
        public string? TimeZone { get; set; }
        public string? IP { get; set; }
    }
}
