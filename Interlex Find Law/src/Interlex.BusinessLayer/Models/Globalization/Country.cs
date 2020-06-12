namespace Interlex.BusinessLayer.Models
{
    using System;
    using Interlex.DataLayer;

    public class Country
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public bool IsEu { get; set; }

        public int? VatId { get; set; }

        public string Name { get; set; }

        public Country(int id, string code, bool isEu, int? VatId, string name)
        {
            this.Id = id;
            this.Code = code;
            this.IsEu = IsEu;
            this.VatId = VatId;
            this.Name = name;
        }

        public Country() { }

        public static Country GetCountry(int id)
        {
            Country country = null;
            var row = DB.GetCountry(id);
            if (row != null)
            {
                country = new Country(){
                    Id = Convert.ToInt32(row["id"]),
                    Code = row["code"].ToString(),
                    IsEu = Convert.ToBoolean(row["eu"]),
                    VatId = Convert.ToInt32(row["vat_id"])
                };
            }
            return country;
        }

        public static string GetCountryNameByIdAndLang(int countryId, int langId)
        {
            return DB.GetCountryNameByIdAndLang(countryId, langId);
        }
    }
}
