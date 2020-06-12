using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Interlex.App.Filters;
using Newtonsoft.Json;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Entities;

namespace Interlex.App.Controllers
{

    [UserAuthorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            HomeData h = Home.GetHomeData(this.UserData.UserId, this.Language.Id, this.ProductId);
            return View(h);
            //return RedirectToAction("Index", "RecentDocuments");
        }

        public ActionResult ChangeProduct(string productName)
        {
            if (UserData.Username.ToLower() != "sysdemo")
            {
                return RedirectToAction("Index");
            }

            int productId = 1; // EuroCases
            if (productName.ToUpper() == "TAXANDFINANCIALSTANDARDS")
            {
                productId = 2;
            }

            Session["SelectedProductId"] = productId.ToString();

            var updatedCookie = new HttpCookie("SelectedProductId")
            {
                HttpOnly = false,
                Secure = false,
                Expires = DateTime.Now.AddDays(365)
            };
            updatedCookie.Value = productId.ToString();
            Response.Cookies.Set(updatedCookie);

            return RedirectToAction("Index");
        }

        public ActionResult ChangeLanguage(string shortCode)
        {
            var langModel = InterfaceLanguages.GetLanguageByShortCode(shortCode);
            if (langModel == null)
            {
                return RedirectToAction("Index");
            }

            //this.Language = langModel;
            Session["LanguageId"] = langModel.Id;
            var cookie = new HttpCookie("sitelang", langModel.Code)
            {
                HttpOnly = false,
                Secure = false,
                Expires = DateTime.Now.AddDays(365)
            };
            HttpContext.Response.Cookies.Add(cookie);
            
            return RedirectToAction("Index");
        }

        public ActionResult MachineTranslation(string language) // shortcode
        {
            string viewName = "MachineTranslation";
            Language currentLang = InterfaceLanguages.GetLanguageByShortCode(language);
            ViewBag.langs = new string[] { "bg", "en", "de", "fr", "it", "pt", "pl", "nl"};
            if (!String.IsNullOrEmpty(language))
            {
                string shortLang;
                string fullViewName;

                shortLang = language;
                fullViewName = viewName + shortLang;
                if(this.ViewExists(fullViewName))
                {
                    return View(fullViewName, currentLang); 
                }
            }

            currentLang = InterfaceLanguages.GetLanguageByShortCode("en");
            return View(viewName + "en", currentLang);
        }

        #region Static folders (obsolete)

        public ActionResult GetMainFolders(string key)
        {
            List<FolderData> list = new List<FolderData>();
            switch (key)
            {
                case "1_":
                    list.Add(new FolderData() { title = "European Union", key = "1_1_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Austria", key = "1_2_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Bulgaria", key = "1_3_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "France", key = "1_4_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Germany", key = "1_5_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Italy", key = "1_6_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "United Kingdom", key = "1_7_", folder = true, lazy = true });    
                    break;

                case "1_1_":
                    list.Add(new FolderData() { title = "Treaties", key = "1_1_1_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Regulations", key = "1_1_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Directives", key = "1_1_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Decisions", key = "1_1_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other Documents", key = "1_1_5_", folder = true, lazy = false });
                    break;
                case "1_2_":
                    list.Add(new FolderData() { title = "Constitution", key = "1_2_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Laws", key = "1_2_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "By-laws", key = "1_2_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other Documents", key = "1_2_4_", folder = true, lazy = false });
                    break;
                case "1_3_":
                    list.Add(new FolderData() { title = "Constitution", key = "1_3_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Laws", key = "1_3_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "By-laws", key = "1_3_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other Documents", key = "1_3_4_", folder = true, lazy = false });
                    break;
                case "1_4_":
                    list.Add(new FolderData() { title = "Constitution", key = "1_4_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Laws", key = "1_4_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "By-laws", key = "1_4_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other Documents", key = "1_4_4_", folder = true, lazy = false });
                    break;
                case "1_5_":
                    list.Add(new FolderData() { title = "Constitution", key = "1_5_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Laws", key = "1_5_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "By-laws", key = "1_5_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other Documents", key = "1_5_4_", folder = true, lazy = false });
                    break;
                case "1_6_":
                    list.Add(new FolderData() { title = "Constitution", key = "1_6_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Laws", key = "1_6_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "By-laws", key = "1_6_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other Documents", key = "1_6_4_", folder = true, lazy = false });
                    break;
                case "1_7_":
                    list.Add(new FolderData() { title = "Acts of Parliament", key = "1_7_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Statutory instruments", key = "1_7_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Other documents", key = "1_7_3_", folder = true, lazy = false });
                    break;
                case "2_":
                    list.Add(new FolderData() { title = "European Union", key = "2_1_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Austria", key = "2_2_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Bulgaria", key = "2_3_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "France", key = "2_4_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Germany", key = "2_5_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Italy", key = "2_6_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "United Kingdom", key = "2_7_", folder = true, lazy = true });
                    break;
                case "2_1_":
                    list.Add(new FolderData() { title = "Court of Justice of the European Union", key = "2_1_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Court of First Instance", key = "2_1_2_", folder = true, lazy = false });
                    break;
                case "2_2_":
                    list.Add(new FolderData() { title = "Verfassungsgerichtshof", key = "2_2_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Verwaltungsgerichtshof", key = "2_2_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Oberster Gerichtshof", key = "2_2_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Bundesverwaltungsgericht", key = "2_2_4_", folder = true, lazy = false });
                    break;
                case "2_3_":
                    list.Add(new FolderData() { title = "Конституционен Съд", key = "2_3_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Върховен Kасационен Съд", key = "2_3_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Върховен Административен Съд", key = "2_3_3_", folder = true, lazy = false });
                    break;
                case "2_4_":
                    list.Add(new FolderData() { title = "Conseil Constitutionnel", key = "2_4_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Cour de Cassation", key = "2_4_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Conseil d'Etat", key = "2_4_3_", folder = true, lazy = false });
                    break;
                case "2_5_":
                    list.Add(new FolderData() { title = "Bundesverfassungsgericht", key = "2_5_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Bundesgerichtshof", key = "2_5_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Bundessozialgericht", key = "2_5_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Bundesarbeitsgericht", key = "2_5_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Bundesfinanzhof", key = "2_5_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Bundesverwaltungsgericht", key = "2_5_5_", folder = true, lazy = false });
                    break;
                case "2_6_":
                    list.Add(new FolderData() { title = "Corte Costituzionale", key = "2_6_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Corte Suprema di Cassazione", key = "2_6_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consiglio di Stato", key = "2_6_3_", folder = true, lazy = false });
                    break;
                case "2_7_":
                    list.Add(new FolderData() { title = "Supreme Court", key = "2_7_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "House of Lords", key = "2_7_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "High Court of Justice", key = "2_7_3_", folder = true, lazy = false });
                    break;
                case "4_":
                    list.Add(new FolderData() { title = "Package travel", key = "4_1_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Unfair contract terms", key = "4_2_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Consumer sales", key = "4_3_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Injunction", key = "4_4_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Distance selling", key = "4_5_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Doorstep selling", key = "4_6_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Timeshare", key = "4_7_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Price indication", key = "4_8_", folder = true, lazy = true });
                    break;

                case "4_1_":
                    list.Add(new FolderData() { title = "Avoidance", key = "4_1_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Obligations of organiser", key = "4_1_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Rider", key = "4_1_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Damages", key = "4_1_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Performance", key = "4_1_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Non-performance", key = "4_1_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Organizer", key = "4_1_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Package", key = "4_1_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Package travel directive", key = "4_1_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Price revision", key = "4_1_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Retailer", key = "4_1_11_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Security in case of insolvency", key = "4_1_12_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Transfer of booking", key = "4_1_13_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Withdrawal, right of", key = "4_1_14_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_1_15_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Formal requirements", key = "4_1_16_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Liability", key = "4_1_17_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Sanctions", key = "4_1_18_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Option of member states", key = "4_1_19_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_1_20_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Cancellation of withdrawal", key = "4_1_21_", folder = true, lazy = false });
                    break;
                case "4_2_":
                    list.Add(new FolderData() { title = "Arbitration clause", key = "4_2_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Good faith", key = "4_2_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Terms of contract", key = "4_2_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Unfair contract terms directive", key = "4_2_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Unfair terms", key = "4_2_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Burden of proof", key = "4_2_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_2_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Legal action, right to take", key = "4_2_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Sanctions", key = "4_2_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Reference of national transposition law", key = "4_2_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_2_11_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Seller", key = "4_2_12_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Supplier", key = "4_2_13_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Falling of the scope", key = "4_2_14_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Definition of contract", key = "4_2_15_", folder = true, lazy = false });
                    break;

                case "4_3_":
                    list.Add(new FolderData() { title = "Concurring claims", key = "4_3_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer goods", key = "4_3_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer sales directive", key = "4_3_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Disproportionate remedy", key = "4_3_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Expiration of consumer rights", key = "4_3_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Free of charge", key = "4_3_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Guarantee", key = "4_3_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Lack of conformity", key = "4_3_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Price reduction, right of", key = "4_3_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Producer", key = "4_3_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Replacement, right of", key = "4_3_11_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Rescission, right of", key = "4_3_12_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_3_13_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Formal requirements", key = "4_3_14_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Liability", key = "4_3_15_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Reference of national transposition law", key = "4_3_16_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Minimum clause", key = "4_3_17_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Redress, right of", key = "4_3_18_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Option of member states", key = "4_3_19_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_3_20_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer sales directive", key = "4_3_21_", folder = true, lazy = false });
                    break;

                case "4_4_":
                    list.Add(new FolderData() { title = "Consumer organisation", key = "4_4_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Infringement", key = "4_4_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Injunctions directive", key = "4_4_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Injunction, action for", key = "4_4_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Qualified entity", key = "4_4_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Prior consultation, duty of", key = "4_4_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_4_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Legal action, right to take", key = "4_4_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Reference of national transposition law", key = "4_4_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Option of member states", key = "4_4_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_4_11_", folder = true, lazy = false });
                    break;

                case "4_5_":
                    list.Add(new FolderData() { title = "Distance selling directive", key = "4_5_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Fraudulent card payments", key = "4_5_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Inertia selling", key = "4_5_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Means of distance communication", key = "4_5_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Operator of means of communication", key = "4_5_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Performance, period of", key = "4_5_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Priority of specific community rules", key = "4_5_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Unavailability of goods or services", key = "4_5_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Burden of proof", key = "4_5_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Withdrawal, right of", key = "4_5_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_5_11_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Credit agreement", key = "4_5_12_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Formal requirements", key = "4_5_13_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Legal action, right to take", key = "4_5_14_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Sanctions", key = "4_5_15_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Reference of national transposition law", key = "4_5_16_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Minimum clause", key = "4_5_17_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Redress, right of", key = "4_5_18_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Option of member states", key = "4_5_19_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_5_20_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Supplier", key = "4_5_21_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Credit agreement", key = "4_5_22_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Cancellation of  withdrawal", key = "4_5_23_", folder = true, lazy = false });
                    break;

                case "4_6_":
                    list.Add(new FolderData() { title = "Doorstep selling directive", key = "4_6_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Time limit for transposition", key = "4_6_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Withdrawal, right of", key = "4_6_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_6_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Sanctions", key = "4_6_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Minimum clause", key = "4_6_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Option of member states", key = "4_6_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_6_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Trader", key = "4_6_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Cancellation of  withdrawal", key = "4_6_10_", folder = true, lazy = false });
                    break;

                case "4_7_":
                    list.Add(new FolderData() { title = "Immovable property", key = "4_7_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Timeshare directive", key = "4_7_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Non-compliance with the directive", key = "4_7_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Purchaser", key = "4_7_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Vendor", key = "4_7_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Withdrawal, right of", key = "4_7_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_7_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Credit agreement", key = "4_7_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Formal requirements", key = "4_7_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Reference of national transposition law", key = "4_7_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_7_11_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Credit agreement", key = "4_7_12_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Cancellation of withdrawal", key = "4_7_13_", folder = true, lazy = false });
                    break;

                case "4_8_":
                    list.Add(new FolderData() { title = "Indication duties", key = "4_8_1", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Waiver of indication obligation", key = "4_8_2_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Price indication directive", key = "4_8_3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Products sold in bulk", key = "4_8_4_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Selling price", key = "4_8_5_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Unit price", key = "4_8_6_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Consumer", key = "4_8_7_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Formal requirements", key = "4_8_8_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Sanctions", key = "4_8_9_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Reference of national transposition law", key = "4_8_10_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Option of member states", key = "4_8_11_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Scope of application", key = "4_8_12_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Trader", key = "4_8_13_", folder = true, lazy = false });
                    break;

                default:
                    list.Add(new FolderData() { title = "Consumer Legislation", key = "1_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Consumer Cases", key = "2_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "Legal Doctrine Articles", key = "3_", folder = true, lazy = false });
                    list.Add(new FolderData() { title = "Classifier by Subject Matter", key = "4_", folder = true, lazy = true });
                    list.Add(new FolderData() { title = "New Documents", key = "5_", folder = true, lazy = false });
                    break;
            }           
            string json = JsonConvert.SerializeObject(list);
            return Content(json, "application/json");
        }

        #endregion

    }
}
