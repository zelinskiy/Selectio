using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Selectio.Data;
using Selectio.Models;

namespace Selectio.Controllers
{
    public class MessagesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "teacher")]
        public IActionResult Index()
        {
            var model = _context.Messages.ToArray();
            return View(model);
        }

        [HttpGet]
        public IActionResult NewMessage()
        {
            var model = new Message();
            return View(model);
        }

        [HttpPost]
        public IActionResult NewMessage(Message msg)
        {
            if (!ModelState.IsValid)
            {
                return View(msg);
            }
            _context.Messages.Add(msg);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}