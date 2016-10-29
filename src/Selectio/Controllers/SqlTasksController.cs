using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public SqlTasksController(
            ApplicationDbContext context,
            SandboxService sandboxService
        )
        {
            _context = context;
            _sandboxService = sandboxService;
        }

        public IActionResult Index()
        {
            var allTasks = _context.SqlTasks.ToArray();
            return View(allTasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateViewModel());
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel CreateViewModel)
        {
            var sep = "\n\n\n";
            string output = "";
            var completeQuery = CreateViewModel.Creates
                                + sep
                                + CreateViewModel.Inserts
                                + sep
                                + CreateViewModel.Solving;

            var succeed = _sandboxService.TryExecuteQuery(completeQuery, ref output);
            
            if (succeed)
            {
                var newSqlTask = new SqlTask
                {
                    Creates = CreateViewModel.Creates,
                    Inserts = CreateViewModel.Inserts,
                    Solving = CreateViewModel.Solving,
                    IsWriteAction = CreateViewModel.IsWriteAction,
                    SolvingOutput = output
                };

                _context.SqlTasks.Add(newSqlTask);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("sql", output);
                return View(CreateViewModel);
            }

            
        }
    }
}