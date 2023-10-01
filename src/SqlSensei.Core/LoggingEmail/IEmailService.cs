#nullable enable

using System.Threading.Tasks;

namespace SqlSensei.Core.Logging.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string content, string? fileName, byte[]? attachmentBytes, string? attachmentContentType);
    }
}
