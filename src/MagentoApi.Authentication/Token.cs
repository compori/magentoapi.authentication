using System;
using System.Threading.Tasks;

namespace Compori.MagentoApi.Authentication
{
    public class Token : IToken
    {
        /// <summary>
        /// Gets the default expiration period for admin token.
        /// </summary>
        /// <value>The default expiration period for admin token.</value>
        public static TimeSpan DefaultAdminTokenExpirationPeriod => new TimeSpan(4, 0, 0);

        /// <summary>
        /// Gets the default customer token expiration period.
        /// </summary>
        /// <value>The default customer token expiration period.</value>
        public static TimeSpan DefaultCustomerTokenExpirationPeriod => new TimeSpan(1, 0, 0);

        /// <summary>
        /// The expires datetime
        /// </summary>
        private DateTime? _expires;

        /// <summary>
        /// The current admin token
        /// </summary>
        private Task<string> _token;

        /// <summary>
        /// The is configured
        /// </summary>
        private bool _isConfigured;

        /// <summary>
        /// Gets the token request.
        /// </summary>
        /// <value>The token request.</value>
        private ITokenRequest TokenRequest { get; }

        /// <summary>
        /// Gets the system date time.
        /// </summary>
        /// <value>The system date time.</value>
        private ISystemDateTime SystemDateTime { get; }

        /// <summary>
        /// Gets or sets the expiration period.
        /// </summary>
        /// <value>The expiration period.</value>
        public TimeSpan? ExpirationPeriod { get; private set; }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public TokenType TokenType { get; private set; }

        /// <summary>
        /// Gets or sets the base endpoint address.
        /// </summary>
        /// <value>The base endpoint address.</value>
        public string BaseEndpointAddress { get; private set; }

        /// <summary>
        /// Gets or sets the name of the request user.
        /// </summary>
        /// <value>The name of the request user.</value>
        public string RequestUserName { get; private set; }

        /// <summary>
        /// Gets or sets the request password.
        /// </summary>
        /// <value>The request password.</value>
        public string RequestPassword { get; private set; }

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; private set; }

        /// <summary>
        /// Gets or sets the name of the HTTP basic authentication user.
        /// </summary>
        /// <value>The name of the HTTP basic authentication user.</value>
        public string HttpAuthenticationUser { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP basic authentication password.
        /// </summary>
        /// <value>The HTTP basic authentication password.</value>
        public string HttpAuthenticationPassword { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="tokenRequest">The token request.</param>
        /// <param name="systemDateTime">The system date time.</param>
        public Token(ITokenRequest tokenRequest, ISystemDateTime systemDateTime)
        {
            this.TokenRequest = tokenRequest;
            this.SystemDateTime = systemDateTime;
        }

        /// <summary>
        /// Configures the admin token objekt.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Token.</returns>
        /// <exception cref="System.InvalidOperationException">Token is already configured.</exception>
        /// <exception cref="System.ArgumentNullException">settings</exception>
        /// <exception cref="System.ArgumentException">No access token provided for integration token type. - settings</exception>
        /// <exception cref="System.ArgumentException">Token type {settings.TokenType} not supported. - settings</exception>
        /// <exception cref="System.ArgumentException">The base endpoint addresss in settings is not set. - settings</exception>
        /// <exception cref="System.ArgumentException">The username for requesting token in settings is not set. - settings</exception>
        /// <exception cref="System.ArgumentException">The user password for requesting tokens in settings is not set. - settings</exception>
        /// <exception cref="ArgumentNullException">settings</exception>
        public Token Configure(TokenSettings settings)
        {
            if(this._isConfigured)
            {
                throw new InvalidOperationException("Token is already configured.");
            }
            
            if(settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this._token = null;
            this._expires = null;

            //
            // Copy settings
            //
            switch(settings.TokenType)
            {
                case TokenType.Integration:
                    
                    //
                    // Renewal of integration tokens is indefinite and token string is predefined.
                    // They do not have to renewed after a period of time.
                    //
                    if(string.IsNullOrWhiteSpace(settings.AccessToken))
                    {
                        throw new ArgumentException("No access token provided for integration token type.", nameof(settings));
                    }
                    this._token = Task.Run( () => settings.AccessToken );
                    break;

                case TokenType.Admin:
                    this.ExpirationPeriod = settings.ExpirationPeriod ?? DefaultAdminTokenExpirationPeriod;
                    break;
                case TokenType.Customer:
                    this.ExpirationPeriod = settings.ExpirationPeriod ?? DefaultCustomerTokenExpirationPeriod;
                    break;
                default:
                    throw new ArgumentException($"Token type {settings.TokenType} not supported.", nameof(settings));
            }
            
            this.TokenType = settings.TokenType;

            //
            // Customer or admin token must be requested via username and password
            //
            if(settings.TokenType == TokenType.Admin || settings.TokenType == TokenType.Customer)
            {
                if(string.IsNullOrWhiteSpace(settings.BaseEndpointAddress))
                {
                    throw new ArgumentException("The base endpoint addresss in settings is not set.", nameof(settings));
                }
                if(string.IsNullOrWhiteSpace(settings.RequestUserName))
                {
                    throw new ArgumentException("The username for requesting token in settings is not set.", nameof(settings));
                }
                if(settings.RequestPassword == null)
                {
                    throw new ArgumentException("The user password for requesting tokens in settings is not set.", nameof(settings));
                }

                this.BaseEndpointAddress = settings.BaseEndpointAddress;
                this.RequestUserName = settings.RequestUserName;
                this.RequestPassword = settings.RequestPassword;
                this.UserAgent = settings.UserAgent;
                this.HttpAuthenticationUser = settings.HttpAuthenticationUser;
                this.HttpAuthenticationPassword = settings.HttpAuthenticationPassword;
            }

            this._isConfigured = true;

            return this;
        }

        /// <summary>
        /// Gets a valid token or creates a new one, if current is invalid asynchrounly.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public Task<string> CreateAsync()
        {
            if(!this._isConfigured)
            {
                throw new InvalidOperationException("The token is not configured.");
            }

            if(this.TokenType == TokenType.Integration)
            {
                return this._token;
            }

            if (!this.IsValid)
            {
                var utcNow = this.SystemDateTime.UtcNow;

                this._token = this.TokenRequest.RequestTokenAsync(
                    this.TokenType,
                    this.BaseEndpointAddress, 
                    this.UserAgent,
                    this.RequestUserName, 
                    this.RequestPassword, 
                    this.HttpAuthenticationUser, 
                    this.HttpAuthenticationPassword);

                this._expires = utcNow.Add(this.ExpirationPeriod.Value);
            }

            return this._token;
        }

        /// <summary>
        /// Returns true if the token is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid 
        {
            get
            {
                if(!this._isConfigured)
                {
                    throw new InvalidOperationException("The token is not configured.");
                }

                switch (this.TokenType)
                {
                    case TokenType.Integration:
                        // Token is valid if token is set
                        return this._token != null;

                    case TokenType.Admin:
                    case TokenType.Customer:

                        // Token is valid if token is set and not expired.
                        return this._token != null
                            && this._expires.HasValue
                            && this.SystemDateTime.UtcNow < this._expires.Value;
                    case TokenType.Undefined:
                    default:
                        throw new InvalidOperationException("Token type is not supported.");
                }
            }
        }

        /// <summary>
        /// Gets the expires datetime for the current token.
        /// </summary>
        /// <value>The valid to.</value>
        public DateTime? Expires 
        {
            get
            {
                if(!this._isConfigured)
                {
                    throw new InvalidOperationException("The token is not configured.");
                }

                return  this._expires;
            }
        }
    }
}
