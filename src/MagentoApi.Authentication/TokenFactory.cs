namespace Compori.MagentoApi.Authentication
{
    public class TokenFactory
    {
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
        /// Initializes a new instance of the <see cref="TokenFactory"/> class.
        /// </summary>
        /// <param name="tokenRequest">The token request.</param>
        /// <param name="systemDateTime">The system date time.</param>
        public TokenFactory(ITokenRequest tokenRequest, ISystemDateTime systemDateTime)
        {
            this.TokenRequest = tokenRequest;
            this.SystemDateTime = systemDateTime;
        }

        /// <summary>
        /// Creates the a admin token object by specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Compori.MagentoApi.Authentication.IToken.</returns>
        public IToken Create(TokenSettings settings)
        {
            var token = new Token(this.TokenRequest, this.SystemDateTime);
            token.Configure(settings);
            return token;
        }
    }
}
