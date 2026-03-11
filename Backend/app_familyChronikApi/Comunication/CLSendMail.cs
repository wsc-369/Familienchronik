using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace appAhnenforschungBackEnd.Comunication
{
    public class CLSendMail
    {

        // -------------------------------------------------------------------------------------
        public bool SendMail(CMailConfiguration i_oConfiguration, string i_strFrom, string i_strTo, string i_strSubject, string i_strMessage, bool i_IsBodyHtml)
        {
            bool bResult = false;
            string strEMailFrom = "";
            string strEMailTo = "";
            string strNetworkCredentialUser = "";
            string strNetworkCredentialUserPW = "";

            MailMessage oMailMessage;
            string strClient = i_oConfiguration.Client;// System.Configuration.ConfigurationManager.AppSettings.Get("SMTP_CLIENT");
            //System.Net.Mail.SmtpClient oSmtpClient;

            try
            {
                if (i_strTo.Length > 0)
                {
                    strEMailTo = StripMailAdress(i_strTo);
                    strEMailFrom = StripMailAdress(i_strFrom);
                    oMailMessage = new MailMessage(strEMailFrom, CreateEmailFistInCollection(strEMailTo));
                    oMailMessage.Subject = i_strSubject;
                    oMailMessage.Body = i_strMessage;


                    if (i_IsBodyHtml)
                    {
                        oMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        oMailMessage.IsBodyHtml = i_IsBodyHtml;
                    }
                    CreateEmailNextCollection(strEMailTo, ref oMailMessage);
                }
                else
                {
                    //strEMailFrom = StripMailAdress(i_strFrom);
                    oMailMessage = new System.Net.Mail.MailMessage(strEMailFrom, strEMailFrom, i_strSubject, i_strMessage);
                }

                strNetworkCredentialUser = i_oConfiguration.NETWORK_CREDENTIAL_USER;// CGlobal.NETWORK_CREDENTIAL_USER();
                strNetworkCredentialUserPW = i_oConfiguration.NETWORK_CREDENTIAL_PW;// CGlobal.NETWORK_CREDENTIAL_PW();

                using (SmtpClient oSmtpClient = new SmtpClient(strClient))
                {
                    oSmtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    oSmtpClient.Credentials = new NetworkCredential(strNetworkCredentialUser, strNetworkCredentialUserPW);
                    oSmtpClient.EnableSsl = false;
                    oSmtpClient.Send(oMailMessage);
                }

                bResult = true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Error in processing. The server response was: 4.7.1  GreylistedRecipient address rejected: Greylisted for 90 seconds (see http://www.webland.ch/support/ms_greylisting.aspx)")
                {
                    bResult = true;
                    // this.SendMail(i_oConfiguration,i_strFrom,i_strTo,i_strSubject,i_strMessage,i_IsBodyHtml);
                }
                else
                {
                    bResult = false;
                }
                //CGlobal.LogError(ex.Message.ToString());
                Console.WriteLine(ex.Message);

            }
            //  Throw ex
            //End Try


            return bResult;

        }

        /// <summary>
        /// Ein eMail mit Attachment senden.
        /// </summary>
        /// <param name="i_oConfiguration"></param>
        /// <param name="i_strFrom"></param>
        /// <param name="i_strTo"></param>
        /// <param name="i_strSubject"></param>
        /// <param name="i_strMessage"></param>
        /// <param name="i_IsBodyHtml"></param>
        /// <param name="i_strFilename"></param>
        public void SendMailWithAttachment(CMailConfiguration i_oConfiguration, string i_strFrom, string i_strTo, string i_strSubject, string i_strMessage, bool i_IsBodyHtml, string i_strFilename)
        {
            string strEMailFrom = "";
            string strEMailTo = "";
            string strNetworkCredentialUser = "";
            string strNetworkCredentialUserPW = "";
            string strClient = "";

            System.Net.Mail.MailMessage oMailMessage;
            System.Net.Mail.SmtpClient oSmtpClient;

            strClient = i_oConfiguration.Client;

            try
            {
                if (i_strTo.Length > 0)
                {
                    strEMailTo = StripMailAdress(i_strTo);
                    strEMailFrom = StripMailAdress(i_strFrom);

                    //oMailMessage = new System.Net.Mail.MailMessage(strEMailFrom, strEMailTo);
                    oMailMessage = new System.Net.Mail.MailMessage(strEMailFrom, CreateEmailFistInCollection(strEMailTo));
                    oMailMessage.Subject = i_strSubject;
                    oMailMessage.Body = i_strMessage;

                    // Attachment
                    if (i_strFilename.Length > 0)
                    {
                        oMailMessage.Attachments.Add(new System.Net.Mail.Attachment(i_strFilename));
                    }

                    if (i_IsBodyHtml)
                    {
                        oMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        oMailMessage.IsBodyHtml = i_IsBodyHtml;
                    }
                    CreateEmailNextCollection(strEMailTo, ref oMailMessage);
                }
                else
                {
                    //strEMailFrom = StripMailAdress(i_strFrom);
                    oMailMessage = new System.Net.Mail.MailMessage(strEMailFrom, strEMailFrom, i_strSubject, i_strMessage);
                }

                strNetworkCredentialUser = i_oConfiguration.NETWORK_CREDENTIAL_USER;// CGlobal.NETWORK_CREDENTIAL_USER();
                strNetworkCredentialUserPW = i_oConfiguration.NETWORK_CREDENTIAL_PW;// CGlobal.NETWORK_CREDENTIAL_PW();

                oSmtpClient = new System.Net.Mail.SmtpClient(strClient);
                oSmtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                oSmtpClient.Credentials = new System.Net.NetworkCredential(strNetworkCredentialUser, strNetworkCredentialUserPW);
                oSmtpClient.Send(oMailMessage);

            }
            catch (Exception)
            {
                throw;
            }
        }

        // -------------------------------------------------------------------------------------
        public bool SendMailNewsLetter(CMailConfiguration i_oConfiguration, string i_strFrom, string i_strTo, string i_strSubject, string i_strMessage, bool i_IsBodyHtml)
        {
            bool bResult = false;
            string strEMailFrom = "";
            string strEMailTo = "";
            string strNetworkCredentialUser = "";
            string strNetworkCredentialUserPW = "";

            System.Net.Mail.MailMessage oMailMessage;
            string strClient = i_oConfiguration.Client;// System.Configuration.ConfigurationManager.AppSettings.Get("SMTP_CLIENT");
            System.Net.Mail.SmtpClient oSmtpClient;

            try
            {
                if (i_strTo.Length > 0)
                {
                    strEMailTo = StripMailAdress(i_strTo);
                    strEMailFrom = StripMailAdress(i_strFrom);

                    oMailMessage = new System.Net.Mail.MailMessage(strEMailFrom, strEMailFrom);

                    strEMailTo = strEMailTo.Replace(";", ",");
                    oMailMessage.Bcc.Add(strEMailTo);
                    oMailMessage.Subject = i_strSubject;
                    oMailMessage.Body = i_strMessage;
                    if (i_IsBodyHtml)
                    {
                        oMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        oMailMessage.IsBodyHtml = i_IsBodyHtml;
                    }
                    //CreateEmailCollection(strEMailTo, ref oMailMessage);
                }
                else
                {
                    //strEMailFrom = StripMailAdress(i_strFrom);
                    oMailMessage = new System.Net.Mail.MailMessage(strEMailFrom, strEMailFrom, i_strSubject, i_strMessage);
                }

                strNetworkCredentialUser = i_oConfiguration.NETWORK_CREDENTIAL_USER;// CGlobal.NETWORK_CREDENTIAL_USER();
                strNetworkCredentialUserPW = i_oConfiguration.NETWORK_CREDENTIAL_PW;// CGlobal.NETWORK_CREDENTIAL_PW();

                oSmtpClient = new System.Net.Mail.SmtpClient(strClient);
                oSmtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                oSmtpClient.Credentials = new System.Net.NetworkCredential(strNetworkCredentialUser, strNetworkCredentialUserPW);
                oSmtpClient.Send(oMailMessage);
                bResult = true;
            }
            catch
            {
                //CGlobal.LogError(ex.Message.ToString());
                bResult = false;
            }
            //  Throw ex
            //End Try
            return bResult;

        }


        // -------------------------------------------------------------------------------------
        // Alle Empfanger in die Collection ablegen
        private string CreateEmailFistInCollection(string i_strEmails)
        {

            string strTo = i_strEmails;
            string[] Emails = i_strEmails.Split(';');
            foreach (string word in Emails)
            {
                strTo = word;
                //io_oMailMessage.To.Add(word);
                break;
            }
            return strTo;
        }


        // -------------------------------------------------------------------------------------
        // Alle Empfanger in die Collection ablegen
        private void CreateEmailNextCollection(string i_strEmails, ref System.Net.Mail.MailMessage io_oMailMessage)
        {
            int i = 0;
            string[] Emails = i_strEmails.Split(';');
            foreach (string word in Emails)
            {
                if (i > 0)
                {
                    io_oMailMessage.To.Add(word);
                }
                i += 1;
            }
        }

        // -------------------------------------------------------------------------------------
        // Alle Empfanger in die Collection ablegen
        private void CreateEmailCollection(string i_strEmails, ref System.Net.Mail.MailMessage io_oMailMessage)
        {

            string[] Emails = i_strEmails.Split(';');
            foreach (string word in Emails)
            {
                io_oMailMessage.To.Add(word);
            }
        }

        // -------------------------------------------------------------------------------------
        // Das Letzte ";" entfernen
        private string StripMailAdress(string i_strAdress)
        {
            string strResult = i_strAdress;
            string strLastChar = i_strAdress.Substring(i_strAdress.Length - 1);

            if (strLastChar == ";")
            {
                int lastSlash = i_strAdress.LastIndexOf(";");
                if (lastSlash > 0)
                {
                    strResult = i_strAdress.Substring(0, lastSlash);
                }
            }
            return strResult;
        }

        // -------------------------------------------------------------------------------------
        private void LogUpload(string i_strMessage, string i_strFilePath)
        {

            using (StreamWriter sw = new StreamWriter(i_strFilePath))
            {

                sw.Write(i_strMessage);
                //sw.Write("This is the ");
                //sw.WriteLine("header for the file.");
                //sw.WriteLine("-------------------");
                //// Arbitrary objects can also be written to the file.
                //sw.Write("The date is: ");
                //sw.WriteLine(DateTime.Now);
            }

        }

    }
}
