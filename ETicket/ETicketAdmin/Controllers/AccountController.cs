﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ETicket.Admin.Models.IdentityModels;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser user = new IdentityUser { Email = model.Email, UserName = model.Email };

                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //await userManager.AddToRoleAsync(user, "SuperUser");// какая роль при регистрации?
                        await signInManager.SignInAsync(user, false);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            log.Warn(error.Description);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest();
            } 
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            try
            {
                return View(new LoginModel { ReturnUrl = returnUrl });
            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        log.Warn("Wrong email or password");
                        ModelState.AddModelError("", "Wrong email or password"); 
                    }
                }

                return View(model);
            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest();
            }   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();

                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest();
            }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}