using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project1.DAL;
using Project1.Models;
using System.Web.Helpers;
using System.Net.Mail;
using Microsoft.AspNet.Identity;

namespace Project1.Controllers
{
    //this controller handles the landing page and the info and contact pages
    public class HomeController : Controller
    {
        
        private MagicFeetContext db = new MagicFeetContext();

        public ActionResult Index()
        {
            int blogID = db.Blogs.Max(x => x.BlogID);
            Blog blog = db.Blogs.Find(blogID);
            return View(blog);
        }

        public ActionResult FootZone()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {

            Emailer email = new Emailer();

            return View(email);
        }

        //handles contact submission logic and view
        [HttpPost]
        public ActionResult Contact([Bind(Include ="EmailerName, EmailAddress, EmailSubject, EmailMessage")] Emailer emailer)
        {
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                mail.From = new MailAddress("dreantester@gmail.com");
                mail.To.Add("dreandesigns@me.com");
                mail.Subject = emailer.EmailSubject;
                mail.Body = emailer.EmailMessage + "\n\n\n" +
                            "From: " + emailer.EmailerName + "\n" +
                             "Email Address: " + emailer.EmailAddress;
                             

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("dreantester@gmail.com", "TestAccount");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                return RedirectToAction("Success");
            }

            List<SelectListItem> Subjects = new List<SelectListItem>();
            Subjects.Add(new SelectListItem { Text = "Request an appointment", Value = "0", Selected = true });
            Subjects.Add(new SelectListItem { Text = "Get more information", Value = "1" });
            Subjects.Add(new SelectListItem { Text = "Other", Value = "2" });
            ViewBag.Subjects = Subjects;

            return View(emailer);


        }

        public ActionResult Success()
        {

            return View();
        }
    }
}