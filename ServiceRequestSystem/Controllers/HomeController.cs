using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRequestSystem.Data;
using ServiceRequestSystem.Models;
using System.Diagnostics;

namespace ServiceRequestSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var roleId = HttpContext.Session.GetInt32("RoleId");

            if (userId == null || roleId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var requestsQuery = _context.ServiceRequests.AsQueryable();

            // Клієнт бачить статистику тільки за власними заявками
            if (roleId == 1)
            {
                requestsQuery = requestsQuery.Where(r => r.CreatedByUserId == userId.Value);
            }

            // Інженер бачить статистику тільки за призначеними заявками
            if (roleId == 3)
            {
                requestsQuery = requestsQuery.Where(r => r.AssignedEngineerId == userId.Value);
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.RoleName = HttpContext.Session.GetString("RoleName");
            ViewBag.RoleId = roleId;

            ViewBag.TotalRequests = await requestsQuery.CountAsync();
            ViewBag.NewRequests = await requestsQuery.CountAsync(r => r.RequestStatusId == 1);
            ViewBag.InProgressRequests = await requestsQuery.CountAsync(r => r.RequestStatusId == 4);
            ViewBag.DoneRequests = await requestsQuery.CountAsync(r => r.RequestStatusId == 6);

            ViewBag.TotalObjects = await _context.ServiceObjects.CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
