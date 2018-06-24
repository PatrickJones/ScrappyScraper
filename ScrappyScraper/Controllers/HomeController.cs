using ScrappyScraper.ControllerHelpers;
using ScrappyScraper.ExtMethods;
using ScrappyScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ScrappyScraper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string scrapUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ScrapAsync(string scrapUrl)
        {
            var helper = new HomeIndexHelper(scrapUrl);
            await helper.StartScrapping();

            return View(helper.GetViewModel());
        }
    }
}