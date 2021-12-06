using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace Job_Portal.Controllers
{
    public class EmployeersController : Controller
    {
        private readonly JobPortalContext _context;

        public EmployeersController(JobPortalContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            return View();
        }

        public void testgit()
        {
            Console.WriteLine("Hello GIT");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employeer employeer)
        {
            var count = _context.Employeers.ToList().Count();
            employeer.EmployerId = count + 1;
            ModelState.Remove("EmployerId");
            if (ModelState.IsValid)
            {
                if (EmployeerExists(employeer.EmailId))
                {
                    ModelState.AddModelError("EmailId", "Already Exist");
                    return View(employeer);
                }
                if (EmployeerUserExists(employeer.Username))
                {
                    ModelState.AddModelError("Username", "Already Exist");
                    return View(employeer);
                }
                _context.Add(employeer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(LoginPageE));
            }
            else
            {
                return View(employeer);
            }
        }
        public IActionResult LoginPageE()
        {
            HttpContext.Session.SetString("LoggedUserName", "null");
            var l = HttpContext.Session.GetString("LoggedUserName");
            var temp = Request.Cookies["user"] ?? "null";
            if (temp != "null" || l!="null")
            {
                HttpContext.Session.SetString("LoggedUserName", temp);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPageE(Employeer employeer)
        {
            var ans = HttpContext.Request.Form["remember"].ToString();
            var loggeduser = _context.Employeers.FirstOrDefault(emp => emp.Username == employeer.Username && emp.Password == employeer.Password);
            if (loggeduser != null)
            {
                if (ans != "")
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(10);

                    Response.Cookies.Append("user", employeer.Username, options);
                    Response.Cookies.Append("userid", employeer.EmployerId.ToString(), options);
                    Response.Cookies.Append("password", employeer.Password, options);
                }
                HttpContext.Session.SetString("LoggedUserName", loggeduser.EmployerId.ToString());
                return RedirectToAction(nameof(HomePage));
            }
            else
            {
                ViewBag.error = "Username and Password does not exist";
                return View();
            }

        }
        public IActionResult HomePage()
        {
            ViewBag.control = "Employer";
            var user = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.log = user;
            if (user != "null")
            {
                var result = _context.Jobes.Where(x => x.EmployerId == Convert.ToInt32(user)).ToList();
                return View(result);
            }
            else
            {
                return RedirectToAction(nameof(LoginPageE));
            }
        }
        public IActionResult AddJob()
        {
            ViewBag.control = "Employer";
            var user = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.log = user;
            if (user != "null")
            {
                ViewData["JobLocation"] = new SelectList(_context.Locations, "LocationId", "LocationName");
                return View();
            }
            else
            {
                return RedirectToAction(nameof(LoginPageE));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJob(Jobe j)
        {
            var user = HttpContext.Session.GetString("LoggedUserName");
            var result = _context.Employeers.First(x => x.EmployerId == Convert.ToInt32(user));
            j.EmployerId = result.EmployerId;
            j.JobId = _context.Jobes.Count() + 1;
            ModelState.Remove("EmployerId");
            ModelState.Remove("JobId");
            if (ModelState.IsValid)
            {
                _context.Add(j);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomePage));
            }
            ViewData["DeptNo"] = new SelectList(_context.Locations, "LocationId", "LocationName", j.JobLocation);
            return View(j);
        }
        public IActionResult ViewCandidate(int id)
        {
            ViewBag.control = "Employer";
            var user = JsonConvert.DeserializeObject<string>(HttpContext.Session.GetString("LoggedUserName"));
            ViewBag.log = user;
            if (user != "null")
            {
                ViewBag.s = _context.Statuses.ToList();
                List<Candidate> result = new List<Candidate>();
                HttpContext.Session.SetString("jobid", id.ToString());
                var data = _context.SubmittedJobs.Where(x => x.JobId == id);
                foreach (var d in data)
                {
                    Candidate temp = new Candidate();
                    var t = _context.JobSeekers.FirstOrDefault(x => x.ApplicantId == d.ApplicantId);
                    temp.ApplicantName = t.UserName;
                    temp.Status = Convert.ToInt32(d.StatusId);
                    result.Add(temp);
                }

                return View(result);
            }
            else
            {
                return RedirectToAction(nameof(LoginPageE));
            }
                
        }
        public IActionResult ViewCandidateEdit(string id)
        {
            ViewBag.control = "Employer";
            var user = HttpContext.Session.GetString("LoggedUserName");
            ViewBag.log = user;
            if (user != "null")
            {
                ViewBag.s = _context.Statuses.ToList();
                var data = _context.JobSeekers.FirstOrDefault(x => x.UserName == id);
                ViewBag.name = data.UserName;
                ViewBag.id = data.ApplicantId;
                HttpContext.Session.SetString("name", data.UserName);
                return View();
            }
            else
            {
                return RedirectToAction(nameof(LoginPageE));
            }
                
            //HttpContext.Session.SetString("jobdetails", JsonConvert.SerializeObject(data));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewCandidateEdit(Candidate candidate)
        {
            var name = HttpContext.Session.GetString("name");
            var jobid = HttpContext.Session.GetString("jobid");
            var statusid = HttpContext.Request.Form["JobStatus"].ToString();
            //var name = HttpContext.Request.Form["name"].ToString();
            var n = _context.SubmittedJobs.First(x => x.Applicant.UserName == name);
            var candid = _context.SubmittedJobs.First(x => x.JobId == Convert.ToInt32(jobid) && x.Applicant.UserName == name);
            candid.JobId = Convert.ToInt32(jobid);
            candid.ApplicantId = n.ApplicantId;
            var x = _context.Statuses.FirstOrDefault(x => x.StatusName == statusid);
            candid.StatusId = x.StatusId;
            _context.SubmittedJobs.Update(candid);
            _context.SaveChanges();
            return RedirectToAction(nameof(HomePage));
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("LoggedUserName", "null");
            Response.Cookies.Delete("user");
            Response.Cookies.Delete("password");
            return RedirectToAction("Index", "Home");
        }
        private bool EmployeerExists(string email)
        {
            return _context.JobSeekers.Any(e => e.EmailId == email) && _context.Employeers.Any(e => e.EmailId == email);
        }

        private bool EmployeerUserExists(string username)
        {
            return _context.Employeers.Any(e => e.Username == username) && _context.JobSeekers.Any(e => e.EmailId == username);
        }
    }
}
