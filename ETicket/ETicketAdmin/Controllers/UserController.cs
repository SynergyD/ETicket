﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETicket.DataAccess.Domain;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Repositories;
using ETicket.Admin.Models;
using ETicket.Admin.Services;
using Microsoft.AspNetCore.Authorization;

namespace ETicket.Admin.Controllers
{
    [Authorize(Roles = "Admin, SuperUser")]
    public class UserController : Controller
    {
        private readonly ETicketDataContext context;
        private readonly UnitOfWork repository;

        public UserController(ETicketDataContext context)
        {
            this.context = context;
            repository = new UnitOfWork(context);
        }

        // GET: User
        public IActionResult Index(string sortOrder)
        {
            ViewData["PrivilegeId"] = new SelectList(repository.Privileges.GetAll(), "Id", "Name");
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "LastName_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            var eTicketDataContext = repository.Users.GetAll();
            IOrderedQueryable<User> users;

            switch (sortOrder)
            {
                case "LastName_desc":
                    users = eTicketDataContext.OrderByDescending(s => s.LastName);
                    break;
                case "FirstName_desc":
                    users = eTicketDataContext.OrderByDescending(s => s.FirstName);
                    break;
                case "FirstName":
                    users = eTicketDataContext.OrderBy(s => s.FirstName);
                    break;
                default:
                    users = eTicketDataContext.OrderBy(s => s.LastName);
                    break;
            }

            return View(users.ToList());
        }

        // GET: User/Details/5
        public IActionResult Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = repository.Users.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {

            ViewData["DocumentId"] = new SelectList(repository.Documents.GetAll(), "Id", "Number");
            ViewData["PrivilegeId"] = new SelectList(repository.Privileges.GetAll(), "Id", "Name");

            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FirstName,LastName,Phone,Email,DateOfBirth,PrivilegeId,DocumentId")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                repository.Users.Create(user);
                repository.Save();

                return RedirectToAction(nameof(Index));
            }

            ViewData["DocumentId"] = new SelectList(context.Documents, "Id", "Number", user.DocumentId);
            ViewData["PrivilegeId"] = new SelectList(context.Privileges, "Id", "Name", user.PrivilegeId);

            return View(user);
        }

        // GET: User/SendMessage/5
        public IActionResult SendMessage(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = repository.Users.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMessage(Guid id, string message)
        {
            if (ModelState.IsValid)
            {
                var user = repository.Users.Get(id);
                if (user == null)
                {
                    return NotFound();
                }

                MailService emailService = new MailService();
                emailService.SendEmail(user.Email, message);

                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: User/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = repository.Users.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["DocumentId"] = new SelectList(context.Documents, "Id", "Number", user.DocumentId);
            ViewData["PrivilegeId"] = new SelectList(context.Privileges, "Id", "Name", user.PrivilegeId);

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,FirstName,LastName,Phone,Email,DateOfBirth,PrivilegeId,DocumentId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    repository.Users.Update(user);
                    repository.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!repository.Users.UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DocumentId"] = new SelectList(context.Documents, "Id", "Number", user.DocumentId);
            ViewData["PrivilegeId"] = new SelectList(context.Privileges, "Id", "Name", user.PrivilegeId);

            return View(user);
        }

        // GET: User/Delete/5
        public IActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = repository.Users.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            repository.Users.Delete(id);
            repository.Save();

            return RedirectToAction(nameof(Index));
        }

    }
}
