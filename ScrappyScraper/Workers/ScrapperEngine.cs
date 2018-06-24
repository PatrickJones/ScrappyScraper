using AngleSharp;
using ScrappyScraper.ExtMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ScrappyScraper.Workers
{
    /// <summary>
    /// Class to scrape web pages from passed in Url using AngleSharp library to parse html document.
    /// </summary>
    public class ScrapperEngine
    {
        private readonly string scrapUrl;

        IConfiguration config;
        IBrowsingContext context;

        ConcurrentBag<string> ccImages = new ConcurrentBag<string>();
        ConcurrentBag<string> ccWords = new ConcurrentBag<string>();

        /// <summary>
        /// ScrapperEngine constructor.
        /// </summary>
        /// <param name="url">Web page URL to scrape</param>
        public ScrapperEngine(string url)
        {
            scrapUrl = url;
            config = AngleSharp.Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);
        }

        /// <summary>
        /// Gets the collection of images source values.
        /// </summary>
        /// <returns>IList<string></returns>
        public IList<string> GetImages()
        {
            return ccImages.ToList();
        }

        /// <summary>
        /// Gets the collection of words with corresponding occurrence count as a Dictionary.
        /// Key: word, Value: occurrence count
        /// </summary>
        /// <returns>IDictionary<string, int></returns>
        public IDictionary<string, int> GetWords()
        {
            if (ccWords.Count == 0)
            {
                return new Dictionary<string, int>();
            }

            var kv = from w in ccWords
                     group w by w into wds
                     select new KeyValuePair<string, int>(wds.Key, wds.Count());

            return kv.ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// Initializes the scrapping of html document from url passed in constructor.
        /// </summary>
        /// <returns>Task</returns>
        public async Task Scrap()
        {
            try
            {
                await context.OpenAsync(scrapUrl).ContinueWith(c => {
                    ScrapImages();
                    ScrapText();
                });
            }
            catch (Exception e)
            {
                throw new HttpParseException($"Error scrapping url: {scrapUrl}.", e);
            }
        }

        /// <summary>
        /// Scrapes active html document for text content. 
        /// </summary>
        private void ScrapText()
        {
            try
            {
                var elements = context.Active.Body.GetElementsByTagName("*").ToArray();

                if (elements.Length > 0)
                {
                    // loop though html elements and grab their text content then,
                    // apply "WordArray' extension method and add each array item to words collection
                    Parallel.ForEach(elements, a => {
                        var wArray = a.TextContent.WordArray();

                        for (int i = 0; i < wArray.Length; i++)
                        {
                            var wrd = wArray[i];
                            ccWords.Add(wrd);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                throw new HttpParseException($"Error scrapping text from url: {scrapUrl}.", e);
            }
        }

        /// <summary>
        /// Scrapes active html document for image tags and source values. 
        /// </summary>
        private void ScrapImages()
        {
            try
            {
                if (context.Active.Images != null && context.Active.Images.Count() > 0)
                {
                    // loop though html image elements and add their source (src) attribute to images collection
                    Parallel.ForEach(context.Active.Images.ToArray(), a => {
                        ccImages.Add(a.Source);
                    });
                }
            }
            catch (Exception e)
            {
                throw new HttpParseException($"Error scrapping images from url: {scrapUrl}.", e);
            }
        }
    }
}