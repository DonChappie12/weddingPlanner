using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using wedding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace wedding.Controllers
{
    public class HomeController : Controller
    {
        private UserContext _context;
        public HomeController(UserContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Create(ValidateUser user)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<ValidateUser> Hasher = new PasswordHasher<ValidateUser>();
                user.Password = Hasher.HashPassword(user, user.Password);
                User newUser = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password
                };
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("user_id",newUser.Id);
                return Redirect("/dashboard/"+newUser.Id);
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult LoginIn(string Email, string Password)
        {
            var user = _context.user.Where(u=> u.Email == Email).FirstOrDefault();
            if(user != null && Password != null)
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(user, user.Password, Password))
                {
                    HttpContext.Session.SetInt32("user_id", user.Id);
                    return RedirectToAction("Dashboard");
                }
            }
            ViewBag.error="Email and/or Password dont match";
            return View("Index");
        }

        [Route("dashboard")]
        public IActionResult Dashboard(int Id)
        {
            User currUser = _context.user.SingleOrDefault(c=>c.Id == HttpContext.Session.GetInt32("user_id"));
            if( currUser != null)
            {
                List<Wedding> weddings = _context.wedding.Include(wedd=>wedd.Attendee).ToList();
                ViewBag.wedders = weddings;
                ViewBag.id = currUser.Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("info/{Id}")]
        public IActionResult Info(int Id)
        {
            User currUser = _context.user.SingleOrDefault(c=>c.Id == HttpContext.Session.GetInt32("user_id"));
            if( currUser != null )
            {
                List<Wedding> weddings = _context.wedding.Where(I => I.Id == Id).Include(u => u.user).Include(a => a.Attendee).ThenInclude(ua => ua.user).ToList();
                ViewBag.info = weddings;
                return View(weddings[0]);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("new_wedding")]
        public IActionResult NewWedding(int Id)
        {
            User currUser = _context.user.SingleOrDefault(c=>c.Id == HttpContext.Session.GetInt32("user_id"));
            if( currUser != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("create_wedding")]
        public IActionResult CreateWedding(Wedding wedd)
        {
            User currUser = _context.user.SingleOrDefault(c=>c.Id == HttpContext.Session.GetInt32("user_id"));
            if( currUser != null)
            {
                if(ModelState.IsValid)
                {
                    if(wedd.Date > DateTime.Now)
                    {
                        Wedding newWedding = new Wedding
                        {
                            WedderOne = wedd.WedderOne,
                            WedderTwo = wedd.WedderTwo,
                            Date = wedd.Date,
                            WeddingAddress = wedd.WeddingAddress,
                            User_Id = currUser.Id
                        };
                        _context.wedding.Add(newWedding);
                        _context.SaveChanges();
                        return RedirectToAction("Dashboard");
                    }
                    ViewBag.date = "Date of the wedding must be AFTER today!";
                    return View("NewWedding");
                }
                return View("NewWedding");
            }
            return RedirectToAction("Index");
        }

        [Route("delete/{Id}")]
        public IActionResult Delete(int Id)
        {
            Wedding thisWedding = _context.wedding.Include(x => x.Attendee).SingleOrDefault(d => d.Id == Id);
            foreach(var at in thisWedding.Attendee){
                _context.Remove(at);
            }
            _context.Remove(thisWedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [Route("rvsp/{Id}")]
        public IActionResult RVSP(int Id)
        {
            int curId = (int)HttpContext.Session.GetInt32("user_id");
            Wedding thisWed = _context.wedding.Include(x => x.Attendee).ThenInclude(x => x.user).SingleOrDefault(x => x.Id == Id);
            Attendees newAttendant = new Attendees()
            {
                User_Id = curId,
                Wedding_Id = Id
            };
            _context.attendees.Add(newAttendant);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [Route("unrvsp/{Id}")]
        public IActionResult UNRVSP(int Id)
        {
            int curId = (int)HttpContext.Session.GetInt32("user_id");
            Attendees thisAttendant = _context.attendees.SingleOrDefault(x => x.Wedding_Id == Id && x.User_Id == curId);
            if(thisAttendant!=null){
                _context.attendees.Remove(thisAttendant);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

    // [HttpPost]
    // [Route("create_wedding")]
    // public IActionResult CreateWedding(Wedding data)
    // {
    //     User currUser = _context.user.SingleOrDefault(c=>c.Id == HttpContext.Session.GetInt32("user_id"));
    //     if(ModelState.IsValid)
    //     {
    //         if(data.Date < DateTime.Today)
    //         {
    //             ViewBag.date = "Date of Wedding must be AFTER today.";
    //             return View("NewWedding");
    //         }
    //         _context.Add(data);
    //         _context.SaveChanges();
    //         return RedirectToAction("Dashboard");
    //     }else{
    //         return View("NewWedding");
    //     }
    // }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            User currUser = _context.user.SingleOrDefault(c=>c.Id == HttpContext.Session.GetInt32("user_id"));
            if( currUser != null)
            {
                HttpContext.Session.Clear();
                return Redirect("/");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}