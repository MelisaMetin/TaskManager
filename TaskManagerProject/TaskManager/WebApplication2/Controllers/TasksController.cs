using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Models.Enums;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public IActionResult Index(Status? selectedStatus, string titleSearch, Priority? priorityFilter)
        {
            var tasks = _context.Tasks.ToList();

            // Populate ViewBag.StatusFilterValues
            ViewBag.StatusFilterValues = Enum.GetValues(typeof(Status))
                                            .Cast<Status>()
                                            .Select(status => new SelectListItem
                                            {
                                                Text = status.ToString(),
                                                Value = ((int)status).ToString(),
                                                Selected = status == selectedStatus
                                            })
                                            .ToList();

            // Filter tasks based on the selected status
            if (selectedStatus != null)
            {
                tasks = tasks.Where(t => t.Status == selectedStatus).ToList();
            }

            if (!string.IsNullOrEmpty(titleSearch))
            {
                tasks = tasks.Where(t => t.Title.Contains(titleSearch)).ToList();
            }

            if (priorityFilter.HasValue)
            {
                tasks = tasks.Where(t => t.Priority == priorityFilter).ToList();
            }

            tasks = tasks.OrderBy(t => t.Priority).ToList();

            var viewModel = new TaskFilterViewModel
            {
                SelectedStatus = selectedStatus,
                TitleSearch = titleSearch,
                PriorityFilter = priorityFilter,
                Tasks = tasks
            };

            return View(viewModel);
        }


        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var statusValues = Enum.GetValues(typeof(Status))
                               .Cast<Status>()
                               .Select(v => new SelectListItem
                               {
                                   Text = v.ToString(),
                                   Value = v.ToString()
                               });
            ViewBag.StatusValues = statusValues;

            ViewBag.PriorityValues = Enum.GetValues(typeof(Priority))
                                .Cast<Priority>()
                                .Select(priority => new SelectListItem
                                {
                                    Text = priority.ToString(),
                                    Value = priority.ToString()
                                })
                                .ToList();



            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Title,Description,CreatedOn,Status,CategoryId,Priority")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                task.TaskId = Guid.NewGuid();
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", task.CategoryId);
            ViewBag.PriorityValues = Enum.GetValues(typeof(Priority))
                                .Cast<Priority>()
                                .Select(priority => new SelectListItem
                                {
                                    Text = priority.ToString(),
                                    Value = priority.ToString()
                                })
                                .ToList();
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", task.CategoryId);

            // Populate ViewBag.StatusValues
            ViewBag.StatusValues = Enum.GetValues(typeof(Status))
                                       .Cast<Status>()
                                       .Select(status => new SelectListItem
                                       {
                                           Text = status.ToString(),
                                           Value = ((int)status).ToString(),
                                           Selected = status == task.Status
                                       })
                                       .ToList();

            // Populate ViewBag.Categories
            ViewBag.Categories = _context.Categories
                                           .Select(category => new SelectListItem
                                           {
                                               Text = category.Name,
                                               Value = category.Id.ToString(),
                                               Selected = category.Id == task.CategoryId
                                           })
                                           .ToList();
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TaskId,Title,Description,CreatedOn,Status,CategoryId")] Models.Task task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", task.CategoryId);
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(Guid id)
        {
          return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
