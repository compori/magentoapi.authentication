using CommandLine;
using Compori.MagentoApi.Authentication;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestToken
{
    class Program
    {
        /// <summary>
        /// Requests the token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Int32.</returns>
        public static async Task<int> RequestToken(CancellationToken cancellationToken, RequestTokenOption options)
        {
            TokenType tokenType;
            
            switch(options.TokenType?.ToUpperInvariant())
            {
                case "ADMIN":
                    tokenType = TokenType.Admin;
                    break;
                case "CUSTOMER":
                    tokenType = TokenType.Customer;
                    break;
                default:
                    Console.WriteLine("Unkown token type was given.");
                    return -1;
            }

            try
            {
                var settings = new TokenSettings
                {
                    TokenType = tokenType,
                    BaseEndpointAddress = options.BaseEndpointAddress,
                    RequestUserName = options.TokenRequestUserName,
                    RequestPassword = options.TokenRequestPassword,
                    HttpAuthenticationUser = options.HttpAuthUser,
                    HttpAuthenticationPassword = options.HttpAuthPassword,
                    UserAgent = options.UserAgent
                };

                var token = new TokenFactory(new TokenRequest(), new SystemDateTime()).Create(settings);

                Console.WriteLine("Token Typ    : <" + settings.TokenType + ">");
                Console.WriteLine("Username     : " + settings.RequestUserName);
                Console.WriteLine("Access Token : " + await token.CreateAsync());
                Console.WriteLine("Is valid     : " + token.IsValid);
                Console.WriteLine("Expires      : " + token.Expires.Value.ToLocalTime().ToString("G"));

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Request a token.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>System.Int32.</returns>
        public static int RequestToken(RequestTokenOption options)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            return RequestToken(token, options).GetAwaiter().GetResult();
        }

        #region Entry Point

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<RequestTokenOption>(args).MapResult(
                (RequestTokenOption opts) => RequestToken(opts),
                errs => 1);
            return result;
        }

        #endregion
    }
}
