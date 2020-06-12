namespace Interlex.DataLayer.Models.DataReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Apis.Common.Reflection.Extension;
    using Npgsql;

    internal static class ExecuteReader
    {
        internal static IEnumerable<IDataRecord> Yield<T>(String connectionString, String functionName, T parameterBag)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var function = new NpgsqlCommand(functionName, connection);
                function.CommandType = CommandType.StoredProcedure;

                var parameters = parameterBag.GetPublicProperties();
                foreach (var parameter in parameters)
                {
                    function.Parameters.Add(parameter.Key, parameter.Value);
                }

                using (var reader = function.ExecuteReader())
                {
                    while (reader.Read())
                        yield return reader;
                }
            }
        }
    }
}
