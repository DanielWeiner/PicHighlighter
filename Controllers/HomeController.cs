using PictureThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PictureThing.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index(string whatNow)
        {
            ViewBag.Error = whatNow;
            return View();
        }

        [HttpPost]
        public ActionResult Picture(HttpPostedFileBase file, string text, int winWidth, double opacity, bool transpText)
        {
            if (file != null && file.ContentType.ToLower().Contains("image"))
            {
                string browser = Regex.Replace(Request.Browser.Type, "[0-9]", "").ToLower();
                var VM = new UploadedImage(file, text, winWidth, opacity, transpText, browser);

                return View(VM);

            }
            return Redirect("Index/Error");
        }

    }
}
