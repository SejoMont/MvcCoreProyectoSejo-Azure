using System.Net.Mail;
using System.Net;

namespace MvcCoreProyectoSejo.Helpers
{
    // Clase que proporciona métodos para enviar correos electrónicos
    public class HelperMails
    {
        private readonly IConfiguration configuration;

        // Constructor que recibe la configuración necesaria para enviar correos
        public HelperMails(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // Método privado para configurar un objeto MailMessage
        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje)
        {
            MailMessage mail = new MailMessage();

            // Configurar el remitente y destinatario
            string user = this.configuration.GetValue<string>("MailSettings:Credentials:User");
            mail.From = new MailAddress(user);
            mail.To.Add(para);

            // Configurar el asunto, cuerpo y prioridad del correo
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            return mail;
        }

        // Método privado para configurar un objeto SmtpClient
        private SmtpClient ConfigureSmtpClient()
        {
            // Configuración del servidor SMTP
            string password = this.configuration.GetValue<string>("MailSettings:Credentials:Password");
            string hostName = this.configuration.GetValue<string>("MailSettings:ServerSmtp:Host");
            int port = this.configuration.GetValue<int>("MailSettings:ServerSmtp:Port");
            bool enableSSL = this.configuration.GetValue<bool>("MailSettings:ServerSmtp:EnableSsl");
            bool defaultCredentials = this.configuration.GetValue<bool>("MailSettings:ServerSmtp:DefaultCredentials");
            string user = this.configuration.GetValue<string>("MailSettings:Credentials:User");

            // Crear el cliente SMTP
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = hostName;
            smtpClient.Port = port;
            smtpClient.EnableSsl = enableSSL;
            smtpClient.UseDefaultCredentials = defaultCredentials;

            // Configuración de las credenciales de red para enviar el correo
            NetworkCredential credentials = new NetworkCredential(user, password);
            smtpClient.Credentials = credentials;

            return smtpClient;
        }

        // Método público para enviar un correo sin adjuntos
        public async Task SendMailAsync(string para, string asunto, string mensaje)
        {
            // Crear un objeto MailMessage con los datos proporcionados
            MailMessage mail = this.ConfigureMailMessage(para, asunto, mensaje);

            // Configurar el cliente SMTP
            SmtpClient smtp = this.ConfigureSmtpClient();

            // Enviar el correo electrónico
            await smtp.SendMailAsync(mail);
        }

        // Método público para enviar un correo con un archivo adjunto
        public async Task SendMailAsync(string para, string asunto, string mensaje, string pathAttachment)
        {
            // Crear un objeto MailMessage con los datos proporcionados
            MailMessage mail = this.ConfigureMailMessage(para, asunto, mensaje);

            // Crear un objeto Attachment para el archivo adjunto
            Attachment attachment = new Attachment(pathAttachment);
            mail.Attachments.Add(attachment);

            // Configurar el cliente SMTP
            SmtpClient smtp = this.ConfigureSmtpClient();

            // Enviar el correo electrónico con el archivo adjunto
            await smtp.SendMailAsync(mail);
        }
    }
}