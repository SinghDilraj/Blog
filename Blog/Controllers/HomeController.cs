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
            post.UserId = User.Identity.GetUserId();
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
            model.DateCreated = post.DateCreated;
            model.DateUpdated = post.DateUpdated;
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

        [Route("Blog/{slug}")]
        [HttpGet]
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
            model.Id = post.Id;
            model.Published = post.Published;
            model.ImageUrl = post.Image;
            model.Comments = post.Comments;
            model.DateCreated = post.DateCreated;
            model.DateUpdated = post.DateUpdated;
            model.Slug = post.Slug;
            return View(model);
        }

        [Authorize]
        [Route("Blog/{slug}")]
        [HttpPost]
        public ActionResult Comment(int? id, CommentHomeViewModel model)
        {
            if (!id.HasValue || !ModelState.IsValid)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            Comment comment = new Comment
            {
                Body = model.Body,
                UserId = User.Identity.GetUserId(),
                BlogPostId = id.Value
            };

            BlogPost post = DbContext.BlogPosts.FirstOrDefault(p => p.Id == id.Value);

            post.Comments.Add(comment);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(HomeController.Details), new { slug = post.Slug });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public ActionResult EditComment(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(
                p => p.Id == id);

            if (comment == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var model = new CommentHomeViewModel();
            model.Body = comment.Body;
            model.BlogPostId = comment.BlogPostId;
            model.ModifyingReason = comment.ModifyingReason;
            model.UserId = comment.UserId;
            model.Id = comment.Id;
            model.DateUpdated = DateTimeOffset.Now;
            return View("CommentEdit", model);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult EditComment(int? id, CommentHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("_Comment", model);
            }

            Comment comment;

            if (!id.HasValue)
            {
                comment = new Comment();
                DbContext.Comments.Add(comment);
            }
            else
            {
                comment = DbContext.Comments.FirstOrDefault(p => p.Id == id);

                if (comment == null)
                {
                    return RedirectToAction(nameof(HomeController.Index));
                }
            }

            comment.Body = model.Body;
            comment.DateUpdated = model.DateUpdated;
            comment.Id = model.Id;
            comment.UserId = model.UserId;
            comment.BlogPostId = model.BlogPostId;
            comment.ModifyingReason = model.ModifyingReason;

            DbContext.SaveChanges();

            return RedirectToAction(nameof(HomeController.Details), new { slug = DbContext.BlogPosts.FirstOrDefault(p => p.Id == comment.BlogPostId).Slug });
        }

        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteComment(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(
                p => p.Id == id);

            if (comment == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            DbContext.Comments.Remove(comment);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(HomeController.Details), new { slug = DbContext.BlogPosts.FirstOrDefault(p => p.Id == comment.BlogPostId).Slug });
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