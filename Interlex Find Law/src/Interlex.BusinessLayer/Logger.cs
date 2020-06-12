namespace Interlex.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.BusinessLayer.Models;
    using Interlex.BusinessLayer.Enums;
    using System.IO;

    public class Logger
    {
        public static void LogExceptionToFolder(string basePath, UserData ud, Exception ex, string moreInfo)
        {
            string userInfo = "";
            if (ud != null)
            {
                userInfo = "-- " + ud.Username + " (" + ud.UserId + ")";
            }

            string logsPath = basePath + "Logs\\";

            string errorMsg = DateTime.Now.ToString("dd.MM.yyyy H:mm:ss") + "-- " + userInfo + Environment.NewLine + "Message: " + ex.Message + "|| Stack Trace: " + ex.StackTrace + "|| Target: " + ex.TargetSite + Environment.NewLine + "MORE INFO: " + moreInfo
             + Environment.NewLine + Environment.NewLine;

            var timeOfLoging = DateTime.Now;
            var timePathParsed = "";

            if (timeOfLoging.Day.ToString().Length == 1)
            {
                timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_0" + timeOfLoging.Day;
            }
            else
            {
                timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_" + timeOfLoging.Day;
            }

            var fullPath = logsPath + timePathParsed + ".txt";

            //File.Create(fullPath);
            File.AppendAllText(fullPath, errorMsg + Environment.NewLine);
        }

        public static void LogExceptionToFolder(string basePath, UserData ud, Exception ex)
        {
            LogExceptionToFolder(basePath, ud, ex, String.Empty);
        }

        public static void LogExceptionToEventViewer(UserData ud, Exception ex, string appLabel)
        {
            string userInfo = "";
            if (ud != null)
            {
                userInfo = "-- " + DateTime.Now.ToString() + " " + ud.Username + " (" + ud.UserId + ")";
            }

            string errorMsg = userInfo + Environment.NewLine + "Message: " + ex.Message + "|| Stack Trace: " + ex.StackTrace + "|| Target: " + ex.TargetSite
             + Environment.NewLine + Environment.NewLine;

            System.Diagnostics.EventLog.WriteEntry(appLabel, errorMsg, System.Diagnostics.EventLogEntryType.Error);
        }

        public static void LogApplicationUpdate(string basePath, ApplicationUpdateType updateType, bool passed, string ip)
        {
            string loggingMessage = "";
            if (updateType == ApplicationUpdateType.FTI)
            {
                if (passed)
                {
                    loggingMessage = "FTI Updated successfuly";
                }
                else
                {
                    loggingMessage = "Error updating FTI";
                }
            }
            else if (updateType == ApplicationUpdateType.DocStruct)
            {
                if (passed)
                {
                    loggingMessage = "DocStruct updated successfuly";
                }
                else
                {
                    loggingMessage = "Error updating DocStruct";
                }
            }
            else if (updateType == ApplicationUpdateType.Classifiers)
            {
                if (passed)
                {
                    loggingMessage = "Classifiers updated successfuly";
                }
                else
                {
                    loggingMessage = "Error updating classifiers";
                }
            }

            loggingMessage = loggingMessage + " " + ip;

            LogApplicationUpdateMessage(basePath, loggingMessage);

            //string logsPath = basePath + "Logs\\";
            //var fullPath = logsPath + "updates.txt";
            //File.AppendAllText(fullPath, loggingMessage + Environment.NewLine);
        }

        public static void LogApplicationUpdateMessage(string basePath, string message)
        {
            string logsPath = basePath + "Logs\\";
            var fullPath = logsPath + "updates.txt";
            File.AppendAllText(fullPath, DateTime.Now.ToString("dd.MM.yyyy H:mm:ss") + " " + message + Environment.NewLine);
        }

        public static void LogDemoDocs(string basePath, string error, string ip, List<int> docLangIds, List<string> docIdentifiers)
        {
            StringBuilder loggingContent = new StringBuilder();
            string logsPath = basePath + "Logs\\Blog_Docs\\";

            var timeOfLoging = DateTime.Now;
            var timePathParsed = "";

            if (timeOfLoging.Day.ToString().Length == 1)
            {
                timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_0" + timeOfLoging.Day;
            }
            else
            {
                timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_" + timeOfLoging.Day;
            }

            var fullPath = logsPath + timePathParsed + ".txt";
          
            if (String.IsNullOrEmpty(error))
            {
                loggingContent.Append(Environment.NewLine);
                loggingContent.Append("Demo docs successfuly added");
                loggingContent.Append(Environment.NewLine);
                loggingContent.Append("Ip: " + ip);
                loggingContent.Append(Environment.NewLine);
                loggingContent.Append("Original doc lang ids: ");
                loggingContent.Append(Environment.NewLine);
                loggingContent.Append(String.Join("; ", docLangIds));
                loggingContent.Append(Environment.NewLine);
                loggingContent.Append("Added doc identifiers: ");
                loggingContent.Append(String.Join("; ", docIdentifiers));

                File.AppendAllText(fullPath, loggingContent.ToString() + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(fullPath, error + Environment.NewLine);
            }
        }
    }
}
