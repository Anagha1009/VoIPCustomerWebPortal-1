﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VoipApplicationProject.Models;
using VoipApplicationProject.Repositories;

namespace VoipApplicationProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepo repo;
        public CustomerController(ICustomerRepo _repo)
        {
            repo = _repo;
        }

        #region "Get All Customers / Get All Existing Users - Lucky"
        public IActionResult Index()
        {
            List<CustomerModel> CustomerList = repo.GetAllCustomers();
            ViewBag.ShowAlert = false;

            if (CustomerList.Count > 0)
            {
                return View(CustomerList);
            }
            else
            {
                return ViewBag.ShowAlert = true;
            }
        }
        #endregion

        #region "Sign Up - Jaideep"
        [HttpGet]
        public IActionResult SignUp()
        {
            ViewBag.ShowAlert = "";
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(CustomerModel CM)
        {
            CustomerModel Customer = repo.Register(CM);

            if (Customer.Id != null)
            {
                if (repo.CreateMenuAccess(Customer.Id,"Users"))
                {
                    return RedirectToAction("Login", "Customer");
                }
                else
                {
                    repo.DeleteCustomer(Customer.Id);
                    ViewBag.ShowAlert = "menu_error";
                    return View();
                }
            }
            else
            {
                ViewBag.ShowAlert = Customer.Message.ToString();
                return View();
            }
        }
        #endregion

        #region "Login - Anagha"
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.ShowAlert = "";
            return View();
        }

        [HttpPost]
        public IActionResult Login(CustomerModel customer)
        {
            CustomerModel Customer = repo.IsAuthenticated(customer);

            if (Customer.IsAuthenticated == true)
            {
                if(Customer.CustomerTypeID == customer.CustomerTypeID)
                {                    
                    SetCookie("CustomerId", Customer.Id, 60);
                    SetCookie("token", Customer.token, 60);

                    string isRememberMe = Request.Form["ChkRememberMe"];

                    if(isRememberMe != "false")
                    {
                        SetCookie("refreshtoken", Customer.refreshtoken, 600);
                    }                   

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ShowAlert = "customertype_error";
                    return View();
                }
            }
            else
            {
                ViewBag.ShowAlert = "login_error";
                return View();
            }

        }
        #endregion

        #region "Forgot Password - Krunal"
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ViewBag.ShowAlert = "";
            return View();
        }

        [HttpPost]
        [ActionName("ForgotPassword")]
        public ActionResult ForgotPasswordPost()
        {
            string email = Request.Form["Email"];

            if (repo.ForgotPassword(email))
            {
                ViewBag.ShowAlert = "success";
            }
            else
            {
                ViewBag.ShowAlert = "failed";
            }
            return View();
        }
        #endregion

        #region "Reset Password - Krunal"
        [HttpGet]
        public ActionResult ResetPassword()
        {
            ViewBag.ShowAlert = "";
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(CustomerModel resetPasswordModel)
        {
            if (repo.ResetPassword(resetPasswordModel))
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ShowAlert = "failed";
                return View();
            }            
        }
        #endregion

        #region "Set Cookies"
        public void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            var encodedValue = Encoding.UTF8.GetBytes(value);
            var validValue = WebEncoders.Base64UrlEncode(encodedValue);

            Response.Cookies.Append(key, validValue, option);
        }
        #endregion
    }
}
