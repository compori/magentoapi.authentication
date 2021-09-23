using System;

namespace Compori.MagentoApi.Authentication
{
    /// <summary>
    /// The class SystemDateTime implement the <see cref="ISystemDateTime"/> Interface.
    /// </summary>
    /// <seealso cref="ISystemDateTime" />
    public class SystemDateTime : ISystemDateTime
    {
        /// <summary>
        /// Returns the system's UTCs of now.
        /// </summary>
        /// <value>The UTC now.</value>
        DateTime ISystemDateTime.UtcNow => DateTime.UtcNow;
    }
}
