using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Selectio.Data;
using Selectio.Models;
using Selectio.Models.SqlTasksViewModels;
using Selectio.Services;

namespace Selectio.Controllers
{
    [Authorize(Roles = "teacher")]
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
        private IActionResult Create()
        {
            return View(new TaskViewModel());
        }

        [HttpGet]
        public IActionResult Create(int id = -1)
        {
            if(id == -1) return Create();

            var oldTask = _context.SqlTasks
                .FirstOrDefault(t => t.Id == id);
            if (oldTask == null)
            {
                return NotFound($"Task {id} not found!");
            }
            var model = new TaskViewModel
            {
                Creates = oldTask.Creates,
                Inserts = oldTask.Inserts,
                Solving = oldTask.Solving,
                Description = oldTask.Description,
                Name = oldTask.Name,
                IsWriteAction = oldTask.IsWriteAction
            };
            return View(model);
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _context.SqlTasks
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
            {
                return NotFound($"Task {id} not found");
            }
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(SqlTask model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.SqlTasks.Update(model);
            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }
        

        public IActionResult Delete(int id)
        {
            var model = _context.SqlTasks
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
            {
                return NotFound($"Task {id} not found");
            }
            _context.SqlTasks.Remove(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var model = _context.SqlTasks
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
            {
                return NotFound($"Task {id} not found");
            }

            return View(model);
        }


        public IActionResult FlushDatabase()
        {
            return Content(_sandboxService.FlushDatabase());
        }
    }
}