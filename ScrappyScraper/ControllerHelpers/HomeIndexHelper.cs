using AngleSharp;
using ScrappyScraper.Models;
using ScrappyScraper.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ScrappyScraper.ControllerHelpers
{
    /// <summary>
    /// Home controller helper class to create and populate Home controller related viewmodels
    /// </summary>
    public class HomeIndexHelper
    {
        private readonly string url;
        private HomeViewModel vModel;

        /// <summary>
        /// HomeIndexHelper constructor.
        /// </summary>
        /// <param name="scrapUrl">Web page URL to scrape</param>
        public HomeIndexHelper(string scrapUrl)
        {
            url = scrapUrl;
            vModel = new HomeViewModel { CurrentUrl = scrapUrl };
        }

        /// <summary>
        /// Starts ScrapperEngine to generate and retrieve image and word collections
        /// </summary>
        /// <returns>Task</returns>
        public async Task StartScrapping()
        {
            // validate url entered by user, well at least up to a certain point
            if (String.IsNullOrEmpty(url))
            {
                vModel.ErrorMessage = "Please enter valid Url.";
            }
            else if (!url.StartsWith("http"))
            {
                vModel.ErrorMessage = "Url must be valid 'http://' or 'https://'.";
            }
            else
            {
                try
                {
                    var engine = new ScrapperEngine(url);
                    // start scrapping
                    await engine.Scrap().ContinueWith(c =>
                    {
                        vModel.ImageSources = engine.GetImages();
                        // aphabetically ordered list of words
                        vModel.WordDictionary = engine.GetWords().OrderBy(o => o.Key).ToDictionary(k => k.Key, v => v.Value);
                    });
                }
                catch
                {
                    vModel.ErrorMessage = "Unable to scrape Url.";
                }
            }
        }

        /// <summary>
        /// Gets generated viewmodel.
        /// </summary>
        /// <returns>HomeViewModel</returns>
        public HomeViewModel GetViewModel()
        {
            return vModel;
        }
    }
}