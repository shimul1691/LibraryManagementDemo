﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;
using LibraryManagement.Helper;
using MailKit.Net.Smtp;
using MimeKit;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace LibraryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryManagementContext _context;

        public AccountController(LibraryManagementContext context)
        {
            _context = context;
        }


        public IActionResult RegisterAccount()
        {
            return View();
        }



        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAccount([Bind("FirstName,LastName,Nidno,PassportNo,Email,MobileNo,HomeAddress,City,Country,DateOfBirth,Uname,Upassword,ConfirmPassword")] LibraryUserRegistrationRequest libraryUserRequest)
        {
            //Model validation
            if (ModelState.IsValid)
            {
                string message;
                //Email verify
                var userWithEmail =
                    _context.LibraryUser.Where(
                        a => a.Email == libraryUserRequest.Email && a.AccountStatus == "Active").FirstOrDefault();

                var userRequestWithEmail =
                    _context.LibraryUserRegistrationRequest.Where(
                        a => a.Email == libraryUserRequest.Email &&
                        (a.UserRequestStatus != "New" || a.UserRequestStatus == "Email Veified")).FirstOrDefault();

                var rejectedInthreeMonth =
                    _context.LibraryUserRegistrationRequest.Where(
                        a => (a.Email == libraryUserRequest.Email
                        || a.Nidno == libraryUserRequest.Nidno ||
                        a.PassportNo == libraryUserRequest.PassportNo) && a.UserRequestStatus == "Reject"
                        && a.RequestTime >= DateTime.Now.AddMonths(-3)).FirstOrDefault();
                if (userRequestWithEmail != null)
                {
                    ModelState.AddModelError(
                        "Email", "There is already a pending membership request for this mailing address");
                    return View(libraryUserRequest);
                }
                if (userWithEmail != null)
                {
                    ModelState.AddModelError("Email", "There is already a member with mailing address");
                    return View(libraryUserRequest);
                }
                if (rejectedInthreeMonth != null)
                {
                    ViewBag.Status = true;
                    message = "Libarry Membership Request for "
                        + libraryUserRequest.FirstName +
                        " " + libraryUserRequest.LastName + " " +
                        "has been rejected less then three months ago. We can not receive your application now."
                        + " Please try again later";
                    ViewBag.Message = message;
                    return View(libraryUserRequest);
                }


                //generate activation code
                libraryUserRequest.ActivationCode = System.Guid.NewGuid().ToString();


                //Password Hashing
                libraryUserRequest.Upassword = CryptoHelper.Hash(libraryUserRequest.Upassword);

                libraryUserRequest.RequestTime = System.DateTime.Now;
                libraryUserRequest.UserRequestStatus = "New";
                //Save Request
                _context.LibraryUserRegistrationRequest.Add(libraryUserRequest);
                await _context.SaveChangesAsync();

                //Send email to user
                //SendEmailWithVerificationString(libraryUserRequest.Email, libraryUserRequest.ActivationCode);
                ViewBag.Status = true;
                message = "Libarry Membership Request for "
                    + libraryUserRequest.FirstName +
                    " " + libraryUserRequest.LastName + " " +
                    "is successfully placed. To Verify request please check your mail for activation link."
                    + " if you fail to activate with in 3 days your activation link will expire";
                ViewBag.Message = message;

                return View(libraryUserRequest);
            }
            return View(libraryUserRequest);
        }


        public async Task<IActionResult> Activate(Guid id)
        {
            var libraryUserRequest = _context.LibraryUserRegistrationRequest.Where(
                a => a.UserRequestStatus == "New" && a.ActivationCode == id.ToString()).FirstOrDefault();

            if (libraryUserRequest != null)
            {
                LibraryUserRegistrationRequest Request = (LibraryUserRegistrationRequest)libraryUserRequest;
                if ((DateTime.Today - Request.RequestTime).TotalDays > 3)
                {
                    ViewBag.Message = "Your membership request verification time has expired. Please apply again";
                }
                else
                {
                    ViewBag.Message =
                        "Hi! " + Request.FirstName + " " + Request.LastName +
                        " Your library membership request has been received by WellDev." +
                        " Please take the original and photocopy of your identification document (NID or Passport)" +
                        " to the library administrator office for user account activation with in 30 days";

                    Request.UserRequestStatus = "Email Verified";

                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                ViewBag.Message = "Invalid Request";
            }

            return View();
        }

        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Uname")] string Uname,[Bind("Upassword")] string Upassword) 
        {
            if (!string.IsNullOrEmpty(Uname) && !string.IsNullOrEmpty(Upassword))
            {
                ViewBag.State = true;
                string uname = Uname.Trim();
                string upassword = Upassword.Trim();
                upassword = CryptoHelper.Hash(upassword);

                var user = _context.LibraryUser.Where(
                    a => a.Uname == uname && a.Upassword == upassword).FirstOrDefault();

                if (user != null)
                {
                    if (user.AccountStatus == "Active")
                    {
                        Microsoft.AspNetCore.Http.HttpContext currentHttpContext = HttpContext;
                        currentHttpContext.Session.SetString("UserID", user.UserId.ToString());
                        currentHttpContext.Session.SetString("UserName", user.Uname.ToString());
                        currentHttpContext.Session.SetString("FullName", 
                            user.FirstName.ToString() + " " + user.LastName.ToString());

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "Your account is not active. Please contact Library Admin";
                    }
                }
                else
                {
                    ViewBag.Message = "invalid username or password";
                }

            }
           
            
            return View();
        }

        public IActionResult Logout()
        {
            Microsoft.AspNetCore.Http.HttpContext currentHttpContext = HttpContext;

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                HttpContext.Session.Remove("UserID");
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                HttpContext.Session.Remove("UserName");
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("FullName")))
            {
                HttpContext.Session.Remove("FullName");
            }

            return RedirectToAction("Index", "Home");
        }

        private bool LibraryUserExists(int id)
        {
            return _context.LibraryUser.Any((System.Linq.Expressions.Expression<Func<LibraryUser, bool>>)(e => e.UserId == id));
        }

        [NonAction]
        private void SendEmailWithVerificationString(string emailId, string verificationCode)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Admin",
            "admin@example.com");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User",
            emailId);
            message.To.Add(to);

            message.Subject = "This is email subject";

            string urlLink = "/Account/Activate/" + verificationCode;
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody =
                "<h1>Thank you for requesting WellDev Library Membershi </h1> <br /> <P>please click this <a href=\"" + urlLink + "\" >link</a> to activat your mail.";

            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            //client.Connect("smtp_address_here", "port_here", true);
            client.Authenticate("user_name_here", "pwd_here");

            client.Send(message);

        }
    }
}
