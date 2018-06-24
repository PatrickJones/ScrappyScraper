using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ScrappyScraper.ExtMethods
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// String extension method to create array of word from passed in string.
        /// </summary>
        /// <param name="str">string used to create word array</param>
        /// <returns>string[]</returns>
        public static string[] WordArray(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return new string[] { };
            }

            ConcurrentBag<string> wBag = new ConcurrentBag<string>();
            var cleanStr = str;

            // remove all symbols and numerics except hyphen, periods, apostrophy
            var charSym = new List<char> { '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '[', ']', '{', '}', '|', '\\', '?', '/', '<', '>', '>', ',', ';', ':', '"','0','1','2','3','4','5','6','7','8','9' };
            // remove escape sequences
            var charSpec = new List<string> { "\n", "\t", "\r" };

            // loop to replace unwanted characters and esacpe sequences
            for (int i = 0; i < charSym.Count; i++)
            {
                cleanStr = cleanStr.Replace(charSym[i], ' ');

                for (int j = 0; j < charSpec.Count; j++)
                {
                    cleanStr = cleanStr.Replace(charSpec[j], "");
                }
            }

            // Split on spaces and keep words with length greater the 1 with exception for 'a' and 'I' (could techically be considered words!)
            var wArray = cleanStr.Split(' ').Where(w => w.Length > 1 || w.ToUpper() == "A" || w == "I").ToArray();

            // loop through each word in array removing empty strings and periods from words that end a sentence
            // but allowing words that have period (dot) within them (i.e ASP.NET)
            Parallel.ForEach(wArray, a => {
                if (!String.IsNullOrEmpty(a))
                {
                    a.Trim();
                    if (a.Last() == '.')
                    {
                        var idx = a.LastIndexOf('.');
                        wBag.Add(a.Remove(idx, 1));
                    }
                    else
                    {
                        wBag.Add(a);
                    }
                }
            });

            return wBag.ToArray();
        }
    }
}