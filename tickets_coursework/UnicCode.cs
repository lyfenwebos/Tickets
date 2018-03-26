using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace tickets_coursework
{
    class UnicCode
    {
        DataBase db = new DataBase();

        public string Generating()
        {
            Random rand = new Random();
            string code = "";
            while (code.Length < 10)
            {
                code += rand.Next(0, 9);
            }
            return code;
        }

        public void GenerateMessage(string text, string email, int sessionID)
        {
            
            List<string> generatedCodes = db.GetCodes(sessionID);
            string code = Generating();
            foreach (string element in generatedCodes)
            {
                while (element == code)
                {
                    code = Generating();
                }
            }
            string message = "<b> <h2> Hello! </h2> </b> <br> Thank you for using our service. </br> <br> Your session information is: " + text + ".</br> <br> Use this code to get your tickets in the cinema: " + code + "</br>";
            db.InsertCodeDB(sessionID, code);
            SendMessage(message, email, code);
            
        }

        public void SendMessage (string msg, string mail, string code)
        {
            MailAddress from = new MailAddress("cinemacash@exordium.cloud", "Cinema Support");
            MailAddress to = new MailAddress(mail);

            SmtpClient mailClient = new SmtpClient("mail.exordium.clou", 587);
            mailClient.EnableSsl = true;
            MailMessage msgMail = new MailMessage();
            msgMail.From = from;
            msgMail.To.Add(to);
            msgMail.Subject = "Your unic Code is ready!";
            msgMail.Body = msg;
            msgMail.IsBodyHtml = true;
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate,
             X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
            try
            {
                mailClient.Send(msgMail);
                msgMail.Dispose();
            }
            catch
            {
                db.DeleteCode(code);
            }
            
        }

    }
}
