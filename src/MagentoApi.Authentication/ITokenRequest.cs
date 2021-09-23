using System.Threading.Tasks;

namespace Compori.MagentoApi.Authentication
{
    public interface ITokenRequest
    {
        /// <summary>
        /// Requests the token asynchronously.
        /// </summary>
        /// <param name="tokenTyp">The token typ.</param>
        /// <param name="baseEndpointAddress">The base endpoint address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> RequestTokenAsync(TokenType tokenTyp, string baseEndpointAddress, string userAgent, string userName, string password);

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
        Task<string> RequestTokenAsync(TokenType tokenTyp, string baseEndpointAddress, string userAgent, string userName, string password, string httpAuthUser, string httpAuthPassword);
    }
}
