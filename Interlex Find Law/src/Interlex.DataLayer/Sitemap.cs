using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace Interlex.DataLayer
{
    public partial class DB
    {
        public static IEnumerable<IDataRecord> GetDocLinksSitemap()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_links_sitemap", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }
    }
}
