using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common.StringEx
{
    public class StringExtension
    {
        // Fix: Replace 'GeneratedRegex' with 'new Regex' to resolve CS0103 error  
        private static readonly Regex SpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public static string CleanString(string model)
        {
            if (model == null)
            {
                return "";
            }
            if (model.Trim() == "")
            {
                return model;
            }
            string currentString = "";
            currentString = model.Trim();
            currentString = SpaceRegex.Replace(currentString, " ");

            return currentString;
        }
    }
}
