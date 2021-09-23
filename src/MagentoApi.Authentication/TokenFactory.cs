using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compori.MagentoApi.Authentication
{
    public class TokenFactory
    {
        /// <summary>
        /// The token request
        /// </summary>
        private ITokenRequest tokenRequest { get; }

        /// <summary>
        /// The system date time
        /// </summary>
        private ISystemDateTime systemDateTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenFactory"/> class.
        /// </summary>
        /// <param name="tokenRequest">The token request.</param>
        /// <param name="systemDateTime">The system date time.</param>
        public TokenFactory(ITokenRequest tokenRequest, ISystemDateTime systemDateTime)
        {
            this.tokenRequest = tokenRequest;
            this.systemDateTime = systemDateTime;
        }

        /// <summary>
        /// Creates the a admin token object by specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Compori.MagentoApi.Authentication.IToken.</returns>
        public IToken Create(TokenSettings settings)
        {
            var token = new Token(this.tokenRequest, this.systemDateTime);
            token.Configure(settings);
            return token;
        }
    }
}
