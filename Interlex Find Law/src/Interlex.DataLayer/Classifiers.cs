using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Npgsql;

namespace Interlex.DataLayer
{
    public partial class DB
    {
        public static IEnumerable<IDataRecord> GetClassifier(int classifierType, Guid? parentId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_classifier_type_id", classifierType);
                comm.Parameters.AddWithValue("_parent_id", parentId);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int GetClassifierTypeIdByName(string name)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_type_id_by_name", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_name", name);
                object result = comm.ExecuteScalar();
                /* if (result == DBNull.Value)
                     return null;
                 else*/
                return Convert.ToInt32(result);
            }
        }

        public static string GetClassifierId_KeyPath(Guid id, bool skipRoot)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_keypathlist", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);
                comm.Parameters.AddWithValue("_skip_root", skipRoot);
                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetClassifierId_TitlePath(Guid id, int langId, bool skipRoot)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_keypathlist_title", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);
                comm.Parameters.AddWithValue("_lang", langId);
                comm.Parameters.AddWithValue("_skip_root", skipRoot);
                return comm.ExecuteScalar().ToString();
            }
        }

        public static IEnumerable<IDataRecord> GetClassifierById(string id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_by_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }


        public static IEnumerable<IDataRecord> GetClassifiersByParentId(string parentId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_by_parent_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_parent_id", parentId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetClassifierLangs(string id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_langs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static string GetClassifierBaseByTypeId(int classifierTypeId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_base_id_by_type_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_classifier_type_id", classifierTypeId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static IEnumerable<IDataRecord> GetAllClassifiers()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_all_classifiers", conn);
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

        public static IEnumerable<IDataRecord> GetAllClassifiersAndLangs()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_all_classifiers_and_langs", conn);
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

        public static int GetClassifiersDeepestLevel()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_deepest_level", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                var res = comm.ExecuteScalar().ToString();
                if (String.IsNullOrEmpty(res))
                {
                    return 0;
                }

                return int.Parse(res);
            }
        }

        public static IEnumerable<IDataRecord> GetClassifiersByTreeLevel(int treeLevel)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_by_tree_level_alternative", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_tree_level", treeLevel);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetClassifiersMappings()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_mappings", conn);
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

        public static int GetClassifiersTopLevelCount()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_top_level_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static string GetClassifierIdByTitle(string title)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_id_by_title", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_title", title);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetClassifierIdByXmlId(string xmlId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_id_by_xml_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_xml_id", xmlId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetClassifierNameByXmlIdLangId(string xmlId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_name_by_xml_id_lang_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_xml_id", xmlId);
                comm.Parameters.AddWithValue("_lang_id", langId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string[] GetClassifierChildrenIds(Guid id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifier_children_ids", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_classifier_id", id);

                return (string[])comm.ExecuteScalar();
            }
        }

        public static IEnumerable<IDataRecord> GetClassifiersMap()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_map", conn);
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
