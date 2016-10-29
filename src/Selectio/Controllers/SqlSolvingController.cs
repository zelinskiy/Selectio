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
    public class SqlSolvingController : Controller
    {

        private const string SEP = "\n\n\n";

        private readonly ApplicationDbContext _context;
        private readonly SandboxService _sandboxService;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public SqlSolvingController(
            ApplicationDbContext context,
            SandboxService sandboxService,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _sandboxService = sandboxService;
            _logger = loggerFactory.CreateLogger<SqlSolvingController>();
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
            var model = _context.SqlTasks.Select(t => new SqlTaskViewModel
            {
                Id = t.Id,
            })
            .ToArray();
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
                Inserts = sqlTask.Inserts
            };

            return View(model);
            
        }

        [HttpPost]
        public IActionResult Create(SolvingViewModel CreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(CreateViewModel);
            }

            var sqlTask = _context.SqlTasks
                .FirstOrDefault(t => t.Id == CreateViewModel.SqlTaskId);
            if (sqlTask == null)
            {
                ModelState.AddModelError("task", $"Task {CreateViewModel.SqlTaskId} not found!");
                return View(CreateViewModel);
            }

            var newSolving = new SqlSolving
            {
                SqlTaskId = sqlTask.Id,
                IsCorrect = false,
                ApplicationUserId = Me.Id,
                Solving = CreateViewModel.MySolving,
                SolvedAt = DateTime.Now
            };
            _context.SqlSolvings.Add(newSolving);

            var myOutput = "";
            
            var completeQuery = CreateViewModel.Creates
                                + SEP
                                + CreateViewModel.Inserts
                                + SEP
                                + CreateViewModel.MySolving;


            if (_sandboxService.TryExecuteQuery(completeQuery, ref myOutput))
            {
                if (myOutput == sqlTask.SolvingOutput)
                {
                    newSolving.IsCorrect = true;
                    _context.SaveChanges();
                    _sandboxService.FlushDatabase();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("sql", myOutput + "=/=" + sqlTask.SolvingOutput);
                }
            }
            else
            {
                ModelState.AddModelError("sql", myOutput);
            }
            _context.SaveChanges();
            _sandboxService.FlushDatabase();
            return View(CreateViewModel);

        }

        public IActionResult AllSolvings()
        {
            var model = _context.SqlSolvings.ToArray();
            return View(model);
        }
    }
}