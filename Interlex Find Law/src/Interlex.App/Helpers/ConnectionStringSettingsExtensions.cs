using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Interlex.App.Helpers
{
    public static class ConnectionStringSettingsExtensions
    {
        public static String DatabaseName(this ConnectionStringSettings settings)
        {
            // Server=****;Port=******;Database=*******;User Id=******;Password=*******;Pooling=false;

            String[] parts = settings.ConnectionString.Split(';');

            String databaseName = parts
                .Where(x => x.StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Replace("Database=", String.Empty))
                .FirstOrDefault()
                ?? String.Empty;

            return databaseName;
        }
    }
}