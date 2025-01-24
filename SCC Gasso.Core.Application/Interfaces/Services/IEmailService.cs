using SCC_Gasso.Core.Application.Dtos.Email;

namespace SCC_Gasso.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
