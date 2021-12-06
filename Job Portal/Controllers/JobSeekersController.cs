using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Job_Portal.Controllers
{
    public class JobSeekersController : Controller
    {
        private readonly JobPortalContext _context;

        public JobSeekersController(JobPortalContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            HttpContext.Session.SetString("LoggedUserName", "null");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobSeeker jobSeeker)
        {
            var count = _context.JobSeekers.ToList().Count();
            jobSeeker.ApplicantId = count + 1;
            ModelState.Remove("ApplicantId");
            if (ModelState.IsValid)
            {
                if (jobSeekerExists(jobSeeker.EmailId))
                {
                    ModelState.AddModelError("EmailId", "Already Exist");
                    return View(jobSeeker);
                }
                if (jobSeekerUserExists(jobSeeker.UserName))
                {
                    ModelState.AddModelError("Username", "Already Exist");
                    return View(jobSeeker);
                }
                _context.Add(jobSeeker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(LoginPage));
            }
            else
            {
                return View(jobSeeker);
            }
        }
        public IActionResult LoginPage()
        {
            HttpContext.Session.SetString("LoggedUserName","null");
            var l = HttpContext.Session.GetString("LoggedUserName");
            var temp = Request.Cookies["user"] ?? "null";
            if (temp != "null" || l!="null" )
            {
                HttpContext.Session.SetString("LoggedUserName", temp);
                return RedirectToAction(nameof(AllJob));
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPage(JobSeeker jobSeeker)
        {
            var ans = HttpContext.Request.Form["remember"].ToString();
            var loggeduser = _context.JobSeekers.FirstOrDefault(seeker => seeker.UserName == jobSeeker.UserName && seeker.Password == jobSeeker.Password);
            if (loggeduser != null)
            {
                if (ans != "")
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(10);

                    Response.Cookies.Append("user", jobSeeker.ApplicantId.ToString(), options);
                    Response.Cookies.Append("password", jobSeeker.Password, options);
                }
                HttpContext.Session.SetString("LoggedUserName", loggeduser.ApplicantId.ToString());
                return RedirectToAction(nameof(AllJob));   /// Index Page
            }
            else
            {
                ViewBag.error = "Username and Password does not exist";
                return View();
            }
        }

        public IActionResult AllJob()
        {
            ViewBag.log = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.control = "JobSeekers";
            if (ViewBag.log !="null")
            {
                SortedList<int, String> comp = new SortedList<int, String>();
                List<Jobe> job = new List<Jobe>();
                foreach (var item in _context.Jobes)
                {
                    DateTime t1 = item.JobLastDate;
                    DateTime t2 = DateTime.Now;
                    if (DateTime.Compare(t1, t2) >= 0)
                    {
                        job.Add(item);
                        var temp = _context.Employeers.First(x => x.EmployerId == item.EmployerId);
                        if (!comp.ContainsKey(item.EmployerId))
                        {
                            comp.Add(item.EmployerId, temp.CompanyName);
                        }
                    }
                }
                ViewBag.comp = comp;
                ViewBag.jobs = job;
                return View(_context.Jobes.Include(x => x.JobLocationNavigation));
            }
            else
            {
                return RedirectToAction(nameof(LoginPage));
            }
            
        }
        public IActionResult SubmittedJob(int id)
        {
            ViewBag.log = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.control = "JobSeekers";
            if (ViewBag.log != "null")
            {
                var det = _context.Jobes.FirstOrDefault(m => m.JobId == id);
                var name = _context.Employeers.FirstOrDefault(m => m.EmployerId == det.EmployerId);
                ViewBag.Company = name.CompanyName.ToString();
                var aman = _context.Locations.First(x => x.LocationId == det.JobLocation);
                ViewBag.Location = aman.LocationName;
                if (TempData.ContainsKey("show"))
                {
                    ViewBag.show = TempData["show"].ToString();
                }
                return View(det);
            }
            else
            {
                return RedirectToAction(nameof(LoginPage));
            }
            
        }
        [HttpPost, ActionName("SubmittedJob")]
        [ValidateAntiForgeryToken]
        public IActionResult SubmittedJobs(int id)
        {
            SubmittedJob sub = new SubmittedJob();
            sub.Id = _context.SubmittedJobs.ToList().Count() + 1;
            sub.JobId = id;
            sub.ApplicantId = Convert.ToInt32(HttpContext.Session.GetString("LoggedUserName"));
            sub.StatusId = 1;

            var check = _context.SubmittedJobs.FirstOrDefault(x => x.JobId == sub.JobId && x.ApplicantId == sub.ApplicantId);
            if (check == null)
            {
                _context.SubmittedJobs.Add(sub);
                _context.SaveChanges();

                return RedirectToAction(nameof(AllJob));
            }
            else
            {
                TempData["show"] = "Already Applied"; 
                return RedirectToAction("SubmittedJob", "JobSeekers",new { id = sub.JobId });
            }
        }

        public IActionResult ViewMyJob()
        {
            ViewBag.log = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.control = "JobSeekers";
            if (ViewBag.log != "null")
            {
                var ApplicantId = Convert.ToInt32(HttpContext.Session.GetString("LoggedUserName"));
                var appliedjob = _context.SubmittedJobs.Where(x => x.ApplicantId == ApplicantId);
                List<MyJobApp> all = new List<MyJobApp>();
                foreach (var item in appliedjob)
                {
                    MyJobApp temp = new MyJobApp();
                    temp.JobTitle = _context.Jobes.FirstOrDefault(x => x.JobId == item.JobId).Jobtitle;
                    temp.JobDescription = _context.Jobes.FirstOrDefault(x => x.JobId == item.JobId).JobDescription;
                    temp.JobLocation = _context.Locations.FirstOrDefault(z => z.LocationId == _context.Jobes.FirstOrDefault(x => x.JobId == item.JobId).JobLocation).LocationName;
                    temp.JobStatus = Convert.ToInt32(item.StatusId);
                    temp.CompanyName = _context.Employeers.FirstOrDefault(z => z.EmployerId == _context.Jobes.FirstOrDefault(x => x.JobId == item.JobId).EmployerId).CompanyName;
                    all.Add(temp);
                }
                return View(all);
            }
            else
            {
                return RedirectToAction(nameof(LoginPage));
            }
            
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("LoggedUserName", "null");
            Response.Cookies.Delete("user");
            Response.Cookies.Delete("password");
            return RedirectToAction("Index", "Home");
        }

        private bool jobSeekerExists(string email)
        {
            return _context.JobSeekers.Any(e => e.EmailId == email) && _context.Employeers.Any(e => e.EmailId == email);
        }

        private bool jobSeekerUserExists(string username)
        {
            return _context.Employeers.Any(e => e.Username == username) && _context.JobSeekers.Any(e => e.EmailId == username);
        }

    }
}
