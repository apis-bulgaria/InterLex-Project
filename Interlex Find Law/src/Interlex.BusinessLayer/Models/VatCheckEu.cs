namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using Interlex.DataLayer;

    public class VatCheckEu
    {
        public string VatNumber { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public DateTime RequestDate { get; set; }

        public int ErrorTypeId { get; set; }

        public string Name { get; set; }

        public string RequestId { get; set; }

        public VatCheckEu(IDataRecord row)
        {
            this.VatNumber = row["vat_number"].ToString();
            this.Address = row["address"].ToString();
            this.Country = row["country"].ToString();
            this.RequestDate = Convert.ToDateTime(row["request_date"]);
            this.ErrorTypeId = Convert.ToInt32(row["error_type"]);
            this.Name = row["name"].ToString();
            if (row["request_id"] != DBNull.Value && !(String.IsNullOrEmpty(row["request_id"].ToString())))
            {
                this.RequestId = row["request_id"].ToString();
            }
        }

        public VatCheckEu() { }

        public static VatCheckEu GetVatCheckEu(int orderId)
        {
            foreach (var item in DB.GetOrderVatCheckEu(orderId))
            {
                return new VatCheckEu(item); //will stop after first iteration
            }

            return new VatCheckEu();
        }
    }
}
