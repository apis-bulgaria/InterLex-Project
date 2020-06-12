using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Models.EuFins;
using Interlex.BusinessLayer.Models.EuFins.Eurostat;
using Interlex.App.ViewModels;
using Interlex.BusinessLayer.Enums;

namespace Interlex.App.Controllers
{
    public class FinancesController : BaseController
    {
        // GET: Finance
        public ActionResult Index(string page, string from, string to, string type)
        {
            ViewBag.Page = page;

            if (String.IsNullOrEmpty(from))
            {
                from = "0";
            }

            if (String.IsNullOrEmpty(to))
            {
                to = "0";
            }

            if (String.IsNullOrEmpty(type))
            {
                type = "1";
            }

            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.Type = type;

            return View();
        }

        public ActionResult EuroLibor()
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            if (UserData.Username.ToLower() == "sysdemo")
            {
                ViewBag.UseLayout = false;
                return PartialView("~/Views/Shared/_ProductFeaturesInfo_Finances.cshtml", new ProductFeaturesInfo(FunctionalityTypes.Finances));
            }

            var euroLibor = Interlex.BusinessLayer.Models.EuFins.EuroLibor.GetFinsEuroLiborTypes();

            var model = new EuroLibor()
            {
                Libors = euroLibor,
                DateAsString = Interlex.BusinessLayer.Models.EuFins.EuroLibor.GetFinsEuroLiborLastDate(euroLibor[0].Name).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            return PartialView("~/Views/Finances/_EuroLibor.cshtml", model);
        }

        // TODO: Rework this shit
        public ActionResult Euribor()
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }


            if (UserData.Username.ToLower() == "sysdemo")
            {
                ViewBag.UseLayout = false;
                return PartialView("~/Views/Shared/_ProductFeaturesInfo_Finances.cshtml", new ProductFeaturesInfo(FunctionalityTypes.Finances));
            }

            var euroLibor = Interlex.BusinessLayer.Models.EuFins.EuroLibor.GetFinsEuroLiborTypes();

            var model = new EuroLibor()
            {
                Libors = euroLibor,
                DateAsString = Interlex.BusinessLayer.Models.EuFins.EuroLibor.GetFinsEuroLiborLastDate(euroLibor[0].Name).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            return PartialView("~/Views/Finances/_Euribor.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetEuroLiborData(TableFormData data)
        {
            data.Month += 1;
            var newDate = new DateTime(data.Year, data.Month, 1);

            var euroLibor = Interlex.BusinessLayer.Models.EuFins.EuroLiborDataRow.GetFinsEuroLibor(data.LiborFor, newDate, data.Name, "asc");

            var tableData = new TableData<EuroLiborDataRow>(euroLibor)
            {
                Draw = data.Draw
            };
            tableData.RecordsTotal = tableData.Data.Count();
            tableData.RecordsFiltered = tableData.Data.Count();

            return CamelCaseJson(tableData);
        }

        [HttpPost]
        public ActionResult GetEuRiborData(TableFormData data)
        {
            var euribor = EuroLiborDataRow.GetFinsEuribor(data.LiborFor, data.Year, "asc");

            var tableData = new TableData<EuroLiborDataRow>(euribor)
            {
                Draw = data.Draw
            };
            tableData.RecordsTotal = tableData.Data.Count();
            tableData.RecordsFiltered = tableData.Data.Count();

            return CamelCaseJson(tableData);
        }

        public ActionResult FinancesCurrency()
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            var financesCurrency = Interlex.BusinessLayer.Models.EuFins.FinsCurrency.GetFinsCurrencyTypes();

            var model = new FinsCurrency()
            {
                FinsCurrencies = financesCurrency,
                DateAsString = Interlex.BusinessLayer.Models.EuFins.FinsCurrency.GetFinsCurrencyLastDate(financesCurrency[0].Name).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            return PartialView("~/Views/Finances/_FinsCurrency.cshtml", model);
        }

        public ActionResult FinancesCurrencyECB()
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            var financesCurrencyECB = Interlex.BusinessLayer.Models.EuFins.FinsCurrency.GetFinsCurrencyEcbTypes();

            var model = new FinsCurrency("ecb")
            {
                FinsCurrencies = financesCurrencyECB,
                DateAsString = Interlex.BusinessLayer.Models.EuFins.FinsCurrency.GetFinsCurrencyEcbLastDate(financesCurrencyECB[0].Name).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            return PartialView("~/Views/Finances/_FinsCurrencyECB.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetFinancesCurrencyData(TableFormData data)
        {
            data.Month += 1;
            var newDate = new DateTime(data.Year, data.Month, 1);

            var financesCurrency = FinsCurrencyDataRow.GetFinsEuroLibor(data.CurrentCurrency, newDate, "asc");

            var tableData = new TableData<FinsCurrencyDataRow>(financesCurrency)
            {
                Draw = data.Draw
            };
            tableData.RecordsTotal = tableData.Data.Count();
            tableData.RecordsFiltered = tableData.Data.Count();

            return CamelCaseJson(tableData);
        }

        [HttpPost]
        public ActionResult GetFinancesCurrencyECBData(TableFormData data)
        {
            data.Month += 1;
            var newDate = new DateTime(data.Year, data.Month, 1);

            var financesCurrency = FinsCurrencyEcbDataRow.GetFinsCurrency(data.CurrentCurrency, newDate, "asc");

            var tableData = new TableData<FinsCurrencyEcbDataRow>(financesCurrency)
            {
                Draw = data.Draw
            };
            tableData.RecordsTotal = tableData.Data.Count();
            tableData.RecordsFiltered = tableData.Data.Count();

            return CamelCaseJson(tableData);
        }

        public ActionResult StockIndex()
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            if (UserData.Username.ToLower() == "sysdemo")
            {
                ViewBag.UseLayout = false;
                return PartialView("~/Views/Shared/_ProductFeaturesInfo_Finances.cshtml", new ProductFeaturesInfo(FunctionalityTypes.Finances));
            }

            var stockIndex = FinsStockIndex.GetFinsStockIndexTypes();

            var model = new FinsStockIndex()
            {
                StockIndexes = stockIndex,
                DateAsString = Interlex.BusinessLayer.Models.EuFins.FinsStockIndex.GetFinsStockIndexLastDate(stockIndex[0].Name).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            return PartialView("~/Views/Finances/_FinsStockIndex.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetStockIndex(TableFormData data)
        {
            data.Month += 1;

            var newDate = new DateTime(data.Year, data.Month, 1);

            var stockIndex = FinsStockIndexDataRow.GetFinsEuroLibor(data.Name, newDate, "asc");

            var tableData = new TableData<FinsStockIndexDataRow>(stockIndex)
            {
                Draw = data.Draw
            };
            tableData.RecordsTotal = tableData.Data.Count();
            tableData.RecordsFiltered = tableData.Data.Count();

            return CamelCaseJson(tableData);
        }

        public ActionResult EuroStat()
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            int curLang = Language.Id;

            var euroStat = new Eurostat()
            {
                LastExtraction = GdpPerCapitaDataRow.GetLastExtraction(),
                //  euroStat.StatisticName = Resources.Resources.UI_fins_eurostat_HICP;
                StatisticName = Resources.Resources.UI_EuroStat
            };
            euroStat.Tables.Add(new EuroStatTable()
            {
                Stats = GdpPerCapitaDataRow.GetGdpPerCapitaDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                TableName = Resources.Resources.UI_fins_eurostat_HICP_PchMV12
            });

            ViewBag.Page = "GDPCapitalPPS";

            return PartialView("~/Views/Finances/_EuroStat.cshtml", euroStat);
        }

        public ActionResult GetEuroStatTable(TableFormData data)
        {
            int curLang = Language.Id;
            var stats = GdpPerCapitaDataRow.GetGdpPerCapitaDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList();
            var tableData = new TableDataEurostat()
            {
                Data = new List<Dictionary<string, string>>()
            };
            int cnt = 0;
            foreach (var stat in stats)
            {
                cnt++;
                var row = new Dictionary<string, string> { { "id", cnt.ToString() } };
                row.Add("country", stat.Country);
                foreach (var value in stat.HicpValues)
                {
                    row.Add(value.TimePeriod.Trim(), value.TableValue);
                }

                tableData.Data.Add(row);
            }

            tableData.Draw = data.Draw;
            tableData.RecordsTotal = tableData.Data.Count();
            tableData.RecordsFiltered = tableData.Data.Count();

            return CamelCaseJson(tableData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GDPCGT(string from, string to, string type)
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            int curLang = Language.Id;

            var euroStat = new Eurostat()
            {
                LastExtraction = RealGdp.GetLastExtraction(),
                StatisticName = Resources.Resources.UI_FinsStatGDPCGT //GET STATISTIC NAME
            };
            if (type == "1")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = RealGdp.GetClv10DataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Верижни обеми (2010 г.), евро на глава от населението"
                });

            }
            else if (type == "2")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = RealGdp.GetClvPchDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Верижни обеми, процентна промяна спрямо предходен период, на глава от населението"
                });
            }

            return PartialView("~/Views/Finances/_EuroStat.cshtml", euroStat);
        }

        public ActionResult GrossDomesticProduct(string from, string to, string type)
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            int curLang = Language.Id;

            var euroStat = new Eurostat()
            {
                LastExtraction = GdpDataRow.GetLastExtraction(),
                StatisticName = Resources.Resources.UI_FinsStatGrossDomesticProduct //GET STATISTIC NAME
            };
            if (type == "1")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = GdpDataRow.GetEurHabDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Текущи цени, евро на глава от населението"
                });
            }
            else if (type == "2")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = GdpDataRow.GetMeurDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Текущи цени, милиона евро"
                });
            }
            else if (type == "3")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = GdpDataRow.GetMppsDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Текущи цени, милион стандарти на покупателна способност"
                });
            }

            return PartialView("~/Views/Finances/_EuroStat.cshtml", euroStat);
        }

        public ActionResult GovernmentGrossDebt(string from, string to, string type)
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            int curLang = Language.Id;

            var euroStat = new Eurostat()
            {
                LastExtraction = GggdDataRow.GetLastExtraction(),
                StatisticName = Resources.Resources.UI_FinsStatGovernmentGrossDebt //GET STATISTIC NAME
            };
            if (type == "1")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = GggdDataRow.GetMioeurDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Милиони евро"
                });
            }
            else if (type == "2")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = GggdDataRow.GetPcgdpDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Процент от брутния вътрешен продукт (БВП)"
                });
            }

            return PartialView("~/Views/Finances/_EuroStat.cshtml", euroStat);
        }

        public ActionResult HarmonizedIndicesConsumerPrices(string from, string to, string type)
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            int curLang = Language.Id;

            var euroStat = new Eurostat()
            {
                LastExtraction = HicpDataRow.GetLastExtraction(),
                StatisticName = Resources.Resources.UI_FinsStatHarmonizedIndicesConsumerPrices //GET STATISTIC NAME
            };

            if (type == "1")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HicpDataRow.GetI2005DataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Индекс 2005=100"
                });
            }
            else if (type == "2")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HicpDataRow.GetPchM12DataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Годишна инфлация: съответният месец от предходната година=100, проценти"
                });
            }
            else if (type == "3")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HicpDataRow.GetPchM1DataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Месечна инфлация: предходният месец=100, проценти"
                });
            }
            else if (type == "4")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HicpDataRow.GetPchMV12DataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Средногодишна инфлация: предходните 12 месеца=100, проценти"
                });
            }

            return PartialView("~/Views/Finances/_EuroStat.cshtml", euroStat);
        }

        public ActionResult HarmonizedUnemploymentRateBySex(string from, string to, string type)
        {
            if (!this.ValidateHasEuroFins())
            {
                return PartialView("~/Views/Error/_UnavaiableProductEuroFins.cshtml");
            }

            int curLang = Language.Id;

            var euroStat = new Eurostat()
            {
                LastExtraction = HursDataRow.GetLastExtraction(),
                StatisticName = Resources.Resources.UI_FinsStatHarmonizedUnemploymentRateBySex //GET STATISTIC NAME
            };

            if (type == "1")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HursDataRow.GetTDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Общо"
                });
            }
            else if (type == "2")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HursDataRow.GetMDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Мъже"
                });
            }
            else if (type == "3")
            {
                euroStat.Tables.Add(new EuroStatTable()
                {
                    Stats = HursDataRow.GetFDataRows(new DateTime(2010, 1, 1), new DateTime(2017, 1, 1), curLang).ToList(),
                    TableName = "Жени"
                });
            }


            return PartialView("~/Views/Finances/_EuroStat.cshtml", euroStat);
        }

        public ActionResult Pragove()
        {
            int curLang = Language.Id;
            string model = Prag.GetPrag(new Guid("afea8c6f-408b-49fa-a62d-939c94e2909d"), curLang);

            return PartialView("~/Views/Finances/_Pragove.cshtml", model);
        }

        public ActionResult TaxInformation(string type)
        {
            int curLang = Language.Id;
            string model = String.Empty;

            if (type == "1")
            {
                model = TaxInformationModel.GetTaxInformation(new Guid("fb22b24c-4872-457a-b2d3-68bf5cf1c112"), curLang);
            }
            else if (type == "2")
            {
                model = TaxInformationModel.GetTaxInformation(new Guid("aee7960d-bd6f-425e-b6a8-f2dac27f107b"), curLang);
            }
            else if (type == "3")
            {
                model = TaxInformationModel.GetTaxInformation(new Guid("29361d72-01c4-4057-ba28-8192c8dcad72"), curLang);
            }
            else if (type == "4")
            {
                model = TaxInformationModel.GetTaxInformation(new Guid("9ff3ca14-6041-4223-8eeb-bdc57cfcfd25"), curLang);
            }

            // TODO: Partial
            return PartialView("~/Views/Finances/_Pragove.cshtml", model);
        }

        /*        public ActionResult FinancialCrysisGlossary(string type)
                {
                    int curLang = Language.Id;
                    string model = FinancialCrysisGlossaryModel.GetFinancialCrysisGlossary(new Guid("5639aefb-34aa-487e-8068-064fd97028f1"), curLang);

                    //TODO: Partial
                    return PartialView("~/Views/Finances/_Pragove.cshtml", model);
                }*/

        public JsonResult GetFinancesDataLastDate(string type, string subType)
        {
            DateTime lastDate = DateTime.Now;

            return Json(lastDate);
        }

        private bool ValidateHasEuroFins()
        {
            return UserData.Products.Count(m => m.ProductId == 2) > 0 ? true : false;
        }
    }
}