using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestToken
{
        /// <summary>
        /// Class ListOptions.
        /// </summary>
        [Verb("request-token", HelpText = "Request an admin or customer token.")]
        public class RequestTokenOption
        {
            /// <summary>
            /// Gets or sets the name of the admin token request user.
            /// </summary>
            /// <value>The name of the admin token request user.</value>
            [Option(shortName: 't', longName: "token-type", HelpText = "The token type (admin or customer.", Required = true)]
            public string TokenType { get; set; }

            /// <summary>
            /// Gets or sets the base endpoint address.
            /// </summary>
            /// <value>The base endpoint address.</value>
            [Option(shortName: 'a', longName:"base-endpoint-address", HelpText = "Endpoint address to shop", Required = true)]
            public string BaseEndpointAddress { get; set; }

            /// <summary>
            /// Gets or sets the user agent.
            /// </summary>
            /// <value>The user agent.</value>
            [Option(longName: "user-agent", HelpText = "User agent.")]
            public string UserAgent { get; set; }

            /// <summary>
            /// Gets or sets the name of the admin token request user.
            /// </summary>
            /// <value>The name of the admin token request user.</value>
            [Option(shortName: 'u', longName: "admin-token-request-user", HelpText = "Username to generate access token.", Required = true)]
            public string TokenRequestUserName { get; set; }

            /// <summary>
            /// Gets or sets the admin token request password.
            /// </summary>
            /// <value>The admin token request password.</value>
            [Option(shortName: 'p', longName: "admin-token-request-password", HelpText = "User password to generate access token.", Required = true)]
            public string TokenRequestPassword { get; set; }

            /// <summary>
            /// Gets or sets the HTTP authentication user.
            /// </summary>
            /// <value>The HTTP authentication user.</value>
            [Option(longName: "http-auth-user", HelpText = "Http authentification user name.")]
            public string HttpAuthUser { get; set; }

            /// <summary>
            /// Gets or sets the HTTP authentication password.
            /// </summary>
            /// <value>The HTTP authentication password.</value>
            [Option(longName: "http-auth-password", HelpText = "Http authentification user name.")]
            public string HttpAuthPassword { get; set; }
        }
}
