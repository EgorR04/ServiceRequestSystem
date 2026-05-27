using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRequestSystem.Data;
using ServiceRequestSystem.Models;

namespace ServiceRequestSystem.Controllers
{
    public class ObjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        private int? GetCurrentRoleId()
        {
            return HttpContext.Session.GetInt32("RoleId");
        }

        private bool CanViewObjects()
        {
            var roleId = GetCurrentRoleId();

            // 2 — оператор, 3 — інженер, 4 — адміністратор, 5 — керівник
            return roleId == 2 || roleId == 3 || roleId == 4 || roleId == 5;
        }

        private bool IsAdmin()
        {
            return GetCurrentRoleId() == 4;
        }

        public async Task<IActionResult> Index(string? search, int? requestFilter)
        {
            if (GetCurrentUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanViewObjects())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var objectsQuery = _context.ServiceObjects
                .Include(o => o.ServiceRequests)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                objectsQuery = objectsQuery.Where(o =>
                    o.ObjectName.Contains(search) ||
                    o.Address.Contains(search) ||
                    (o.ContactPerson != null && o.ContactPerson.Contains(search)) ||
                    (o.ContactPhone != null && o.ContactPhone.Contains(search)) ||
                    (o.Description != null && o.Description.Contains(search))
                );
            }

            if (requestFilter == 1)
            {
                objectsQuery = objectsQuery.Where(o =>
                    o.ServiceRequests != null && o.ServiceRequests.Any());
            }

            if (requestFilter == 2)
            {
                objectsQuery = objectsQuery.Where(o =>
                    o.ServiceRequests == null || !o.ServiceRequests.Any());
            }

            ViewBag.Search = search;
            ViewBag.RequestFilter = requestFilter;

            var objects = await objectsQuery
                .OrderBy(o => o.ObjectName)
                .ToListAsync();

            return View(objects);
        }

        public IActionResult Create()
        {
            if (GetCurrentUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceObject serviceObject)
        {
            if (GetCurrentUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                _context.ServiceObjects.Add(serviceObject);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Об'єкт обслуговування успішно додано.";

                return RedirectToAction(nameof(Index));
            }

            return View(serviceObject);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (GetCurrentUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanViewObjects())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var serviceObject = await _context.ServiceObjects
                .Include(o => o.Equipment)
                .Include(o => o.ServiceRequests)
                    .ThenInclude(r => r.RequestStatus)
                .Include(o => o.ServiceRequests)
                    .ThenInclude(r => r.RequestPriority)
                .FirstOrDefaultAsync(o => o.ServiceObjectId == id);

            if (serviceObject == null)
            {
                return NotFound();
            }

            return View(serviceObject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (GetCurrentUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var serviceObject = await _context.ServiceObjects
                .Include(o => o.ServiceRequests)
                .FirstOrDefaultAsync(o => o.ServiceObjectId == id);

            if (serviceObject == null)
            {
                return NotFound();
            }

            return View(serviceObject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (GetCurrentUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var serviceObject = await _context.ServiceObjects
                .Include(o => o.ServiceRequests)
                .FirstOrDefaultAsync(o => o.ServiceObjectId == id);

            if (serviceObject == null)
            {
                return NotFound();
            }

            if (serviceObject.ServiceRequests != null && serviceObject.ServiceRequests.Any())
            {
                TempData["Error"] = "Об'єкт не можна видалити, оскільки з ним пов'язані сервісні заявки.";
                return RedirectToAction(nameof(Index));
            }

            _context.ServiceObjects.Remove(serviceObject);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Об'єкт обслуговування успішно видалено.";

            return RedirectToAction(nameof(Index));
        }
    }
}