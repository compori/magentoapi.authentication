using System;

namespace Compori.MagentoApi.Authentication
{
    public class TokenSettings
    {
        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public TokenType TokenType { get ; set; }

        /// <summary>
        /// Gets or sets the access token if <see cref="TokenType"/> is set to <see cref="TokenType.Integration"/>.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the individual expiration period for a token.
        /// </summary>
        /// <value>The individual expiration period for a token.</value>
        public TimeSpan? ExpirationPeriod { get; set; }

        /// <summary>
        /// Gets or sets the base endpoint address.
        /// </summary>
        /// <value>The base endpoint address.</value>
        public string BaseEndpointAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent name.
        /// </summary>
        /// <value>The user agent name.</value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the name of the request user.
        /// </summary>
        /// <value>The name of the request user.</value>
        public string RequestUserName { get; set; }

        /// <summary>
        /// Gets or sets the request password.
        /// </summary>
        /// <value>The request password.</value>
        public string RequestPassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the HTTP basic authentication user.
        /// </summary>
        /// <value>The name of the HTTP basic authentication user.</value>
        public string HttpAuthenticationUser { get; set; }

        /// <summary>
        /// Gets or sets the HTTP basic authentication password.
        /// </summary>
        /// <value>The HTTP basic authentication password.</value>
        public string HttpAuthenticationPassword { get; set; }
    }
}
