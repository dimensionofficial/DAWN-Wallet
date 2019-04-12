using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class SendMail : MonoBehaviour
{

    void Start()
    {
        Message();
    }

    public void Message()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("1019716366@qq.com", "Draco");
        mail.To.Add(new MailAddress("3334065155@qq.com", "Draco用户"));
        mail.Subject = "Test Mail";
        mail.Body = "This is for testing SMTP mail from GMAIL";

        SmtpClient smtpServer = new SmtpClient("smtp.qq.com");
        smtpServer.Credentials = new System.Net.NetworkCredential("1019716366@qq.com", "lkgdfqewgivgbcbj") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

        smtpServer.Send(mail);
        Debug.Log("success");

        //MailMessage mailMsg = new MailMessage();
        //mailMsg.From = new MailAddress("1019716366@qq.com", "Draco");
        //mailMsg.To.Add(new MailAddress("3334065155@qq.com", "Draco用户"));
        //mailMsg.Subject = "Test Mail";
        //mailMsg.Body = "This is for testing SMTP mail from GMAIL";
        //SmtpClient client = new SmtpClient("smtp.qq.com", 25);  //发送服务器
        //client.Credentials = (ICredentialsByHost)new NetworkCredential("1019716366@qq.com", "lkgdfqewgivgbcbj");
        //client.Send(mailMsg);
        //Debug.Log("OK");
    }
}