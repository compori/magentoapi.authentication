namespace Compori.MagentoApi.Authentication
{
    public enum TokenType
    {
        /// <summary>
        /// Default value is undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// An integration token
        /// </summary>
        Integration = 1,

        /// <summary>
        /// An admin token
        /// </summary>
        Admin = 2,

        /// <summary>
        /// A customer token
        /// </summary>
        Customer = 3
    }
}
