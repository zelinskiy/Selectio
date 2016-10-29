using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Selectio.Data;
using Selectio.Models;
using Selectio.Models.SqlTasksViewModels;
using Selectio.Services;

namespace Selectio.Controllers
{
    public class SqlTasksController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly SandboxService _sandboxService;
        private readonly ILogger _logger;

        public SqlTasksController(
            ApplicationDbContext context,
            SandboxService sandboxService,
            ILoggerFactory loggerFactory
        )
        {
            _context = context;
            _sandboxService = sandboxService;
            _logger = loggerFactory.CreateLogger<SqlTasksController>();
        }

        public IActionResult Index()
        {
            var allTasks = _context.SqlTasks.ToArray();
            return View(allTasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TaskViewModel());
        }

        [HttpPost]
        public IActionResult Create(TaskViewModel model)
        {
            var sep = "\n\n\n";
            string output = "";
            var completeQuery = model.Creates
                                + sep
                                + model.Inserts
                                + sep
                                + model.Solving;

            var succeed = _sandboxService.TryExecuteQuery(completeQuery, ref output);
            _sandboxService.FlushDatabase();
            if (succeed)
            {
                var newSqlTask = new SqlTask
                {
                    Name = model.Name,
                    Description = model.Description,
                    Creates = model.Creates,
                    Inserts = model.Inserts,
                    Solving = model.Solving,
                    IsWriteAction = model.IsWriteAction,
                    SolvingOutput = output
                };

                _context.SqlTasks.Add(newSqlTask);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("sql", output);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult FlushDatabase()
        {
            return Content(_sandboxService.FlushDatabase());
        }
    }
}