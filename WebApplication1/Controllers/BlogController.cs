using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project1.DAL;
using Project1.Models;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Helpers;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using WebApplication1.Models;
using WebApplication1.Classes;

namespace Project1.Controllers
{
    /*****************************
     * Not deleting the content if a file already exists
     * Host on website
     * Setup database on website
     * XX Deleting commentings when deleting post
     * XX Password requirments in model
     * XX Making so only admins can access the buttons
     * XX Post a comment
     * Update font
     * Fix little css problems
     * Fix blog listing view
     * Decide what to do when no file is uploaded
     * Proofread content
     * XXX Master account to post blog
     * XXX Posting a new blog
     * XXX Edit blogs
     * XXX Delete blogs
     * XXX Seperating by a return carriage
     * XXX Uploading image
     * XXX Saving the image name
     * XXX Fix the database
     * XXX Save Google username to database
     * XXX Register new user to database
     * XXX Encrypt password
     * ***************************/
    //this controller handles all blog logic
    // [Authorize(Users = "dreandesigns@me.com, patrikdrean@gmail.com")]



    public class BlogController : Controller
    {


        private MagicFeetContext db = new MagicFeetContext();
        
        // main blog landing page
        public ActionResult Index()
        {
            ViewBag.UserName = User.Identity.GetUserName();

            // Load up blog list to display

            List<Blog> BlogList = db.Blogs.OrderByDescending(x => x.Date).ToList();
            List<BlogDisplay> BlogDisplayList = new List<BlogDisplay>();
            int iCount = 0;

            foreach(var BlogItem in BlogList)
            {
                BlogDisplayList.Add(new BlogDisplay());

                BlogDisplayList.ElementAt(iCount).BlogID = BlogItem.BlogID;
                BlogDisplayList.ElementAt(iCount).Date = BlogItem.Date;
                BlogDisplayList.ElementAt(iCount).HeaderImage = BlogItem.HeaderImage;
                BlogDisplayList.ElementAt(iCount).Title = BlogItem.Title;

                // Load up all the content and stick it into model

                var contentList = db.Database.SqlQuery<Content>("SELECT * FROM Contents WHERE BlogID ='" + BlogItem.BlogID + "'").ToList();
                string contentToDisplay = "";


                foreach (var contentItem in contentList)
                {
                    contentToDisplay += contentItem.Text.ToString();
                    contentToDisplay += "\r\n";
                }


                BlogDisplayList.ElementAt(iCount).Text= Utility.TruncateAtWord(contentToDisplay, 250);

                iCount++;
            }

           



            return View(BlogDisplayList);
        }


        // individual blog postings
        public ActionResult Post(int BlogID)
        {
            PostComments pc = new PostComments();

            pc.Content = db.Database.SqlQuery<Content>("SELECT * FROM Contents WHERE BlogID = " + BlogID + " ORDER BY ContentID").ToList();
            pc.Comment = db.Database.SqlQuery<Comment>("SELECT * FROM Comments WHERE BlogID = " + BlogID + " ORDER BY CommentID").ToList();
            pc.Blog = db.Database.SqlQuery<Blog>("SELECT * FROM Blogs WHERE BlogID = " + BlogID).First();

           return View(pc);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Creator([Bind(Include = "ContentID,Text,BlogID")] Content content, List<String> jeff)
        {
            content.BlogID = db.Blogs.Max(p => p.BlogID) + 1;

            try
            {
                foreach (String item in jeff)
                {
                    content.ContentID = db.Contents.Max(p => p.ContentID) + 1;
                    content.Text = item;

                    if (ModelState.IsValid)
                    {
                        db.Contents.Add(content);
                        db.SaveChanges();
                    }
                }
            }catch(NullReferenceException e)
            {

            }

            if (jeff.Count > 0)
            {
                return View("Index");
            }


            return View(content);
        }*/

        //temporary blog creation page
        [Authorize(Users = "dreandesigns@me.com, patrikdrean@gmail.com, Patrik, Lauri Drean, Lauri, dreandesigns")]
        public ActionResult Creator()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Creator(FormCollection form, HttpPostedFileBase postedFile)
        {
            // Check to make sure a file isn't already stored with the same name
            Blog CheckExistingFileBlog = null;

            if (postedFile != null)
            {
                CheckExistingFileBlog = db.Blogs.SingleOrDefault(x => x.HeaderImage == postedFile.FileName);
            }

            if (CheckExistingFileBlog == null)
            {
                // Assign the blog object all of its attributes
                Blog blog = new Blog();
                if (db.Blogs.Max(x => x.BlogID) > 0)
                {
                    blog.BlogID = db.Blogs.Max(x => x.BlogID) + 1;
                }
                else
                {
                    blog.BlogID = 1;
                }

                blog.Date = Convert.ToDateTime(form["inputDate"]);
                blog.Title = form["inputTitle"];

                if (postedFile != null)
                {
                    blog.HeaderImage = postedFile.FileName;
                }
              /*  else
                {
                    blog.HeaderImage = "Lauri.jpg";
                } */


                // Upload the image to the images folder to be recalled later

                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Content/Images/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));
                }

                // Save blog to the database
                db.Blogs.Add(blog);
                db.SaveChanges();

                // Split the content inputed into an array in order to save it to the database
                var formContent = form["inputContent"];
                string[] contentList = formContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int iContentCounter;

                // Add each content piece to the database
                if (db.Contents.ToList().Count > 0)
                {
                    iContentCounter = db.Contents.Max(x => x.ContentID);
                }
                else
                {
                    iContentCounter = 0;
                }
                foreach (var blogContent in contentList)
                {
                    iContentCounter++;

                    Content content = new Content();
                    content.BlogID = blog.BlogID;
                    content.ContentID = iContentCounter;
                    content.Text = blogContent;

                    // Save changes to database
                    db.Contents.Add(content);
                    db.SaveChanges();
                }

                // Redirect to all blog posts
                return RedirectToAction("Index");
            }

            // Return to the view if a file already exists with the same name
            else
            {
                ViewBag.FileAlreadyExists = "A file already exists with this name.";
                ViewBag.BlogTitle = form["inputBlogTitle"];
                ViewBag.Content = form["inputContent"];
                return View();
            }
        }
        // parses blog postings into blog content
        public List<String> ParseBlogPost(String input)
        {
            List<String> jeff = input.Split('\n').ToList();

            return (jeff);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Comment([Bind(Include = "CommentID, Text, UserName, BlogID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                if (db.Comments.ToList().Count > 0)
                {
                    comment.CommentID = db.Comments.Max(p => p.CommentID) + 1;
                }
                else
                {
                    comment.CommentID = 0;
                }
                comment.UserName = User.Identity.GetUserName();

                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Post", new { BlogID = comment.BlogID.Value });
        }

        /**************************** Delete Action Method *****************************/
        [Authorize(Users = "dreandesigns@me.com, patrikdrean@gmail.com, Patrik, Lauri Drean, Lauri, dreandesigns")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ID = id;

            return View();
        }

  
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(FormCollection form)
        {
            // Grab the blogId to first delete all the content associated and then the actual blog information
            int blogID = Int32.Parse(form["id"]);
            var contentList = db.Database.SqlQuery<Content>("SELECT * FROM Contents WHERE BlogID ='" + blogID + "'").ToList();

            foreach (var contentItem in contentList)
            {
                Content content = db.Contents.Find(contentItem.ContentID);

                db.Contents.Remove(content);
                db.SaveChanges();
            }

            var commentList = db.Database.SqlQuery<Comment>("SELECT * FROM Comments WHERE BlogID ='" + blogID + "'").ToList();

            foreach (var commentItem in commentList)
            {
                Comment comment = db.Comments.Find(commentItem.CommentID);

                db.Comments.Remove(comment);
                db.SaveChanges();
            }

            var blog = db.Blogs.Find(blogID);

            db.Blogs.Remove(blog);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        /**************************** Edit Action Method *****************************/
        [Authorize(Users = "dreandesigns@me.com, patrikdrean@gmail.com, Patrik, Lauri Drean, Lauri, dreandesigns")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Blog blog = db.Blogs.Find(id);

            ViewBag.Date = blog.Date;
            ViewBag.HeaderImage = blog.HeaderImage;
            ViewBag.BlogTitle = blog.Title;
            ViewBag.ID = blog.BlogID;
            // Load up the blog content
            var contentList = db.Database.SqlQuery<Content>("SELECT * FROM Contents WHERE BlogID ='" + blog.BlogID + "'").ToList();
            string contentToDisplay = "";


            foreach(var contentItem in contentList)
            {
                contentToDisplay += contentItem.Text.ToString();
                contentToDisplay += "\r\n";
            }

            ViewBag.Content = contentToDisplay;

            return View();
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirmed(FormCollection form, HttpPostedFileBase postedFile)
        {
            // Grab the blogId to first edit all the content associated and then the actual blog information
            int blogID = Int32.Parse(form["id"]);

            // Assign the blog object all of its new attributes
            Blog blog = new Blog();

            // Delete all prior content content related to this blog before adding it in

            var contentList = db.Database.SqlQuery<Content>("SELECT * FROM Contents WHERE BlogID ='" + blogID + "'").ToList();

            foreach (var contentItem in contentList)
            {
                Content content = db.Contents.Find(contentItem.ContentID);

                db.Contents.Remove(content);
                db.SaveChanges();
            }

            // Recall the original blog item and remove it
            Blog pastBlog = db.Blogs.Find(blogID);
            db.Blogs.Remove(pastBlog);
            db.SaveChanges();


            blog.BlogID = blogID;
            blog.Date = Convert.ToDateTime(form["inputDate"]);
            blog.Title = form["inputTitle"];

            if (postedFile != null)
            {
                blog.HeaderImage = postedFile.FileName;

                // Upload the image to the images folder to be recalled later
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Content/Images/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));
                }
            }

            // Retain previous image if no image is entered to edit
            else
            {
                blog.HeaderImage = pastBlog.HeaderImage;
            }

            // Save blog to the database


            db.Blogs.Add(blog);
            db.SaveChanges();

            // Add each content piece to the database

            var formContent = form["inputContent"];
            string[] newContentList = formContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Add each content piece to the database
            int iContentCounter = db.Contents.Max(x => x.ContentID);
            foreach (var blogContent in newContentList)
            {
                iContentCounter++;

                Content content = new Content();
                content.BlogID = blog.BlogID;
                content.ContentID = iContentCounter;
                content.Text = blogContent;

                // Save changes to database
                db.Contents.Add(content);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
            
        }

















        public string Encrypt(string str)
        {
            string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
    }
}

