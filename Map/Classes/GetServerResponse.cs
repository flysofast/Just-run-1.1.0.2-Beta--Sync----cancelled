using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map
{
    class ServerResponse
    {
        public static string GetMessage(WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            string result = e.Result.ToString();
            string startString = "<JustRunServerResponse>";
            int start = result.IndexOf(startString) + startString.Length;
            int length = result.IndexOf(("</JustRunServerResponse>")) - start;
            if (length > 0)
                return result.Substring(start, length);
            else
                return "";
        }

        public static string GetResult(WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            string result = e.Result.ToString();
            string startString = "<JustRunServerResult>";
            int start = result.IndexOf(startString) + startString.Length;
            int length = result.IndexOf(("</JustRunServerResult>")) - start;
            if (length > 0)
                return result.Substring(start, length);
            else
                return "";
        }
    }
}
