using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceRequestSystem.Data;
using ServiceRequestSystem.Models;
using Microsoft.AspNetCore.Http;

namespace ServiceRequestSystem.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
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

        public async Task<IActionResult> Index(string? search, int? statusId, int? priorityId, int? objectId)
        {
            var userId = GetCurrentUserId();
            var roleId = GetCurrentRoleId();

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.ServiceRequests
                .Include(r => r.ServiceObject)
                .Include(r => r.CreatedByUser)
                .Include(r => r.AssignedEngineer)
                .Include(r => r.RequestStatus)
                .Include(r => r.RequestPriority)
                .AsQueryable();

            // Клієнт бачить тільки власні заявки
            if (roleId == 1)
            {
                query = query.Where(r => r.CreatedByUserId == userId.Value);
            }

            // Інженер бачить тільки призначені йому заявки
            if (roleId == 3)
            {
                query = query.Where(r => r.AssignedEngineerId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Title.Contains(search) ||
                    r.Description.Contains(search));
            }

            if (statusId.HasValue)
            {
                query = query.Where(r => r.RequestStatusId == statusId.Value);
            }

            if (priorityId.HasValue)
            {
                query = query.Where(r => r.RequestPriorityId == priorityId.Value);
            }

            if (objectId.HasValue)
            {
                query = query.Where(r => r.ServiceObjectId == objectId.Value);
            }

            ViewBag.Search = search;

            ViewBag.Statuses = new SelectList(
                _context.RequestStatuses.ToList(),
                "RequestStatusId",
                "StatusName",
                statusId
            );

            ViewBag.Priorities = new SelectList(
                _context.RequestPriorities.ToList(),
                "RequestPriorityId",
                "PriorityName",
                priorityId
            );

            ViewBag.Objects = new SelectList(
                _context.ServiceObjects.ToList(),
                "ServiceObjectId",
                "ObjectName",
                objectId
            );

            var requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        public IActionResult Create()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            LoadSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            var userId = GetCurrentUserId();
            var roleId = GetCurrentRoleId();

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                request.CreatedAt = DateTime.Now;
                request.UpdatedAt = DateTime.Now;
                request.CreatedByUserId = userId.Value;

                // Якщо заявку створює клієнт, виконавець не призначається
                if (roleId == 1)
                {
                    request.AssignedEngineerId = null;
                    request.RequestStatusId = 1; // Нова
                }
                else
                {
                    request.RequestStatusId = request.AssignedEngineerId != null ? 3 : 1; // Призначена або Нова
                }

                _context.ServiceRequests.Add(request);

                _context.RequestHistory.Add(new RequestHistory
                {
                    ServiceRequest = request,
                    UserId = userId.Value,
                    ActionType = "Створення заявки",
                    OldValue = null,
                    NewValue = "Нова заявка",
                    CreatedAt = DateTime.Now
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadSelectLists();
            return View(request);
        }

        private void LoadSelectLists()
        {
            ViewBag.ServiceObjects = new SelectList(
                _context.ServiceObjects.ToList(),
                "ServiceObjectId",
                "ObjectName"
            );

            ViewBag.Users = new SelectList(
                _context.Users.ToList(),
                "UserId",
                "FullName"
            );

            ViewBag.Engineers = new SelectList(
                _context.Users.Where(u => u.RoleId == 3).ToList(),
                "UserId",
                "FullName"
            );

            ViewBag.Priorities = new SelectList(
                _context.RequestPriorities.ToList(),
                "RequestPriorityId",
                "PriorityName"
            );
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            var roleId = GetCurrentRoleId();

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _context.ServiceRequests
                .Include(r => r.ServiceObject)
                .Include(r => r.CreatedByUser)
                .Include(r => r.AssignedEngineer)
                .Include(r => r.RequestStatus)
                .Include(r => r.RequestPriority)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.User)
                .Include(r => r.History)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Клієнт може переглядати лише власні заявки
            if (roleId == 1 && request.CreatedByUserId != userId.Value)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Інженер може переглядати лише призначені йому заявки
            if (roleId == 3 && request.AssignedEngineerId != userId.Value)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.Statuses = new SelectList(
                _context.RequestStatuses.ToList(),
                "RequestStatusId",
                "StatusName",
                request.RequestStatusId
            );

            ViewBag.Engineers = new SelectList(
                _context.Users.Where(u => u.RoleId == 3).ToList(),
                "UserId",
                "FullName",
                request.AssignedEngineerId
            );

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEngineer(int id, int assignedEngineerId)
        {
            var userId = GetCurrentUserId();
            var roleId = GetCurrentRoleId();

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Призначати виконавця можуть оператор, адміністратор або керівник
            if (roleId != 2 && roleId != 4 && roleId != 5)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var request = await _context.ServiceRequests
                .Include(r => r.AssignedEngineer)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var oldEngineer = request.AssignedEngineer?.FullName ?? "Не призначено";

            var newEngineer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == assignedEngineerId && u.RoleId == 3);

            if (newEngineer == null)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            request.AssignedEngineerId = assignedEngineerId;
            request.UpdatedAt = DateTime.Now;

            var assignedStatus = await _context.RequestStatuses
                .FirstOrDefaultAsync(s => s.StatusName == "Призначена");

            if (assignedStatus != null)
            {
                request.RequestStatusId = assignedStatus.RequestStatusId;
            }

            _context.RequestHistory.Add(new RequestHistory
            {
                ServiceRequestId = id,
                UserId = userId.Value,
                ActionType = "Призначення виконавця",
                OldValue = oldEngineer,
                NewValue = newEngineer.FullName,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, int requestStatusId)
        {
            var userId = GetCurrentUserId();
            var roleId = GetCurrentRoleId();

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _context.ServiceRequests
                .Include(r => r.RequestStatus)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Змінювати статус може оператор або призначений інженер
            bool isOperator = roleId == 2;
            bool isAssignedEngineer = roleId == 3 && request.AssignedEngineerId == userId.Value;

            if (!isOperator && !isAssignedEngineer)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var oldStatus = await _context.RequestStatuses
                .FirstOrDefaultAsync(s => s.RequestStatusId == request.RequestStatusId);

            var newStatus = await _context.RequestStatuses
                .FirstOrDefaultAsync(s => s.RequestStatusId == requestStatusId);

            request.RequestStatusId = requestStatusId;
            request.UpdatedAt = DateTime.Now;

            _context.RequestHistory.Add(new RequestHistory
            {
                ServiceRequestId = id,
                UserId = userId.Value,
                ActionType = "Зміна статусу",
                OldValue = oldStatus?.StatusName,
                NewValue = newStatus?.StatusName,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResult(int id, string resultText)
        {
            var userId = GetCurrentUserId();
            var roleId = GetCurrentRoleId();

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _context.ServiceRequests
                .Include(r => r.RequestStatus)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Фіксувати результат може тільки призначений сервісний інженер
            bool isAssignedEngineer = roleId == 3 && request.AssignedEngineerId == userId.Value;

            if (!isAssignedEngineer)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (!string.IsNullOrWhiteSpace(resultText))
            {
                var oldStatus = await _context.RequestStatuses
                    .FirstOrDefaultAsync(s => s.RequestStatusId == request.RequestStatusId);

                var doneStatus = await _context.RequestStatuses
                    .FirstOrDefaultAsync(s => s.StatusName == "Виконана");

                request.Result = resultText;
                request.UpdatedAt = DateTime.Now;

                if (doneStatus != null)
                {
                    request.RequestStatusId = doneStatus.RequestStatusId;
                }

                _context.RequestHistory.Add(new RequestHistory
                {
                    ServiceRequestId = id,
                    UserId = userId.Value,
                    ActionType = "Фіксація результату виконання",
                    OldValue = null,
                    NewValue = resultText,
                    CreatedAt = DateTime.Now
                });

                if (doneStatus != null)
                {
                    _context.RequestHistory.Add(new RequestHistory
                    {
                        ServiceRequestId = id,
                        UserId = userId.Value,
                        ActionType = "Зміна статусу",
                        OldValue = oldStatus?.StatusName,
                        NewValue = doneStatus.StatusName,
                        CreatedAt = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string commentText)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(commentText))
            {
                _context.RequestComments.Add(new RequestComment
                {
                    ServiceRequestId = id,
                    UserId = userId.Value,
                    CommentText = commentText,
                    CreatedAt = DateTime.Now
                });

                _context.RequestHistory.Add(new RequestHistory
                {
                    ServiceRequestId = id,
                    UserId = userId.Value,
                    ActionType = "Додано коментар",
                    OldValue = null,
                    NewValue = commentText,
                    CreatedAt = DateTime.Now
                });

                request.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private bool CanDeleteRequest()
        {
            var roleId = GetCurrentRoleId();

            // 2 — Оператор, 4 — Адміністратор, 5 — Керівник
            return roleId == 2 || roleId == 4 || roleId == 5;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanDeleteRequest())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var request = await _context.ServiceRequests
                .Include(r => r.ServiceObject)
                .Include(r => r.CreatedByUser)
                .Include(r => r.AssignedEngineer)
                .Include(r => r.RequestStatus)
                .Include(r => r.RequestPriority)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!CanDeleteRequest())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Заявку успішно видалено.";

            return RedirectToAction(nameof(Index));
        }
    }
}