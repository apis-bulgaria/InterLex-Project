namespace Interlex.BusinessLayer
{
    using System.Collections.Generic;
    using System.Data;
    using Interlex.DataLayer;
    using Interlex.BusinessLayer.Helpers;
    using Interlex.BusinessLayer.Models;
    using Interlex.DataLayer.Models;

    using System.Text;
    using System;
    public class DBUtil
    {
        /// <summary>
        /// Gets latest DB version
        /// </summary> 
        /// <returns>Version string</returns>
        public static string GetDBMaxVersion()
        {
            return DB.GetMaxDBVersion();
        }

        /// <summary>
        /// Constructs csv string based on DataRecord set
        /// </summary>
        /// <param name="records">Records from DB</param>
        public static string ConstructCSVStringFromData(IEnumerable<IDataRecord> records)
        {
            StringBuilder builder = new StringBuilder();
            bool headersPopulated = false;

            // No data returned
            if (records == null)
            {
                return String.Empty;
            }

            foreach (var r in records)
            {
                // populating headers
                if (!headersPopulated)
                {
                    var columnNames = r.GetColumnNames() as List<string>;
                    columnNames.Remove("total_query_count"); // excluding total query count from headers
                    builder.Append(String.Join(",", columnNames));
                    builder.Append(Environment.NewLine);

                    headersPopulated = true;
                }

                // populating column data
                for (int i = 0; i < r.FieldCount; i++)
                {
                    if (r.GetName(i) == "total_query_count") // excluding total query data from columns
                    {
                        continue;
                    }

                    builder.Append("\"" + r[i] + "\"");
                    builder.Append(",");
                }

                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        public static void InsertLawRelations(Interlex.BusinessLayer.Models.LawRelationUpdateModel[] updateModelList)
        {
            var dataLayerUpdateModelList = new List<Interlex.DataLayer.Models.LawRelationUpdateModel>();

            foreach (var updateModel in updateModelList)
            {
                var dataLayerModel = new Interlex.DataLayer.Models.LawRelationUpdateModel
                {
                    FromCelex = updateModel.FromCelex,
                    FromArticle = updateModel.FromArticle,
                    ToCelex = updateModel.ToCelex,
                    ToArticle = updateModel.ToArticle,
                    LinkIds = updateModel.LinkIds,
                    ToDocParId = updateModel.ToDocParId
                };

                dataLayerUpdateModelList.Add(dataLayerModel);
            }
            
            DB.InsertLawRelations(dataLayerUpdateModelList.ToArray());
        }


        public static void ImportLawRelationsFromCSV(string path)
        {
            DB.ManipulateLawRelationsWithCSV(path, false);
        }

        public static void DeleteLawRelationsFromCSV(string path)
        {
            DB.ManipulateLawRelationsWithCSV(path, true);
        }
    }
}
