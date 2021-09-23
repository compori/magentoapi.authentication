using System;
using System.Threading.Tasks;

namespace Compori.MagentoApi.Authentication
{
    public interface IToken
    {
        /// <summary>
        /// Returns true if the token is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool IsValid { get; }

        /// <summary>
        /// Gets the expires datetime for the current token.
        /// </summary>
        /// <value>The valid to.</value>
        DateTime? Expires { get; }

        /// <summary>
        /// Gets a valid token or creates a new one, if current is invalid asynchrounly.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> CreateAsync();
    }
}
