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
                     Id = post.Id,
                     Title = post.Title,
                     Body = post.Body,
                     Published = post.Published,
                     ImageUrl = post.Image,
                     DateCreated = post.DateCreated,
                     DateUpdated = post.DateUpdated

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
            post.DateUpdated = model.DateUpdated;

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

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(
                p => p.Id == id && p.UserId == userId);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            post.DateUpdated = DateTime.Now;

            var model = new CreateHomeViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.ImageUrl = post.Image;
            model.DateUpdated = post.DateUpdated;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, CreateHomeViewModel model)
        {
            model.DateUpdated = DateTime.Now;
            return SavePost(id, model);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(
                p => p.Id == id && p.UserId == userId);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            DbContext.BlogPosts.Remove(post);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(HomeController.Index));
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(p =>
            p.Id == id.Value &&
            p.UserId == userId);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var model = new CreateHomeViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.ImageUrl = post.Image;

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Blog.Models.Classes.EmailService contact = new Blog.Models.Classes.EmailService();

            contact.Send("dilrajkehra@gmail.com", "Contact Form Blog Website", $"From => Name => {model.Name} Email => {model.Email}, {model.Message}");

            ModelState.AddModelError(string.Empty, "Thank You for Your Message.");
            return View(model);
        }
    }
}