using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PastPapers.Models;

namespace PastPapers.Controllers
{
    public class HomeController : Controller
    {

        public HttpContext httpContext;

        public HomeController(IHttpContextAccessor contextAccessor)
        {
            httpContext = contextAccessor.HttpContext;
        }

        public IActionResult Index(HomeModel homeModel = null)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (homeModel == null)
            {
                return View(new HomeModel(httpContext));
            } else
            {
                homeModel.httpContext = httpContext;
                return View(homeModel);
            }
        }

        public IActionResult Logout()
        {
            httpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPastPaper(HomeModel homeModel = null)
        {
            if (homeModel == null)
            {
                return RedirectToAction("Index");
            } else
            {
                homeModel.httpContext = httpContext;
                homeModel.AddPastPaper();

                return View("Index", homeModel);
            }            

        }
    }
}