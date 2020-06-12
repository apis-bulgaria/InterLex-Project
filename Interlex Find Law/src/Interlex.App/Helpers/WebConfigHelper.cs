namespace Interlex.App.Helpers
{
    using System;
    using System.Configuration;
    using Apis.Common.Extensions;

    public static class WebConfigHelper
    {
        /// <summary>
        /// Determs if the current application is in test mode. Test mode is: Debug or connection string to any test database
        /// </summary>
        /// <returns></returns>
        public static bool IsTestEnvironment()
        {
#if DEBUG
            return true;
#else
            var isTestDatabase = ConfigurationManager.ConnectionStrings["ConnPG"]
                .DatabaseName()
                .StartsWithAny(StringComparison.OrdinalIgnoreCase, "webtest", "eurocases_test");

            return isTestDatabase;
#endif
        }

    }
}