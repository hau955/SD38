using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;

namespace AppApi.Features.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Email người nhận không được để trống", nameof(toEmail));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Tiêu đề email không được để trống", nameof(subject));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Nội dung email không được để trống", nameof(message));

            // Get email settings with validation
            var emailSettings = _configuration?.GetSection("EmailSettings")
                ?? throw new InvalidOperationException("Cấu hình email không tồn tại");

            var fromName = emailSettings["FromName"] ?? "Áo Dài Việt";
            var fromEmail = emailSettings["FromEmail"];
            var host = emailSettings["Host"];
            var portStr = emailSettings["Port"];
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            // Validate email settings
            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("FromEmail không được cấu hình");
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("SMTP Host không được cấu hình");
            if (!int.TryParse(portStr, out var port) || port <= 0)
                throw new InvalidOperationException("Port SMTP không hợp lệ");
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("SMTP Username không được cấu hình");
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SMTP Password không được cấu hình");

            // Create email message
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            // Build email body with improved HTML template
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1'>
            <title>{subject}</title>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ 
                    background: linear-gradient(135deg, #fb923c 0%, #f59e0b 100%); 
                    padding: 20px; 
                    text-align: center; 
                    border-radius: 8px 8px 0 0;
                }}
                .header h1 {{ color: white; margin: 0; }}
                .content {{ padding: 20px; background: #fff; }}
                .footer {{ 
                    background: #f5f5f5; 
                    padding: 15px; 
                    text-align: center; 
                    font-size: 12px; 
                    color: #666;
                    border-radius: 0 0 8px 8px;
                }}
                .btn {{
                    display: inline-block;
                    background-color: #fb923c;
                    color: white;
                    padding: 12px 24px;
                    text-decoration: none;
                    border-radius: 8px;
                    font-weight: bold;
                    margin: 20px 0;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Áo Dài Việt</h1>
                </div>
                <div class='content'>
                    {message}
                    <p style='margin-top: 30px;'>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>
                </div>
                <div class='footer'>
                    <p>© {DateTime.Now.Year} Áo Dài Việt. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>"
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // Configure timeout
                client.Timeout = 60000; // 30 seconds

                // Connect to SMTP server
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync(username, password);

                // Send email
                await client.SendAsync(emailMessage);
            }
            catch (SmtpCommandException ex)
            {
                throw new InvalidOperationException($"Lỗi SMTP khi gửi email: {ex.Message} (Status: {ex.StatusCode})", ex);
            }
            catch (SmtpProtocolException ex)
            {
                throw new InvalidOperationException($"Lỗi giao thức SMTP: {ex.Message}", ex);
            }
            catch (AuthenticationException ex)
            {
                throw new InvalidOperationException($"Xác thực SMTP thất bại: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi gửi email: {ex.Message}", ex);
            }
            finally
            {
                try
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
                catch
                {
                    // Ignore disconnect errors
                }
            }
        }
    }
}
