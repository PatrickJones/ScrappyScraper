using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScrappyScraper.Models
{
    /// <summary>
    /// Home controller view model
    /// </summary>
    public class HomeViewModel
    {
        // Key: word, Value: word occurrence count
        public IDictionary<string, int> WordDictionary = new Dictionary<string, int>();
        public IList<string> ImageSources = new List<string>();

        public string CurrentUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}