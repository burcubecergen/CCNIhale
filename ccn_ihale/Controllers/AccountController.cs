using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ccn_ihale.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Globalization;

namespace ccn_ihale.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        DateTime myDateTime;

        private IhalePortalContext context;

        public AccountController(IhalePortalContext ctx)
        {
            context = ctx;
        }
     
        public void logAdd(string loginName,int ?user_ID,string statement)
        {
            myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("s");
            DateTime sqlFormattedDatedt = Convert.ToDateTime(sqlFormattedDate, CultureInfo.InvariantCulture);
            
            context.Log.Add(new Log
            {
                Mesaj = statement +"("+loginName+")",
                Tarih = sqlFormattedDatedt,
                LoginName = loginName,
                Kullanici_ID = user_ID
            });
            context.SaveChanges();
           
        }
        public IActionResult Login(string returnUrl)
        {
            HttpContext.Session.Remove("kullaniciAdi");
            
            return View();
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Login(LoginViewModel loginViewModel,string returnUrl)
        {
            string loginName = loginViewModel.LoginName.ToLower();
            
            var login = context.Kullanici.Where(k => k.LoginName == loginName && k.Sifre == loginViewModel.Sifre).FirstOrDefault();
     
            if (login != null){

                HttpContext.Session.SetString("kullaniciAdi", loginName);
                HttpContext.Session.SetInt32("kullaniciID", login.ID);
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, loginViewModel.LoginName));
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                props.IsPersistent = loginViewModel.RememberMe;
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();

                string statement = "giriş yapıldı"+" ";
                logAdd(loginName, login.ID,statement);

                return RedirectToAction("Ihl","Home",new {LoginName = loginViewModel.LoginName});
            }
            else{
                ViewBag.error = "Hatalı kullanıcı adı veya parola girişi yaptınız.";
                return View();
            }


        }
        public IActionResult LogOut()
        {
            string statement = "çıkış yapıldı"+" ";
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            int ?exit_ID = HttpContext.Session.GetInt32("kullaniciID");
            if(!String.IsNullOrWhiteSpace(loginName))
            logAdd(loginName,exit_ID, statement);
            
            HttpContext.Session.Remove("kullaniciAdi");
            HttpContext.Session.Remove("kullaniciID");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Account");
        }
        public IActionResult Contact() =>
             View();
        /*
        public IActionResult ChangePassword()
        {
         
            string statement = "çıkış yapıldı" + " ";
            string loginName = HttpContext.Session.GetString("kullaniciAdi");
            int? exit_ID = HttpContext.Session.GetInt32("kullaniciID");
            if(!String.IsNullOrWhiteSpace(loginName))
            logAdd(loginName, exit_ID, statement);

            HttpContext.Session.Remove("kullaniciAdi");
            HttpContext.Session.Remove("kullaniciID");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string loginName)
        {//control yapalım
      
            var login_control = context.Kullanici.Where(k => k.LoginName == loginName).FirstOrDefault();
            if(login_control != null) {
                HttpContext.Session.SetString("kullaniciSifre", login_control.Sifre);
                return RedirectToAction("ShowPassword",new { LoginName = login_control.LoginName });
             }
            else { 
                return View("ChangePassword");
            }
        }

        public IActionResult ShowPassword()
        {
            if (HttpContext.Session.GetString("kullaniciSifre") != null)
            {
                ViewBag.sifre = HttpContext.Session.GetString("kullaniciSifre");
                HttpContext.Session.Remove("kullaniciSifre");
            }
         
            return View();
        }
       
     
 
    */
    }
}
