namespace Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Body { get; set; }
        public long LifeTime { get; set; }
        public bool IsRevoked { get; set; }
    }
}
