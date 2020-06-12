using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EuFins.DataReader
{
    public class OptionReader
    {
        public List<string> GetDistinctCurrencies()
        {
            string query = " SELECT DISTINCT cn FROM currencies ORDER BY cn ; ";
            var currencies = new List<string>();
            string conStr = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(conStr))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            currencies.Add((string)dr["cn"]);
                        }
                    }
                }
            }

            return currencies;
        }

        public List<string> GetDistinctLibors()
        {
            string query = " SELECT DISTINCT n FROM libor ORDER BY n ; ";
            var currencies = new List<string>();
            string conStr = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(conStr))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            currencies.Add((string)dr["n"]);
                        }
                    }
                }
            }

            return currencies;
        }

        public List<string> GetDistinctSofi()
        {
            string query = " SELECT DISTINCT n FROM lsbb ORDER BY n ; ";
            var currencies = new List<string>();
            string conStr = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(conStr))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            currencies.Add((string)dr["n"]);
                        }
                    }
                }
            }

            return currencies;
        }

        public List<string> GetDistinctStockIndexes()
        {
            string query = " SELECT DISTINCT n FROM indices ORDER BY n ; ";
            var currencies = new List<string>();
            string conStr = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(conStr))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            currencies.Add((string)dr["n"]);
                        }
                    }
                }
            }

            return currencies;
        }
    }
}