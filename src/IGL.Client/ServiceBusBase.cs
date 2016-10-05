using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IGL.Client
{
    public abstract class ServiceBusBase
    {
        protected static string GetToken()
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

            var acsEndpoint = "https://" + Configuration.ServiceNamespace + "-sb." + Configuration.ACSHostName + "/WRAPv0.9/";

            // Note that the realm used when requesting a token uses the HTTP scheme, even though
            // calls to the service are always issued over HTTPS
            var realm = "http://" + Configuration.ServiceNamespace + "." + Configuration.SBHostName + "/";

            NameValueCollection values = new NameValueCollection();
            values.Add("wrap_name", Configuration.IssuerName);
            values.Add("wrap_password", Configuration.IssuerSecret);
            values.Add("wrap_scope", realm);

            WebClient webClient = new WebClient();
            byte[] response = webClient.UploadValues(acsEndpoint, values);

            string responseString = Encoding.UTF8.GetString(response);

            var responseProperties = responseString.Split('&');
            var tokenProperty = responseProperties[0].Split('=');
            var token = Uri.UnescapeDataString(tokenProperty[1]);

            return "WRAP access_token=\"" + token + "\"";
        }

        private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Return true if the server certificate is ok
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            bool acceptCertificate = true;

            //The server did not present a certificate
            if ((sslPolicyErrors &
                 SslPolicyErrors.RemoteCertificateNotAvailable) == SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                acceptCertificate = false;
            }
            else
            {
                //The certificate does not match the server name
                if ((sslPolicyErrors &
                     SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    acceptCertificate = false;
                }

                //There is some other problem with the certificate
                if ((sslPolicyErrors &
                     SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    foreach (X509ChainStatus item in chain.ChainStatus)
                    {
                        if (item.Status != X509ChainStatusFlags.RevocationStatusUnknown &&
                            item.Status != X509ChainStatusFlags.OfflineRevocation)
                            break;

                        if (item.Status != X509ChainStatusFlags.NoError)
                        {
                            acceptCertificate = false;
                        }
                    }
                }
            }

            //If Validation failed, present message box
            if (acceptCertificate == false)
            {                
                acceptCertificate = true;
            }

            return acceptCertificate;
        }
    }
}
