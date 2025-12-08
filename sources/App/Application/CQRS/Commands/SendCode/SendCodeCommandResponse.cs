namespace Application.CQRS.Commands.SendCode
{
    public class SendCodeCommandResponse
    {
        public bool IsSuccess { get; set; }

        public string LoginType { get; set; }
    }
}
