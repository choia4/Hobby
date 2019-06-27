using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hobby.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Hobby.Controllers
{
    public class HomeController : Controller
    {
        MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        [HttpPost("registration")]
        public IActionResult Registration(User newUser)
        {
            if (ModelState.IsValid)
            {
                //Check if email already exists
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                if (userInDb != null)
                {
                    ModelState.AddModelError("Email", "Account Already Exists");
                    return View("Index");
                }

                //Check if Username already exists
                var usernameInDb = dbContext.Users.FirstOrDefault(u => u.Username == newUser.Username);
                if( usernameInDb != null) {
                    ModelState.AddModelError("Username", "User name already exists please pick another User name");
                }

                // Initializing a PasswordHasher object, providing our User class as its
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                dbContext.Add(newUser);
                dbContext.SaveChanges();

                //query the user to get their new user Id
                var this_user = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);

                HttpContext.Session.SetString("First_Name", this_user.First_Name);
                HttpContext.Session.SetString("Last_Name", this_user.Last_Name);
                HttpContext.Session.SetInt32("User_Id", this_user.UserId);

                return RedirectToAction("Hobbies");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("loginval")]
        public IActionResult LoginVal(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.Email2);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email2", "Invalid Email/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password2);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("Password2", "Invalid Email/Password");
                    return View("Index");
                }

                //instantiate session variables
                HttpContext.Session.SetString("First_Name", userInDb.First_Name);
                HttpContext.Session.SetString("Last_Name", userInDb.Last_Name);
                HttpContext.Session.SetInt32("User_Id", userInDb.UserId);
                return RedirectToAction("Hobbies");

            }
            else
            {
                return View("INdex");
            }
        }


        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("User_Id") != null)
                return RedirectToAction("Hobbies");
            return View();
        }

        [Route("login")]
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("User_Id") != null)
                return RedirectToAction("Hobbies");
            return View();
        }

        public List<List<string>> CalculateMaxes() {
            List<Hobbies> Hobbies = dbContext.Hobbies
                                .Include(h => h.HobbiesInUsers)
                                .ThenInclude(u => u.User)
                                .OrderByDescending(i => i.HobbiesInUsers.Count)
                                .ToList();
            List<string> maxNovice = new List<string>();
            int NoviceI = 0;
            List<string> maxIntermediate = new List<string>();
            int IntermediateI = 0;
            List<string> maxExpert = new List<string>();
            int ExpertI = 0;

            foreach (Hobbies hobby in Hobbies)
            {
                int countn = 0;
                int counti = 0;
                int counte = 0;
                foreach (HIU h in hobby.HobbiesInUsers)
                {
                    if (h.Proficiency == "Novice")
                        countn++;
                    else if (h.Proficiency == "Intermediate")
                        counti++;
                    else if (h.Proficiency == "Expert")
                        counte++;
                }
                if (countn > NoviceI)
                {
                    maxNovice.Clear();
                    NoviceI = countn;
                    maxNovice.Add(hobby.Name);
                }
                else if (countn == NoviceI && countn != 0)
                {
                    maxNovice.Add(hobby.Name);
                }

                if (counti > IntermediateI)
                {
                    maxIntermediate.Clear();
                    IntermediateI = counti;
                    maxIntermediate.Add(hobby.Name);
                }
                else if (countn == NoviceI && counti != 0)
                {
                    maxIntermediate.Add(hobby.Name);
                }

                if (counte > ExpertI)
                {
                    maxExpert.Clear();
                    ExpertI = counte;
                    maxExpert.Add(hobby.Name);
                }
                else if (countn == NoviceI && counte != 0)
                {
                    maxExpert.Add(hobby.Name);
                }
            }

            if (NoviceI == 0)
                maxNovice.Add("None");
            if (IntermediateI == 0)
                maxIntermediate.Add("None");
            if (ExpertI == 0)
                maxExpert.Add("None");
            return new List<List<string>> {maxNovice, maxIntermediate, maxExpert};
        }

        [Route("/Hobbies")]
        [HttpGet]
        public IActionResult Hobbies()
        {
            if (HttpContext.Session.GetInt32("User_Id") == null)
                return RedirectToAction("Index");
            List<Hobbies> Hobbies = dbContext.Hobbies
                                .Include(h => h.HobbiesInUsers)
                                .ThenInclude(u => u.User)
                                .OrderByDescending(i => i.HobbiesInUsers.Count)
                                .ToList();

            List<List<string>> list = CalculateMaxes();
            
            ViewBag.Novice = list[0];
            ViewBag.Intermediate = list[1];
            ViewBag.Expert = list[2];

         
            return View(Hobbies);
        }

        [Route("/AddHobby")]
        [HttpGet]
        public IActionResult AddHobby() {
            int? MUser_Id = HttpContext.Session.GetInt32("User_Id");
            if (MUser_Id == null)
                return RedirectToAction("Index");
            int User_Id = (int) MUser_Id;
            return View();
        }

        [HttpPost("CreateHobby")]
        public IActionResult CreateHobby(Hobbies Hob) {
            int? MUser_Id = HttpContext.Session.GetInt32("User_Id");
            if (MUser_Id == null)
                return RedirectToAction("Index");
            int User_Id = (int)MUser_Id;
            if(ModelState.IsValid) {
                
                List<Hobbies> hobbies = dbContext.Hobbies.ToList();
                foreach(Hobbies hobby in hobbies)
                    if(hobby.Name == Hob.Name) {
                        ModelState.AddModelError("Name", "Hobby name already exists");
                        return View("AddHobby");
                    }

                dbContext.Add(Hob);
                dbContext.SaveChanges();
                return RedirectToAction("Hobbies");
            } else {
                return View("AddHobby");
            }
        }

        [HttpGet("/HobbyDescription/{Hobby_Id}")]
        public IActionResult HobbyDescription(int Hobby_Id) {
            int? MUser_Id = HttpContext.Session.GetInt32("User_Id");
            if (MUser_Id == null)
                return RedirectToAction("Index");
            int User_Id = (int)MUser_Id;
            Hobbies Hobby = dbContext.Hobbies
                            .Include(h => h.HobbiesInUsers)
                            .ThenInclude(u => u.User)
                            .FirstOrDefault(r => r.HobbyId == Hobby_Id);
            return View("HobbyDescription", Hobby);
        }

        [HttpPost("/CreateEnthusiast")]
        public IActionResult CreateEnthusiast(int Hobby_Id, string Proficiency) {
            int? MUser_Id = HttpContext.Session.GetInt32("User_Id");
            if (MUser_Id == null)
                return RedirectToAction("Index");
            int User_Id = (int) MUser_Id;

            Hobbies hobby = dbContext.Hobbies
                            .Include(h => h.HobbiesInUsers)
                            .ThenInclude(v => v.User)
                            .FirstOrDefault(u => u.HobbyId == Hobby_Id);

            foreach(HIU h in hobby.HobbiesInUsers) {
                System.Console.WriteLine("__________");
                System.Console.WriteLine("session Id: " + User_Id);
                System.Console.WriteLine("hobby user id:" + h.UserId);
                if(h.UserId == User_Id)
                    return View("HobbyDescription",hobby);
            }

            User user = dbContext.Users
                        .Include(h => h.UsersInHobbies)
                        .ThenInclude(v => v.Hobby)
                        .FirstOrDefault(u => u.UserId == User_Id);



            HIU H = new HIU();
            H.User = user;
            H.Hobby = hobby;
            H.Proficiency = Proficiency;
            dbContext.Add(H);
            dbContext.SaveChanges();
            return RedirectToAction("Hobbies");

        }

        // [HttpPost("/EditHobbies")]
        // public IActionResult EditHobbies(Hobbies Hob) {

        // }
        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");

        }

    }
}
