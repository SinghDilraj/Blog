﻿using Blog.Models;
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
using System.Text;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext DbContext;
        public HomeController()
        {
            DbContext = new ApplicationDbContext();
            
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            List<CreateHomeViewModel> postsQuery;

            if (User.IsInRole("Admin"))
            {
                postsQuery =
                (from post in DbContext.BlogPosts
                 select new CreateHomeViewModel
                 {
                     Id = post.Id,
                     Title = post.Title,
                     Body = post.Body,
                     Published = post.Published,
                     ImageUrl = post.Image,
                     Slug = post.Slug,
                     DateCreated = post.DateCreated,
                     DateUpdated = post.DateUpdated

                 }).ToList();
            }
            else
            {
                postsQuery =
                (from post in DbContext.BlogPosts
                 where post.Published == true
                 select new CreateHomeViewModel
                 {
                     Id = post.Id,
                     Title = post.Title,
                     Body = post.Body,
                     Published = post.Published,
                     ImageUrl = post.Image,
                     Slug = post.Slug,
                     DateCreated = post.DateCreated,
                     DateUpdated = post.DateUpdated

                 }).ToList();
            }

            return View(postsQuery);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(IndexHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(nameof(IndexHomeViewModel.Query),
                    "Please enter a search query");
                return RedirectToAction(nameof(HomeController.Index));
            }

            List<CreateHomeViewModel> query;

            if (User.IsInRole("Admin"))
            {
                query = (from post in DbContext.BlogPosts
                            where post.Title.ToLower().Contains(model.Query.ToLower()) || post.Body.ToLower().Contains(model.Query.ToLower()) || post.Slug.ToLower().Contains(model.Query.ToLower())
                            select new CreateHomeViewModel
                            {
                                Id = post.Id,
                                Title = post.Title,
                                Body = post.Body,
                                Published = post.Published,
                                ImageUrl = post.Image,
                                Slug = post.Slug,
                                DateCreated = post.DateCreated,
                                DateUpdated = post.DateUpdated

                            }).ToList();
            }
            else
            {
                query = (from post in DbContext.BlogPosts
                            where post.Title.ToLower().Contains(model.Query.ToLower()) || post.Body.ToLower().Contains(model.Query.ToLower()) || post.Slug.ToLower().Contains(model.Query.ToLower()) && post.Published == true
                        select new CreateHomeViewModel
                        {
                            Id = post.Id,
                            Title = post.Title,
                            Body = post.Body,
                            Published = post.Published,
                            ImageUrl = post.Image,
                            Slug = post.Slug,
                            DateCreated = post.DateCreated,
                            DateUpdated = post.DateUpdated

                        }).ToList();
            }

            return View("SearchResults", query);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SearchResults(List<CreateHomeViewModel> PostQuery)
        {
            return View(PostQuery);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(CreateHomeViewModel model)
        {
            return SavePost(null, model);
        }

        [Authorize(Roles = "Admin")]
        private ActionResult SavePost(int? id, CreateHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (DbContext.BlogPosts.Any(p => p.Title == model.Title &&
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
                DbContext.BlogPosts.Add(post);
            }
            else
            {
                post = DbContext.BlogPosts.FirstOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return RedirectToAction(nameof(HomeController.Index));
                }
            }

            post.Title = model.Title;
            post.Body = model.Body;
            post.Published = model.Published;
            post.Slug = ToUrlSlug(model.Title);
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

        private string ToUrlSlug(string value)
        {
            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(
                p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            post.DateUpdated = DateTime.Now;

            var model = new CreateHomeViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.Slug = post.Slug;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(int id, CreateHomeViewModel model)
        {
            model.DateUpdated = DateTime.Now;
            return SavePost(id, model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var post = DbContext.BlogPosts.FirstOrDefault(
                p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            DbContext.BlogPosts.Remove(post);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(HomeController.Index));
        }

        [AllowAnonymous]
        [Route("Blog/{slug}")]
        public ActionResult Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            BlogPost post;

            if (User.IsInRole("Admin"))
            {
                post = DbContext.BlogPosts.FirstOrDefault(p =>
            p.Slug == slug);
            }
            else
            {
                post = DbContext.BlogPosts.FirstOrDefault(p =>
            p.Slug == slug && p.Published == true);
            }

            

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

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
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