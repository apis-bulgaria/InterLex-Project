using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Interlex.BusinessLayer;

namespace Interlex.App.Controllers
{
    public class SitemapController : BaseController
    {

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GenerateDocSitemap()
        {
            string ip = Request.UserHostAddress;
            const string serverIp = "87.121.111.212";
            if (ip == serverIp || ip == "127.0.0.1" || ip == "::1")
            {
                var links = Sitemap.GetDocLinksSitemap();
                string pathToProject = Server.MapPath("~");
                string folderName = pathToProject + "Sitemaps\\";
                const int chunkSize = 50000;
                double iterations = Math.Ceiling((double)links.Count / chunkSize);
                string sitemapXmlName = "sitemap";

                var xmlSitemapIndexSb = new StringBuilder();
                xmlSitemapIndexSb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                xmlSitemapIndexSb.Append("<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

                for (int i = 0; i < Convert.ToInt32(iterations); i++)
                {
                    var xmlSitemapSb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    xmlSitemapSb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"> ");

                    var currentLinks = links.Skip(i * chunkSize).Take(chunkSize).ToList();
                    for (int j = 0; j < currentLinks.Count; j++)
                    {
                        xmlSitemapSb.Append("<url><loc>");
                        xmlSitemapSb.Append(currentLinks[j]);
                        xmlSitemapSb.Append("</loc></url>");
                    }

                    xmlSitemapSb.Append("</urlset>");
                    var filename = sitemapXmlName + (i + 1) + ".xml";
                    System.IO.File.WriteAllText(folderName + filename, xmlSitemapSb.ToString());

                    xmlSitemapIndexSb.Append("<sitemap>");
                    xmlSitemapIndexSb.Append("<loc>");
                    xmlSitemapIndexSb.Append("http://freecases.eu/Sitemaps/" + filename);
                    xmlSitemapIndexSb.Append("</loc>");
                    xmlSitemapIndexSb.Append("</sitemap>");
                }

                xmlSitemapIndexSb.Append("</sitemapindex>");
                System.IO.File.WriteAllText(folderName + "sitemap-index.xml", xmlSitemapIndexSb.ToString());
                string url = "http://www.google.com/ping?sitemap=http://freecases.eu/Sitemaps/sitemap-index.xml";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return new HttpStatusCodeResult(200);
            }

            return new HttpNotFoundResult();
        }
    }
}