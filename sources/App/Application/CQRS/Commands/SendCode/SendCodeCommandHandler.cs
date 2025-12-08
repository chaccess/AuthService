using Application.Services.VerificationCodesService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Commands.SendCode
{
    public class SendCodeCommandHandler(IVerificationCodesService codesService) : IRequestHandler<SendCodeCommand, SendCodeCommandResponse>
    {
        private readonly IVerificationCodesService _codesService = codesService;

        public async Task<SendCodeCommandResponse> Handle(SendCodeCommand request, CancellationToken cancellationToken)
        {
            var loginType = await _codesService.SendCode(request.Login);

            return new SendCodeCommandResponse { IsSuccess = true, LoginType = loginType };
        }
    }
}
