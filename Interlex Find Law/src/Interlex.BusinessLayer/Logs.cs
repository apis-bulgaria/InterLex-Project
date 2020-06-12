namespace Interlex.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.BusinessLayer.Models;
    using System.IO;
    using System.Configuration;

    public class Logs
    {
        public static List<LogError> GetAllSimpleErrors()
        {
            var logsDirectory = ConfigurationManager.AppSettings["LogsFolderPath"];

            var logErrorList = new List<LogError>();
            string[] fileEntries = Directory.GetFiles(logsDirectory);
            for (int i = fileEntries.Length - 1; i >= 0 ; i--)
            {
                var fileContent = File.ReadAllText(fileEntries[i]);

                var logModel = new LogError();
                logModel.LogContent = fileContent.Replace("--", "<hr/> --");

                var logsCount = fileContent.Count(f => f == '-');
                logModel.LogsCount = logsCount/2;

                var dateIndex = fileEntries[i].LastIndexOf("\\") + 1;
                var realFileNme = fileEntries[i].Substring(dateIndex, fileEntries[i].Length - dateIndex);

                if (realFileNme == "perf.log")
                {
                    continue;
                }

                logModel.Date = realFileNme.Replace(".txt", "");

                logErrorList.Add(logModel);
            }

            return logErrorList;
        }

        public static List<LogError> GetAllSearchErrors()
        {
            var logsDirectory = ConfigurationManager.AppSettings["LogsFolderPath"] + "\\Searches";

            var logErrorList = new List<LogError>();
            string[] fileEntries = Directory.GetFiles(logsDirectory);
            for (int i = fileEntries.Length - 1; i >= 0; i--)
            {
                var fileContent = File.ReadAllText(fileEntries[i]);

                var logModel = new LogError();
                logModel.LogContent = fileContent.Replace("\r\n", "<br/>");

               // var logsCount = fileContent.Count(f => f == '-');
               // logModel.LogsCount = logsCount / 2;

                var dateIndex = fileEntries[i].LastIndexOf("\\") + 1;
                var realFileNme = fileEntries[i].Substring(dateIndex, fileEntries[i].Length - dateIndex);

                logModel.Date = realFileNme.Replace(".txt", "");

                logErrorList.Add(logModel);
            }

            return logErrorList;
        }

        public static List<LogError> GetAllAdministrationErrors()
        {
            var logsDirectory = ConfigurationManager.AppSettings["LogsFolderPathAdministration"];

            var logErrorList = new List<LogError>();
            string[] fileEntries = Directory.GetFiles(logsDirectory);
            for (int i = fileEntries.Length - 1; i >= 0; i--)
            {
                var fileContent = File.ReadAllText(fileEntries[i]);

                var logModel = new LogError();
                logModel.LogContent = fileContent.Replace("--", "<hr/> --");

                var logsCount = fileContent.Count(f => f == '-');
                logModel.LogsCount = logsCount / 2;

                var dateIndex = fileEntries[i].LastIndexOf("\\") + 1;
                var realFileNme = fileEntries[i].Substring(dateIndex, fileEntries[i].Length - dateIndex);

                if (realFileNme == "perf.log")
                {
                    continue;
                }

                logModel.Date = realFileNme.Replace(".txt", "");

                logErrorList.Add(logModel);
            }

            return logErrorList;
        }

        public static List<LogError> GetAllPresentationSiteErrors()
        {
            var logsDirectory = ConfigurationManager.AppSettings["LogsFolderPathSite"];

            var logErrorList = new List<LogError>();
            string[] fileEntries = Directory.GetFiles(logsDirectory);
            for (int i = fileEntries.Length - 1; i >= 0; i--)
            {
                var fileContent = File.ReadAllText(fileEntries[i]);

                var logModel = new LogError();
                logModel.LogContent = fileContent.Replace("--", "<hr/> --");

                var logsCount = fileContent.Count(f => f == '-');
                logModel.LogsCount = logsCount / 2;

                var dateIndex = fileEntries[i].LastIndexOf("\\") + 1;
                var realFileNme = fileEntries[i].Substring(dateIndex, fileEntries[i].Length - dateIndex);

                if (realFileNme == "perf.log")
                {
                    continue;
                }

                logModel.Date = realFileNme.Replace(".txt", "");

                logErrorList.Add(logModel);
            }

            return logErrorList;
        }
    }
}
