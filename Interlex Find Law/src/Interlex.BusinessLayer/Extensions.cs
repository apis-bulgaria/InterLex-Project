using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.BusinessLayer.Entities;
using Interlex.BusinessLayer.Models;

namespace Interlex.BusinessLayer
{
    public static class Extensions
    {
        public static string ToJson(this Dictionary<string, int> dictionary)
        {
            var kvs = dictionary.Select(kvp => string.Format("{{\"i\":\"{0}\", \"c\":{1}}}", kvp.Key, kvp.Value));
            return string.Concat("[", string.Join(",", kvs), "]");
        }

        //public static List<ClassifierItem> ToClassifierItemList(this Dictionary<string, int> dictionary)
        //{
        //    var kvs = dictionary.Select(kvp => new ClassifierItem() { Id = kvp.Key});
        //    return string.Concat("[", string.Join(",", kvs), "]");
        //}

        public static DocContentItem ToDocContentItem(this IDataRecord record)
        {
            if (record == null)
                throw new ArgumentNullException();

            var result = new DocContentItem
            {
                id = Convert.ToInt32(record["doc_par_id"]),
                parent_id = record["parent_doc_par_id"] as int?,
                eid = record["eid"].ToString(),
                title = $"{record["num"]} {record["heading"]}".Trim(),
                tooltip = $"{record["num"]} {record["heading"]}".Trim()
            };

            return result;
        }
    }
}
