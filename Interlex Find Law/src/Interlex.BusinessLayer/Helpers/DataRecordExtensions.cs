namespace Interlex.BusinessLayer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public static class DataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        public static IEnumerable<string> GetColumnNames(this IDataRecord dr)
        {
            List<string> columnNames = new List<string>();

            for (int i = 0; i < dr.FieldCount; i++)
            {
                columnNames.Add(dr.GetName(i));
            }

            return columnNames;
        }
    }
}
