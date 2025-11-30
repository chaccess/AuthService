namespace Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = new Guid();

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }
    }
}
