using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace yildiz.web.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string username,
        string resetLink)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _configuration["Smtp:FromName"],
                _configuration["Smtp:FromEmail"]));

        message.To.Add(
            new MailboxAddress(
                username,
                email));

        message.Subject = "Yıldız Mağaza - Şifre Sıfırlama";

        var body = $"""
        Merhaba {username},

        Yıldız Mağaza hesabınız için şifre sıfırlama talebi aldık.

        Şifrenizi yenilemek için aşağıdaki bağlantıya tıklayın:

        {resetLink}

        Bu bağlantı 30 dakika boyunca geçerlidir ve yalnızca bir kez kullanılabilir.

        Bu isteği siz yapmadıysanız bu e-postayı dikkate almayabilirsiniz.

        Yıldız Mağaza
        """;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _configuration["Smtp:Host"],
            int.Parse(_configuration["Smtp:Port"]!),
            SecureSocketOptions.StartTls);

        var smtpPassword = _configuration["Smtp:Password"];

        if (string.IsNullOrWhiteSpace(smtpPassword))
        {
            throw new Exception(
                "SMTP App Password okunamadı.");
        }

        if (smtpPassword.Length != 16)
        {
            throw new Exception(
                "SMTP App Password okunuyor ancak uzunluğu 16 değil: "
                + smtpPassword.Length);
        }

        await smtp.AuthenticateAsync(
            _configuration["Smtp:Username"],
            smtpPassword);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}