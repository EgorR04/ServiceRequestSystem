using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRequestSystem.Data;

namespace ServiceRequestSystem.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");

            // Статистику бачать оператор, адміністратор і керівник
            if (roleId != 2 && roleId != 4 && roleId != 5)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.TotalRequests = await _context.ServiceRequests.CountAsync();

            ViewBag.NewRequests = await _context.ServiceRequests
                .CountAsync(r => r.RequestStatus != null && r.RequestStatus.StatusName == "Нова");

            ViewBag.InProgressRequests = await _context.ServiceRequests
                .CountAsync(r => r.RequestStatus != null && r.RequestStatus.StatusName == "У роботі");

            ViewBag.DoneRequests = await _context.ServiceRequests
                .CountAsync(r => r.RequestStatus != null && r.RequestStatus.StatusName == "Виконана");

            ViewBag.ClosedRequests = await _context.ServiceRequests
                .CountAsync(r => r.RequestStatus != null && r.RequestStatus.StatusName == "Закрита");

            var byStatus = await _context.ServiceRequests
                .Include(r => r.RequestStatus)
                .GroupBy(r => r.RequestStatus!.StatusName)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var byPriority = await _context.ServiceRequests
                .Include(r => r.RequestPriority)
                .GroupBy(r => r.RequestPriority!.PriorityName)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var byEngineer = await _context.ServiceRequests
                .Include(r => r.AssignedEngineer)
                .Where(r => r.AssignedEngineer != null)
                .GroupBy(r => r.AssignedEngineer!.FullName)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.ByStatus = byStatus;
            ViewBag.ByPriority = byPriority;
            ViewBag.ByEngineer = byEngineer;

            return View();
        }
    }
}
