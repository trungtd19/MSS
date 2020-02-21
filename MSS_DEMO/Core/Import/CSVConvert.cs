using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MSS_DEMO.Core.Import
{
    public class CSVConvert
    {
        public string RemoveCSVQuotes(string value)
        {
            if (value == @"""""") value = "";
            value = value.Replace(@"""""", @"""");
            if (value.Length >= 2)
                if (value.Substring(0, 1) == @"""")
                    value = value.Substring(1, value.Length - 2);
            return value;
        }
        public List<string> RegexRow(StreamReader st)
        {
            return Regex.Matches(st.ReadLine(), @"\A[^,]*(?=,)|(?:[^"",]*""[^""]*""[^"",]*)+|[^"",]*""[^""]*\Z|(?<=,)[^,]*(?=,)|(?<=,)[^,]*\Z|\A[^,]*\Z")
                       .Cast<Match>()
                       .Select(m => RemoveCSVQuotes(m.Value).Replace('|', '¦'))
                       .ToList<string>();
        }
        public string AddCSVQuotes(string item)
        {
            if (item.Contains(","))
            {
                item = @"""" + item + @"""";
            }
            return item;
        }
    }
}