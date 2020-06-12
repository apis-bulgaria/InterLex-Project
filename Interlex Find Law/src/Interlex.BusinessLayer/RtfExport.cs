namespace Interlex.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RtfExport
    {
        public static void Export(string htmlPath, string licenseKey)
        {
            // Convert HTML string to RTF file.
            // If you need more information about HTML-to-RTF Pro DLL .Net email us at:
            // support[at]sautinsoft.com			
            SautinSoft.HtmlToRtf h = new SautinSoft.HtmlToRtf();

            // Please insert your serial number here after purchasing the component
            h.Serial = licenseKey;

            string htmlString = "";
            string rtfPath = Path.ChangeExtension(htmlPath, ".rtf");

            // Get HTML string from a file.
            htmlString = File.ReadAllText(htmlPath);

            // Set 'BaseUrl' that component can find a full path to .css and images
            h.BaseURL = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(htmlPath));

            //h.PreserveFontColor = false;
            h.PreserveBackgroundColor = false;
            h.PreserveFontSize = true;

            //h.FontSize = 12;

            //SautinSoft.HtmlToRtf.CFont

            // Invoke the method to convert HTML string into RTF file.
            int ret = h.ConvertStringToFile(htmlString, rtfPath);

            // 0 - converting successfully
            // 1 - can't open input file or URL, check the input path
            // 2 - can't create output file, check the output path
            // 3 - converting failed
            /*if (ret == 0)
            {
                // Show produced RTF file
                System.Diagnostics.Process.Start(rtfPath);
            }*/
        }
    }
}
