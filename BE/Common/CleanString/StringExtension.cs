using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common.StringEx
{
    public class StringExtension
    {
        public static string CleanString(string model)
        {
            if (model.Trim() == "")
            {
                return null;
            }
            string currentString = "";
            currentString = model.Trim();
            currentString = Regex.Replace(currentString, @"\s+", " ");

            return currentString;
        }
    }
}
