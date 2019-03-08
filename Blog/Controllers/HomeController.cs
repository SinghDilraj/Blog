using Blog.Models.Classes;
using Blog.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            BlogPost post = new BlogPost
            {
                Title = model.Title,
                Body = model.Body,
                ImageURL = model.ImageURL
            };

            return RedirectToAction(nameof(HomeController.Index));
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}