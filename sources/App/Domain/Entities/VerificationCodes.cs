using Domain.Entities;

namespace Domain.Entities
{
    public class VerificationCode : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public long TimeToLive { get; set; }
        public VerificationCodeType Type { get; set; }
        public VerificationCodeDestination Destination { get; set; }
        public bool Used { get; set; }
    }

    public enum VerificationCodeType
    {
        Auth
    }

    public enum VerificationCodeDestination
    {
        Email,
        Phone
    }
}
