using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Compori.MagentoApi.Authentication
{
    public class TokenRequest : ITokenRequest
    {
        /// <summary>
        /// Requests the admin token with HTTP authentication.
        /// </summary>
        /// <param name="tokenTyp">The token typ.</param>
        /// <param name="baseEndpointAddress">The base endpoint address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="httpAuthUser">The HTTP authentication user.</param>
        /// <param name="httpAuthPassword">The HTTP authentication password.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentException">The token type '{tokenTyp}' is not not supported. - tokenTyp</exception>
        /// <exception cref="ArgumentException">The enpoint adress must be set. - baseEndpointAddress</exception>
        /// <exception cref="System.ArgumentException">The user must be set. - userName</exception>
        /// <exception cref="ArgumentException">The password must be set. - password</exception>
        /// <exception cref="TokenRequestException"></exception>
        /// <exception cref="TokenRequestException">Could not request token. " + ex.Message</exception>
        public static async Task<string> RequestTokenAsync(TokenType tokenTyp, string baseEndpointAddress, string userAgent, string userName, string password, string httpAuthUser = null, string httpAuthPassword = null)
        {
            if(tokenTyp != TokenType.Admin || tokenTyp == TokenType.Customer)
            {
                throw new ArgumentException($"The token type '{tokenTyp}' is not not supported.", nameof(tokenTyp));
            }

            if(string.IsNullOrWhiteSpace(baseEndpointAddress))
            {
                throw new ArgumentException("The enpoint adress must be set.", nameof(baseEndpointAddress));
            }
            baseEndpointAddress = baseEndpointAddress.TrimEnd('/');

            if(string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("The user must be set.", nameof(userName));
            }
            if(string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("The password must be set.", nameof(password));
            }

            try
            {
                //
                // explizit setting security protocol.
                //
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                //
                // Prepare default http message handler
                //
                var handler = new HttpClientHandler();

                var typeUrlPart = tokenTyp == TokenType.Customer ? "customer" : "admin";

                if(!string.IsNullOrEmpty(httpAuthUser) && !string.IsNullOrEmpty(httpAuthPassword)){
                    handler.Credentials = new NetworkCredential(httpAuthUser, httpAuthPassword);
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    if (!string.IsNullOrWhiteSpace(userAgent))
                    { 
                        client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    }
                    var response = await client.PostAsync(
                        baseEndpointAddress + $"/index.php/rest/V1/integration/{typeUrlPart}/token",
                        CreateRequestContent(userName, password));

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = "Error: " + response.ReasonPhrase + " (" + ((int)response.StatusCode).ToString() + ").";
                        var additionalMessage = await GetMessageFromResponseContent(response.Content);
                        if(additionalMessage != null)
                        {
                            message = message + " " + additionalMessage;
                        }
                        throw new TokenRequestException(message);
                    }

                    return await GetTokenFromResponseContent(response.Content);
                }
            }
            catch (TokenRequestException)
            {
                //
                // rethrow an existing token request exception
                //
                throw;
            }
            catch (Exception ex)
            {
                throw new TokenRequestException("Could not request token. " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Creates the content of the request.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>StringContent.</returns>
        private static StringContent CreateRequestContent(string userName, string password)
        {
            var doc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";

            doc.LoadXml("<login></login>");
            doc.InsertBefore(xmldecl, doc.DocumentElement);

            //
            // Add username
            //
            var userNameElem = doc.CreateElement("username");
            userNameElem.InnerText = userName;
            doc.DocumentElement.AppendChild(userNameElem);

            //
            // Add password
            //
            var passwordElem = doc.CreateElement("password");
            passwordElem.InnerText = password;
            doc.DocumentElement.AppendChild(passwordElem);

            return new StringContent(doc.OuterXml.ToString(), Encoding.UTF8, "application/xml");
        }

        /// <summary>
        /// Gets the message from XML response.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.String.</returns>
        private static async Task<string> GetTokenFromResponseContent(HttpContent content)
        {
            var doc = new XmlDocument();
            var stream = await content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                doc.LoadXml(text);
                var node = doc.SelectSingleNode("/response");
                return node?.InnerText;
            }
        }

        /// <summary>
        /// Gets the message from XML response if possible.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.String.</returns>
        private static async Task<string> GetMessageFromResponseContent(HttpContent content)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(await content.ReadAsStreamAsync());
                var node = doc.SelectSingleNode("/response/message");
                return node?.InnerText;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Requests the token asynchronously.
        /// </summary>
        /// <param name="tokenTyp">The token typ.</param>
        /// <param name="baseEndpointAddress">The base endpoint address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> ITokenRequest.RequestTokenAsync(TokenType tokenTyp, string baseEndpointAddress, string userAgent, string userName, string password)
        {
            return RequestTokenAsync(tokenTyp, baseEndpointAddress, userAgent, userName, password);
        }

        /// <summary>
        /// Requests the token asynchronously with HTTP authentication.
        /// </summary>
        /// <param name="tokenTyp">The token typ.</param>
        /// <param name="baseEndpointAddress">The base endpoint address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="httpAuthUser">The HTTP authentication user.</param>
        /// <param name="httpAuthPassword">The HTTP authentication password.</param>
        /// <returns>Task.</returns>
        Task<string> ITokenRequest.RequestTokenAsync(TokenType tokenTyp, string baseEndpointAddress, string userAgent, string userName, string password, string httpAuthUser, string httpAuthPassword)
        {
            return RequestTokenAsync(tokenTyp, baseEndpointAddress, userAgent, userName, password, httpAuthUser, httpAuthPassword);
        }
    }
}
