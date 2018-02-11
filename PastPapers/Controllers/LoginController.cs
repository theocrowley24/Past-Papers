using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PastPapers.Models;
using Microsoft.AspNetCore.Http;

namespace PastPapers.Controllers
{
    public class LoginController : Controller
    {
        public HttpContext httpContext;

        public LoginController(IHttpContextAccessor contextAccessor)
        {
            httpContext = contextAccessor.HttpContext;
        }

        public IActionResult Index(LoginModel loginModel = null)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (loginModel == null)
            {
                return View();
            } else
            {
                return View(loginModel);
            }            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AttemptLogin(LoginModel loginModel = null)
        {
            if (loginModel != null)
            {
                loginModel.HttpContext = httpContext;
                loginModel.AttemptLogin();

                if (loginModel.LoginSuccess)
                {
                    return RedirectToAction("Index", "Home", new HomeModel(httpContext));
                } else
                {
                    return View("Index", loginModel);
                }
                
            } else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AttemptRegister(LoginModel loginModel = null)
        {
            if (loginModel != null)
            {
                loginModel.HttpContext = httpContext;
                loginModel.AttemptRegister();

                if (loginModel.RegisterSuccess)
                {
                    return RedirectToAction("Index");
                } else
                {
                    return View("Index", loginModel);
                }
            } else
            {
                return RedirectToAction("Index");
            }
        }
    }
}