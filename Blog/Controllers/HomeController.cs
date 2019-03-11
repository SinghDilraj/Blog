using Blog.Models;
using Blog.Models.Classes;
using Blog.Models.ViewModels;
using Microsoft.AspNet.Identity;
using MovieDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext DbContext;
        public HomeController()
        {
            DbContext = new ApplicationDbContext();
            
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var postsQuery =
                (from post in DbContext.BlogPosts
                 where post.UserId == userId
                 select new CreateHomeViewModel
                 {
                     Title = post.Title,
                     Body = post.Body,
                     Published = post.Published
                     //Image = post.Image
                 }).ToList();

            return View(postsQuery);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateHomeViewModel model)
        {
            return SavePost(null, model);
        }

        private ActionResult SavePost(int? id, CreateHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.GetUserId();

            if (DbContext.BlogPosts.Any(p => p.UserId == userId &&
            p.Title == model.Title &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(CreateHomeViewModel.Title),
                    "Post Title should be unique");

                return View();
            }

            string fileExtension;

            //Validating file upload
            if (model.Image != null)
            {
                fileExtension = Path.GetExtension(model.Image.FileName);

                if (!ImageHelper.AllowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError(nameof(CreateHomeViewModel.Image), "File extension is not allowed.");
                    return View();
                }
            }

            BlogPost post;

            if (!id.HasValue)
            {
                post = new BlogPost();
                post.UserId = userId;
                DbContext.BlogPosts.Add(post);
            }
            else
            {
                post = DbContext.BlogPosts.FirstOrDefault(p => p.Id == id && p.UserId == userId);

                if (post == null)
                {
                    return RedirectToAction(nameof(HomeController.Index));
                }
            }

            post.Title = model.Title;
            post.Body = model.Body;
            post.Published = model.Published;

            //Handling file upload
            if (model.Image != null)
            {
                if (!Directory.Exists(ImageHelper.MappedUploadFolder))
                {
                    Directory.CreateDirectory(ImageHelper.MappedUploadFolder);
                }

                var fileName = model.Image.FileName;
                var fullPathWithName = ImageHelper.MappedUploadFolder + fileName;

                model.Image.SaveAs(fullPathWithName);

                post.Image = ImageHelper.UploadFolder + fileName;
            }

            DbContext.SaveChanges();

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