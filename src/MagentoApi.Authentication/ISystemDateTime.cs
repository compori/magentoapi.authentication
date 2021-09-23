using System;

namespace Compori.MagentoApi.Authentication
{
    /// <summary>
    /// Interface ISystemDateTime is used to retrieve the current system time without a specific implementation.
    /// </summary>
    public interface ISystemDateTime
    {
        /// <summary>
        /// Returns the system's UTCs of now.
        /// </summary>
        /// <returns>DateTime.</returns>
        DateTime UtcNow { get; }
    }
}
