namespace Interlex.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Models;
    using System.Net.Mail;
    using System.Net;
    using System.Net.Configuration;
    using System.Diagnostics;
    using System.Configuration;
    using System.Web.Configuration;
    using System.Globalization;
    using Interlex.BusinessLayer.Enums;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Interlex.App.CustomValidators;

    [AllowAnonymous]
    public class LoginController : BaseController
    {
        public ActionResult Index(int? forceLogin)
        {
            //string[] host = Request.Url.Host.Split('.');
            //bool isDemo = false;
            //if (host[0].ToLower() == "demo" || host[0].ToLower() == "freecases")
            //    isDemo = true;

            //if (isDemo)
            //{
            if (!forceLogin.HasValue && Session["UserData"] == null)
            {
                return Login("sysdemo", ConfigurationManager.AppSettings["DEMO_USER_PASS"]);
            }

            //  }

            if (Session["UserData"] != null)
            {
                if (Request.QueryString["returnUrl"] != null)
                {
                    return Redirect(Request.QueryString["returnUrl"]);
                }
                return RedirectToAction("Index", "Home");
            }

            if (ConfigurationManager.AppSettings.AllKeys.Contains("LOGIN_REDIRECT_URL"))
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LOGIN_REDIRECT_URL_FOR_IP_LIST"))
                {
                    string requestIP = Request.UserHostAddress;
                    string[] ipAllowed = ConfigurationManager.AppSettings["LOGIN_REDIRECT_URL_FOR_IP_LIST"].Split(';');
                    //if (!ipAllowed.Contains(requestIP))
                    //    return View();
                    if (ipAllowed.Count(x => requestIP.StartsWith(x) == true) > 0)
                        return View();
                }

                string siteLang = "en";
                if (this.Language.ShortCode.ToLower() == "bg")
                    siteLang = "en";

                string loginUrl = ConfigurationManager.AppSettings["LOGIN_REDIRECT_URL"] + siteLang + "/";
                if (Request.QueryString["returnUrl"] != null)
                    loginUrl += "?returnUrl=" + Request.QueryString["returnUrl"];
                return Redirect(loginUrl);
            }

            return View();
        }

        private UserData ProcessLogin(string username, string password, bool passHashed, string ipAddr, string language, ref string errorMsg)
        {
            CultureInfo ci = new CultureInfo(language);

            UserData ud = UserMng.Login(username, password, passHashed, ipAddr);
            //string errorMsg = null;
            if (ud != null)
            {
                if (ud.SessionId == -1)
                {
                    errorMsg = Resources.Resources.ResourceManager.GetString("UI_MaxUserLoginCountReached", ci);
                    //errorMsg = Resources.Resources.UI_MaxUserLoginCountReached;
                }
                else if (ud.SessionId == -2)
                {
                    errorMsg = Resources.Resources.UI_SubscriptionExpired;
                }
                else if (ud.SessionId == -3)
                {
                    errorMsg = Resources.Resources.UI_EmailNotValidated;
                }
                else if (ud.SessionId == -4)
                {
                    errorMsg = Resources.Resources.ResourceManager.GetString("UI_UserNotActive", ci);
                    //errorMsg = Resources.Resources.UI_UserNotActive;
                }
                else if (ud.SessionId == -5)
                {
                    errorMsg = Resources.Resources.ResourceManager.GetString("UI_IpNotAllowed", ci);
                }
                else if (ud.SessionId > 0)
                {
                    Session["UserData"] = ud;
                    Session.Timeout = ud.SessionTimeout; // minutes

                    var browserString = Request.Browser.Browser;
                    var userAgentString = Request.UserAgent;
                    var browserId = (int)GetBrowserByBrowserAndUserAgentString(browserString, userAgentString);
                    var isMobileDevice = Request.Browser.IsMobileDevice;

                    if (Common.CheckRequestOriginIsBotSoft(userAgentString) == false)
                    {
                        Stat.AddLogin(ud.UserId, browserId, isMobileDevice, Request.UserHostAddress, ud.ClientId, ud.SellerId);
                    }

                    var productsList = UserMng.GetProductsList(ud.UserId);
                    Session["ProductsList"] = productsList;

                    var selectedProductCookie = Request.Cookies["SelectedProductId"];
                    int selectedProductId = 1;

                    if (UserData.Products.Where(p => p.IsActive.HasValue && p.IsActive == true).ToList().Count == 1)
                    {
                        selectedProductId = UserData.Products.Where(p => p.IsActive.HasValue && p.IsActive == true).ToList().FirstOrDefault().ProductId;
                        var newCookie = new HttpCookie("SelectedProductId");
                        newCookie.Value = selectedProductId.ToString();

                        Response.SetCookie(newCookie);
                    }
                    else
                    {
                        if (selectedProductCookie != null && selectedProductCookie.Value != null && selectedProductCookie.Value.ToString() != "")
                        {
                            selectedProductId = int.Parse(selectedProductCookie.Value.ToString());
                        }
                    }

                    Session["SelectedProductId"] = selectedProductId;
                }
            }
            else
            {
                errorMsg = Resources.Resources.UI_InvalidLogin;
            }

            return ud;
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (Session["UserData"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Login process
            string errorMsg = "";
            UserData ud = ProcessLogin(username, password, false, Request.UserHostAddress, this.Language.ShortCode, ref errorMsg);


            if (String.IsNullOrEmpty(errorMsg))
            {
                if (Request.QueryString["returnUrl"] != null)
                {
                    //Response.Redirect(Request.QueryString["returnUrl"], true);
                    return Redirect(Request.QueryString["returnUrl"]);
                    //return null;
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);

                ViewBag.ErrorMsg = errorMsg;
                return View("Index");
            }
        }

        private string GetCORS_Remote_Domain()
        {
            string siteName = ConfigurationManager.AppSettings["LOGIN_REDIRECT_URL"];
            if (siteName[siteName.Length - 1] == '/')
                siteName = siteName.Remove(siteName.Length - 1);

            return siteName;
        }

        [HttpPost]
        public JsonResult LoginSite(string username, string password, string language)
        {
            string siteName = GetCORS_Remote_Domain();
            Response.Headers.Add("Access-Control-Allow-Origin", siteName);

            string errorMsg = "";
            if (Session["UserData"] == null)
            {
                UserData ud = ProcessLogin(username, password, true, Request.UserHostAddress, this.Language.ShortCode, ref errorMsg);
                if (!String.IsNullOrEmpty(errorMsg))
                {
                    Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);
                }
            }

            return Json(errorMsg);
        }

        [HttpPost]
        public JsonResult LogoutSite()
        {
            string siteName = GetCORS_Remote_Domain();
            Response.Headers.Add("Access-Control-Allow-Origin", siteName);

            Session.Abandon();

            return Json("OK");
        }

        private Browsers GetBrowserByBrowserAndUserAgentString(string browserString, string userAgentString)
        {
            if (userAgentString.Contains("Edge"))
            {
                return Browsers.MicrosoftEdge;
            }
            else if (browserString == "Chrome")
            {
                if (userAgentString.Contains("Opera") || userAgentString.Contains("OPR"))
                {
                    return Browsers.Opera;
                }
                else
                {
                    return Browsers.Chrome;
                }
            }
            else if (browserString == "Opera")
            {
                return Browsers.Opera;
            }
            else if (browserString == "Firefox")
            {
                return Browsers.MozillaFirefox;
            }
            else if (browserString == "Safari")
            {
                return Browsers.Safari;
            }
            else if (browserString == "InternetExplorer")
            {
                return Browsers.InternetExplorer;
            }
            else if (browserString == "Edge")
            {
                return Browsers.MicrosoftEdge;
            }
            else
            {
                return Browsers.Other;
            }
        }

        public ActionResult Logout(int? forceLogin)
        {
            // Response.Cookies.Clear();

            // Remove all site cookies.
            /* string[] myCookies = Request.Cookies.AllKeys;
             foreach (string cookie in myCookies)
             {
                 if (cookie != "sitelang" && cookie != "cb-enabled")
                     Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
             }*/
            Session.Abandon();

            if (forceLogin.HasValue)
            {
                return RedirectToAction("Index", "Login", new { forceLogin });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Empty method used just to refresh server session. This method is called from dialog 'Session expire' if button to continue session is clicked.
        /// </summary>
        /// <returns></returns>
        public int RefreshSession()
        {
            Debug.WriteLine("RefreshSession call");
            return 1;
        }

        /// <summary>
        /// Called when javascript timer for session expire ends. Needed to make sure session is not alive after timer ends (IIS server session time management and javascript timer always differ).
        /// That way we follow javascript client timer because the client is always right;)
        /// </summary>
        public void EndSession()
        {
            if (Session["UserData"] != null)
            {
                Debug.WriteLine("EndSession call from javascript");
                Session.Abandon();
            }
        }

        public ActionResult Registration()
        {
            return View();
        }

        //    [ValidateAjax]
        public ActionResult AddRegistration(RegisterUserData rud)
        {
            var captchaResponseToken = Request.Form["g-recaptcha-response"];
            bool res = Common.ValidateCaptcha(captchaResponseToken);
            if (!res)
            {
                ModelState.AddModelError(String.Empty, "Invalid Captcha"); // for summary-view only

                return Json(this.GetModelStateDictErrors(ModelState));
            }

            if (ModelState.IsValid)
            {
                var fakeProducts = new List<Product>();
                var fakeProduct = new Product
                {
                    Demo = false,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(10),
                    IsActive = true,
                    ProductId = 1,
                    Selected = true,
                    LicenseCnt = 1
                };

                fakeProducts.Add(fakeProduct);

                var fakeClient = new ClientData();
                fakeClient.ClientName = rud.Mail;
                fakeClient.CountryId = 36;
                fakeClient.OriginId = 1;
                fakeClient.Products = fakeProducts;

                var clientId = UserMng.SetClient(6510, fakeClient);

                var fakeUserDataAdm = new UserDataAdm
                {
                    Email = rud.Mail,
                    EmailValid = false, // consider not validating this at start
                    Username = rud.Mail,
                    UserType = UserTypes.User,
                    Active = true, // consider not activating this at start
                    Fullname = rud.Mail, // intentionally
                    ClientId = clientId,
                    Password = rud.Password,
                    OriginId = 1,
                    Products = fakeProducts
                };

                var userId = UserMng.SetUser(fakeUserDataAdm, 6510); // TODO: get some system id

                // SEND ACTIVATION EMAIL
                string subject = "Interlex account activation";

                string token = UserMng.AddUservalidateToken(userId);
                string applicationPath = Request.ApplicationPath != "/" ? Request.ApplicationPath : String.Empty;
                string activationUrl = "http://" + Request.Url.Authority + applicationPath + "/Login/Activate/" + token;
                string activationLink = "<a href=\"" + activationUrl + "\" target=\"_blank\">" + activationUrl + "</a>";
                string body = Resources.Resources.UI_Registration_MailBody;
                body = body.Replace("{fullname}", rud.Mail); // intentionally using mail instead of fullname
                body = body.Replace("{activation_link}", activationLink);

                // var model = new RegistrationEmail(activationLink, name);
                // var body = base.GetViewString("~/Views/User/_RegistrationEmail.cshtml", model, language);

                Mail.SendMail("support@apis.bg", "Interlex", new string[] { rud.Mail }, subject, body);

                return PartialView("AfterRegistration");
            }

           return Json(this.GetModelStateDictErrors(ModelState));
        }

        private dynamic GetModelStateDictErrors(ModelStateDictionary modelState)
        {
            var errorModel =
                       from x in modelState.Keys
                       where modelState[x].Errors.Count > 0
                       select new
                       {
                           key = x,
                           errors = modelState[x].Errors.
                                                         Select(y => y.ErrorMessage).
                                                         ToArray()
                       };

            var res = new
            {
                Data = errorModel
            };

            return res;
        }

        [HttpGet]
        public ActionResult LostPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LostPassword(LostPassword model)
        {
            var desiredUser = Interlex.BusinessLayer.Models.LostPassword.GetUser(model.Email);

            if (desiredUser == null)
            {
                TempData["msg"] = Resources.Resources.UI_RecoverPasswordEmailNotFound;
                return RedirectToAction("LostPassword");
            }
            //else if (model.Username != desiredUser["username"].ToString())
            //{
            //    TempData["msg"] = Resources.Resources.UI_RecoverPasswordUserNotForEmail;
            //    return RedirectToAction("LostPassword");
            //}
            else
            {
                var desiredUserFullName = desiredUser["fullName"].ToString();
                var code = Guid.NewGuid().ToString();

                Interlex.BusinessLayer.Models.LostPassword.InsertPasswordReset(int.Parse(desiredUser["user_id"].ToString()), code);

                this.SendEmail(model.Email, model.Email, code);

                TempData["msg"] = Resources.Resources.UI_ResetPassEmailSent;
                return RedirectToAction("LostPassword");
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string code)
        {
            return View(new ResetPassword() { Code = code });
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var desiredReset = Interlex.BusinessLayer.Models.LostPassword.GetPasswordReset(model.Code);

            if (desiredReset == null)
            {
                TempData["msg"] = Resources.Resources.UI_PasswordResetCodeInvalid;
                TempData["statusCode"] = "Error";
                return RedirectToAction("ResetPassword");
            }
            else if (model.Password != model.Password2)
            {
                TempData["msg"] = Resources.Resources.UI_PasswordsNotMatch;
                TempData["statusCode"] = "Error";
                return RedirectToAction("ResetPassword");
            }
            else
            {
                var desiredUserId = int.Parse(desiredReset["_user_id"].ToString());
                var issueDate = DateTime.Parse(desiredReset["_issue_date"].ToString());
                var expiryFromDB = desiredReset["_expiry_date"].ToString();

                var expiryDate = issueDate.AddMinutes(int.Parse(ConfigurationManager.AppSettings["PasswordReset_ExpiryMinutes"]));
                var now = DateTime.Now;
                var isCodeValid = DateTime.Compare(expiryDate, now);
                if (isCodeValid < 0)
                {
                    TempData["msg"] = Resources.Resources.UI_PasswordResetCodeExpired;
                    TempData["statusCode"] = "Error";
                    return RedirectToAction("ResetPassword");
                }
                else if (expiryFromDB != String.Empty)
                {
                    TempData["msg"] = Resources.Resources.UI_PasswordResetCodeUsed;
                    TempData["statusCode"] = "Error";
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    EditPassword.ChangePassword(desiredUserId, model.Password);
                    Interlex.BusinessLayer.Models.LostPassword.UpdatePasswordResetExpiry(model.Code);

                    TempData["msg"] = Resources.Resources.UI_PasswordChanged;
                    TempData["statusCode"] = "OK";
                    return RedirectToAction("ResetPassword");
                }
            }
        }

        private void SendEmail(string email, string fullName, string code)
        {
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            var host = smtpSection.Network.Host;
            var port = smtpSection.Network.Port;
            var defaultCredentials = smtpSection.Network.DefaultCredentials;
            var userName = smtpSection.Network.UserName;
            var password = smtpSection.Network.Password;

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(userName);

            SmtpClient smtp = new SmtpClient();

            smtp.Port = port;
            smtp.EnableSsl = false; //change to true if ssl certificate is up again
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = defaultCredentials;

            smtp.Credentials = new NetworkCredential(userName, password);
            smtp.Host = host;

            var solutionVersion = WebConfigurationManager.AppSettings["SolutionVersion"];

            string title = "Interlex";

            mail.To.Add(new MailAddress(email));
            mail.Subject = title + " " + Resources.Resources.UI_PasswordRecoveryTitle;

            string applicationPath = Request.ApplicationPath != "/" ? Request.ApplicationPath : String.Empty;
            string urlToActivate = "http://" + Request.Url.Authority + applicationPath + "/Login/ResetPassword/" + code;

            //Formatted mail body
            mail.IsBodyHtml = true;

            /*   string st = String.Format( "<h2>Greetings, <strong>{0}</strong></h2></br><h3>Please click the following link in order to change your password: <a href=\"{1}\">{2}</a></h3></br><h3>Code for resetting: <span style=\"color: blue; font-weight: bold;\">{3}</span></h3></br><img src=\"{4}\">",
                   fullName, urlToActivate, urlToActivate, code.ToString(), logoUrl);*/

            string emailText = String.Format(@Resources.Resources.UI_RecoveryEmailText, fullName, urlToActivate, urlToActivate, code.ToString());

            mail.Body = emailText;
            smtp.Send(mail);
        }

        private string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public ActionResult AddCookieAgreement()
        {
            var ip = this.GetIPAddress();

            int? userId;

            if (UserData != null)
            {
                userId = UserData.UserId;
            }
            else
            {
                userId = null;
            }

            UserMng.AddCookiesAgreement(userId, ip);

            return new EmptyResult();
        }

        public ActionResult LoginUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new EmptyResult();
            }

            var ud = UserMng.GetUserByUsername(username);
            var sessionUd = Session["UserData"] as UserData;
            var ci = new CultureInfo(this.Language.ShortCode);

            if (ud == null)
            {
                string errorMsg = Resources.Resources.ResourceManager.GetString("UI_NonExistingUsername", ci);
                ViewBag.ErrorMsg = errorMsg;
                ViewBag.Title = Resources.Resources.ResourceManager.GetString("UI_UnsuccessfulDirectLogin", ci);
                Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);

                return View("LoginUsernameFailure");
            }

            string requestIp = Request.UserHostAddress;
            if (ud.AllowedIps == null || ud.AllowedIps.Count == 0)
            {
                string errorMsg = Resources.Resources.ResourceManager.GetString("UI_DirectLoginForbidden", ci);
                ViewBag.ErrorMsg = errorMsg;
                ViewBag.Title = Resources.Resources.ResourceManager.GetString("UI_UnsuccessfulDirectLogin", ci);
                Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);

                return View("LoginUsernameFailure");
            }

            if (sessionUd != null && sessionUd.Username != username)
            {
                Session.Abandon();
                return RedirectToAction("LoginUsername", new { username = username });
            }

            if (UserMng.IsAllowedIpUsernameLogin(requestIp, ud.AllowedIps))
            {
                if (Session["UserData"] != null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Login process
                string errorMsg = "";
                ud = ProcessLogin(ud.Username, ud.Password, true, Request.UserHostAddress, this.Language.ShortCode, ref errorMsg);

                if (string.IsNullOrEmpty(errorMsg))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);
                    ViewBag.ErrorMsg = errorMsg;
                    ViewBag.Title = Resources.Resources.ResourceManager.GetString("UI_UnsuccessfulDirectLogin", ci);
                    return View("LoginUsernameFailure");
                }
            }
            else
            {
                string errorMsg = Resources.Resources.ResourceManager.GetString("UI_IpNotAllowed", ci) + " " + requestIp;
                ViewBag.ErrorMsg = errorMsg;
                ViewBag.Title = Resources.Resources.ResourceManager.GetString("UI_UnsuccessfulDirectLogin", ci);
                Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);

                return View("LoginUsernameFailure");
            }
        }

        public ActionResult LoginUsernamePasswordHash(string username, string passwordHash)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(passwordHash))
            {
                return new EmptyResult();
            }

            // data for error for presentation
            string siteLang = "en";
            if (this.Language.ShortCode.ToLower() == "bg")
                siteLang = "en";
            string loginUrl = ConfigurationManager.AppSettings["LOGIN_REDIRECT_URL"] + siteLang + "/";
            if (Request.QueryString["returnUrl"] != null)
                loginUrl += "?returnUrl=" + Request.QueryString["returnUrl"];
            if (Request.QueryString["returnUrl"] != null)
            {
                loginUrl += "&login_error=1";
            }
            else
            {
                loginUrl += "?login_error=1";
            }
            // end of data for error for presentation

            var ud = UserMng.GetUserByUsername(username);
            var sessionUd = Session["UserData"] as UserData;
            var ci = new CultureInfo(this.Language.ShortCode);

            if (ud == null || ud.Password != passwordHash)
            {
                string errorMsg = Resources.Resources.UI_InvalidLogin;
                Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);
                // return Content(errorMsg);

                return Redirect(loginUrl);
            }

            string requestIp = Request.UserHostAddress;

            if (sessionUd != null && sessionUd.Username != username)
            {
                Session.Abandon();
                return RedirectToAction("LoginUsernamePasswordHash", new { username = username, passwordHash = passwordHash });
            }

            if ((ud.AllowedIps == null || ud.AllowedIps.Count == 0) || UserMng.IsAllowedIpUsernameLogin(requestIp, ud.AllowedIps))
            {
                if (Session["UserData"] != null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Login process
                string errorMsg = "";
                ud = ProcessLogin(ud.Username, ud.Password, true, Request.UserHostAddress, this.Language.ShortCode, ref errorMsg);

                if (string.IsNullOrEmpty(errorMsg))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);
                    ViewBag.ErrorMsg = errorMsg;
                    //  return Content(errorMsg);
                    // return RedirectToAction("Index", "Login");
                    return Redirect(loginUrl);
                }
            }
            else
            {
                string errorMsg = Resources.Resources.ResourceManager.GetString("UI_IpNotAllowed", ci) + " " + requestIp;
                ViewBag.ErrorMsg = errorMsg;
                Stat.AddInvalidLogin(username, Request.UserHostAddress, errorMsg);

                // return Content(errorMsg);
                //  return RedirectToAction("Index", "Login");
                return Redirect(loginUrl);
            }
        }

        public ActionResult GetUserInformation(string username)
        {
            // Check request
            var requestHost = HttpContext.Request.Url.Host.ToString();
            var allowedIPs = ConfigurationManager.AppSettings["AllowedClientIPs"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string ip = Request.UserHostAddress;
            if ((requestHost != "localhost" && requestHost != "apis.bg" && requestHost != "web.apis.bg")
                && (ip != "192.168.2.137")
                && (ip != "194.8.4.254")
                && (ip != "87.121.111.213")
                && (ip != "87.121.111.198"))
            {
                return Content("Request address not authorized");
            }

            // Get user information
            var userInformationObj = UserDataRemote.GetUserInformationRemote(username);
            if (userInformationObj == null)
            {
                return Content("User information not found");
            }

            return Content(userInformationObj.GetInformationJSON());
        }

        [HttpGet]
        public ActionResult Activate(string token)
        {
            bool res = UserMng.ActivateUser(new Guid(token));
            return View("AfterActivation", res);
        }
    }
}
