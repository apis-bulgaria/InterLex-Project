using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.DataLayer
{
    public partial class DB
    {
        public static DateTime GetFinsEurostatLastExtraction()
        {
            var result = DateTime.Now;
            string query = " SELECT last_change_date FROM fins.eurostat_update WHERE file_name = 'all' LIMIT 1 ; ";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result = (DateTime)dr["last_change_date"];
                        }
                    }
                }
            }

            return result;
        }

        public static IEnumerable<IDataRecord> GetFinsEuroLibor(string liborFor, DateTime dateFrom, string liborType, string orderDir)
        {
            string query = " SELECT DISTINCT m, " + liborFor + " FROM libor WHERE m >= '" + dateFrom.ToString("yyyy.MM.dd") + "' AND m < '" + dateFrom.AddMonths(1).ToString("yyyy.MM.dd") + "' AND n = '" + liborType + "' ORDER BY m " + orderDir + " ; ";

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    //command.Parameters.Add(new NpgsqlParameter("liborFor", NpgsqlDbType.Varchar) { Value = liborFor });
                    //command.Parameters.Add(new NpgsqlParameter("dateFrom", NpgsqlDbType.Varchar) { Value = dateFrom.ToString("yyyy.MM.dd") });
                    //command.Parameters.Add(new NpgsqlParameter("dateTo", NpgsqlDbType.Varchar) { Value = dateFrom.AddMonths(1).ToString("yyyy.MM.dd") });
                    //command.Parameters.Add(new NpgsqlParameter("liborType", NpgsqlDbType.Varchar) { Value = liborType });
                    //command.Parameters.Add(new NpgsqlParameter("orderDir", NpgsqlDbType.Varchar) { Value = orderDir });

                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetFinsEuribor(string liborFor, int year, string orderDir)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            string query = " SELECT DISTINCT m, " + liborFor + " FROM libor WHERE m >= '" + startDate.ToString("yyyy.MM.dd") + "' AND m < '" + endDate.ToString("yyyy.MM.dd") + "' AND n = 'EuRibor1' ORDER BY m " + orderDir + " ; ";

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    //command.Parameters.Add(new NpgsqlParameter("liborFor", NpgsqlDbType.Varchar) { Value = liborFor });
                    //command.Parameters.Add(new NpgsqlParameter("dateFrom", NpgsqlDbType.Varchar) { Value = dateFrom.ToString("yyyy.MM.dd") });
                    //command.Parameters.Add(new NpgsqlParameter("dateTo", NpgsqlDbType.Varchar) { Value = dateFrom.AddMonths(1).ToString("yyyy.MM.dd") });
                    //command.Parameters.Add(new NpgsqlParameter("liborType", NpgsqlDbType.Varchar) { Value = liborType });
                    //command.Parameters.Add(new NpgsqlParameter("orderDir", NpgsqlDbType.Varchar) { Value = orderDir });

                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static DateTime GetFinsEuroLiborLastDate(string liborType)
        {
            var result = DateTime.Now;
            string query = " SELECT m FROM libor WHERE n = '" + liborType + "' ORDER BY m DESC LIMIT 1 ; ";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result = (DateTime)dr["m"];
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, DateTime> GetFinsEuroLiborLastDate()
        {
            var result = new Dictionary<string, DateTime>();
            string query = " SELECT DISTINCT n, max(m) OVER (PARTITION BY n) maxm FROM libor ORDER BY n ; ";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var libType = (string)dr["n"];
                            var libDate = (DateTime)dr["maxm"];
                            if (!result.ContainsKey(libType))
                            {
                                result.Add(libType, libDate);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static IEnumerable<IDataRecord> GetFinsCurrency(string currentCurrency, DateTime dateFrom, string orderDir, string currType)
        {
            string query = " SELECT * FROM " + currType + " WHERE cn = '" + currentCurrency + "' AND m >= '" + dateFrom.ToString("yyyy.MM.dd") + "' AND m < '" + dateFrom.AddMonths(1).ToString("yyyy.MM.dd") + "' ORDER BY m " + orderDir + ";";

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    /* command.CommandType = System.Data.CommandType.StoredProcedure;
                     command.Parameters.Add(new NpgsqlParameter("currentCurrency", NpgsqlDbType.Varchar) { Value = currentCurrency });
                     command.Parameters.Add(new NpgsqlParameter("dateFrom", NpgsqlDbType.Varchar) { Value = dateFrom.ToString("yyyy.MM.dd") });
                     command.Parameters.Add(new NpgsqlParameter("dateTo", NpgsqlDbType.Varchar) { Value = dateFrom.AddMonths(1).ToString("yyyy.MM.dd") });
                     command.Parameters.Add(new NpgsqlParameter("orderDir", NpgsqlDbType.Varchar) { Value = orderDir });*/

                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static DateTime GetFinsCurrencyLastDate(string currentCurrency, string currType)
        {
            var result = DateTime.Now;
            string query = " SELECT m FROM " + currType + " WHERE cn = '" + currentCurrency + "' ORDER BY m DESC LIMIT 1 ;";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result = (DateTime)dr["m"];
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, DateTime> GetFinsCurrencyLastDate(string currentTypeIn)
        {
            var result = new Dictionary<string, DateTime>();
            string query = " SELECT DISTINCT cn, max(m) OVER (PARTITION BY cn) maxm FROM " + currentTypeIn + " ORDER BY cn; ";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var currType = (string)dr["cn"];
                            var currDate = (DateTime)dr["maxm"];
                            if (!result.ContainsKey(currType))
                            {
                                result.Add(currType, currDate);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static IEnumerable<IDataRecord> GetFinsStockIndex(string indexName, DateTime dateFrom, string orderDir)
        {
            string query = " SELECT m, v FROM indices WHERE n = '" + indexName + "' AND m >= '" + dateFrom.ToString("yyyy.MM.dd") + "' AND m < '" + dateFrom.AddMonths(1).ToString("yyyy.MM.dd") + "' ORDER BY m " + orderDir + ";";

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    /*command.Parameters.Add(new NpgsqlParameter("indexName", NpgsqlDbType.Varchar) { Value = indexName });
                    command.Parameters.Add(new NpgsqlParameter("dateFrom", NpgsqlDbType.Varchar) { Value = dateFrom.ToString("yyyy.MM.dd") });
                    command.Parameters.Add(new NpgsqlParameter("dateTo", NpgsqlDbType.Varchar) { Value = dateFrom.AddMonths(1).ToString("yyyy.MM.dd") });
                    command.Parameters.Add(new NpgsqlParameter("orderDir", NpgsqlDbType.Varchar) { Value = orderDir });*/

                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static DateTime GetFinsStockIndexLastDate(string indexName)
        {
            var result = DateTime.Now;
            string query = " SELECT m FROM indices WHERE n = '" + indexName + "' ORDER BY m DESC LIMIT 1 ;";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result = (DateTime)dr["m"];
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, DateTime> GetFinsStockIndexLastDate()
        {
            var result = new Dictionary<string, DateTime>();
            string query = " SELECT DISTINCT n, max(m) OVER (PARTITION BY n) maxm FROM indices ORDER BY n ;";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var indType = (string)dr["n"];
                            var indDate = (DateTime)dr["maxm"];
                            if (!result.ContainsKey(indType))
                            {
                                result.Add(indType, indDate);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static IEnumerable<IDataRecord> GetFinsEuroLiborTypes()
        {
            string query = " SELECT DISTINCT n FROM libor ORDER BY n ; ";
            var currencies = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetFinsCurrencyTypes(string currType)
        {
            string query = " SELECT DISTINCT cn, n FROM " + currType + " ORDER BY cn ; ";
            var currencies = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetFinsStockIndexTypes()
        {
            string query = " SELECT DISTINCT n FROM indices ORDER BY n ; ";
            var currencies = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetFinsEurostat(string statisticType, string tableType, string tableTypeAffix, DateTime dateFrom, DateTime dateTo, int langId)
        {
            string query = $" SELECT get_country_fins(geo_unit, {langId}) country, * FROM fins.eurostat WHERE statistic_type = '" + statisticType + "' ";
            if (!string.IsNullOrEmpty(tableType))
            {
                query += " AND table_type = '" + tableType + "' ";
            }

            if (!string.IsNullOrEmpty(tableTypeAffix))
            {
                query += " AND table_type_affix = '" + tableTypeAffix + "' ";
            }

            query += " ; ";

            var currencies = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return dr;
                        }
                    }
                }
            }
        }

        // "e163a52c-49a7-42c4-a741-744d927b13e7"
        public static string GetFinsStaticHtml(Guid menuId, int langId)
        {
            string result = string.Empty;
            string query = $" SELECT htm_content FROM fins.html_item_translations WHERE html_menu_id = '{menuId}' and lang_id = {langId} ";
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var content = dr["htm_content"] as byte[];
                            if (content != null)
                            {
                                result = Encoding.UTF8.GetString(content);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
