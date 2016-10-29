using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Selectio.Data;
using Selectio.Models;
using Selectio.Models.SqlSolvingViewModels;
using Selectio.Services;

namespace Selectio.Controllers
{
    [Authorize]
    public class SqlSolvingsController : Controller
    {

        private const string Sep = "\n\n\n";

        private readonly ApplicationDbContext _context;
        private readonly SandboxService _sandboxService;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public SqlSolvingsController(
            ApplicationDbContext context,
            SandboxService sandboxService,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _sandboxService = sandboxService;
            _logger = loggerFactory.CreateLogger<SqlSolvingsController>();
            _userManager = userManager;
        }

        private ApplicationUser Me
        {
            get
            {
                return _userManager.Users
                    .First(u => u.UserName == User.Identity.Name);
            }
        }

        public IActionResult Index()
        {
            var mySolvedTasksIds = _context.SqlSolvings
                .Where(s => s.ApplicationUserId == Me.Id && s.IsCorrect)
                .Select(s => s.SqlTaskId)
                .ToArray();

            var model = _context.SqlTasks.Select(t => new SqlTaskViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description
                })
                .ToArray();

            foreach (var t in model)
            {
                t.IsSolved = mySolvedTasksIds.Contains(t.Id);
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            var sqlTask = _context.SqlTasks
                .FirstOrDefault(t => t.Id == id);
            if (sqlTask == null)
            {
                return RedirectToAction("Index");
            }

            var model = new SolvingViewModel
            {
                SqlTaskId = sqlTask.Id,
                Creates = sqlTask.Creates,
                Inserts = sqlTask.Inserts,
                Name = sqlTask.Name,
                Description = sqlTask.Description
            };

            return View(model);
            
        }

        [HttpPost]
        public IActionResult Create(SolvingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sqlTask = _context.SqlTasks
                .FirstOrDefault(t => t.Id == model.SqlTaskId);
            if (sqlTask == null)
            {
                ModelState.AddModelError("task", $"Task {model.SqlTaskId} not found!");
                return View(model);
            }

            var newSolving = new SqlSolving
            {
                SqlTaskId = sqlTask.Id,
                IsCorrect = false,
                ApplicationUserId = Me.Id,
                Solving = model.MySolving,
                SolvedAt = DateTime.Now
            };
            _context.SqlSolvings.Add(newSolving);
            
            
            var completeQuery = model.Creates
                                + Sep
                                + model.Inserts
                                + Sep
                                + model.MySolving;

            var myOutput = "";
            _sandboxService.TryExecuteQuery(completeQuery, ref myOutput);
            _sandboxService.FlushDatabase();
            if (myOutput == sqlTask.SolvingOutput)
            {
                newSolving.IsCorrect = true;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("sql", myOutput);

            _context.SaveChanges();
            return View(model);

        }

        [Authorize(Roles = "teacher")]
        public IActionResult AllSolvings()
        {
            var model = _context.SqlSolvings.ToArray();
            return View(model);
        }

        [Authorize(Roles = "teacher")]
        public IActionResult AllTaskSolvings(int id)
        {
            var model = _context.SqlSolvings
                .Where(s=>s.IsCorrect && s.SqlTaskId == id)
                .ToArray();
            return View("AllSolvings", model);
        }

        [Authorize(Roles = "teacher")]
        public IActionResult AllUserSolvings(string id)
        {
            var model = _context.SqlSolvings
                .Where(s => s.IsCorrect && s.ApplicationUserId == id)
                .ToArray();
            return View("AllSolvings", model);
        }

        public IActionResult MySolvings()
        {
            var model = _context.SqlSolvings
                .Where(s=>s.ApplicationUserId == Me.Id)
                .ToArray();
            return View(model);
        }
    }
}