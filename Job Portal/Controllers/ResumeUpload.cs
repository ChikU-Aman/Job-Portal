using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal.Controllers
{
    public class ResumeUpload : Controller
    {
        private IHostingEnvironment _env;


        public ResumeUpload(IHostingEnvironment env)
        {
            _env = env;
        }
        public IActionResult FileUpload()
        {
            ViewBag.show = "Only pdf file support";
            if (TempData.ContainsKey("error"))
            {
                ViewBag.error = TempData["error"].ToString();
            }
            if (TempData.ContainsKey("success"))
            {
                ViewBag.success = TempData["success"].ToString();
            }
            return View();
        }
 
        public IActionResult UploadResume(IFormFile file)
        {
            var dir = _env.WebRootPath;
            var name = HttpContext.Session.GetString("LoggedUserName");
            var support = new[] { "pdf" };
            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
            if (!support.Contains(fileExt))
            {
                TempData["error"] = "Invalid Format";
                return RedirectToAction(nameof(FileUpload));
            }
            using (var filestream = new FileStream(Path.Combine(dir, name + "." + fileExt), FileMode.Create, FileAccess.Write))
            {
                TempData["success"] = "Upload Successfull";
                file.CopyTo(filestream);
            }
            return RedirectToAction(nameof(FileUpload));
        }
        public IActionResult ShowResume()
        {
            var dir = _env.ContentRootPath;
            var name = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.path = name + ".pdf";
            return View();
        }
        public IActionResult ShowResumeByEmployee(int id)
        {
            var dir = _env.ContentRootPath;
            ViewBag.path = id.ToString() + ".pdf";
            return View();
        }
    }
}
