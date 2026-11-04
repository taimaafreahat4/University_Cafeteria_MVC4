
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniversityCafeteria.Models;

namespace UniversityCafeteria.Controllers
{
    public class SupplyRequestController : Controller
    {
        // List to store confirmed requests
        private static List<ConfirmedSupplyRequest> confirmedRequests = new();

        private static readonly object requestsLock = new object();
        private static int nextRequestId = 0;

        // Read draft from Session
        private List<SupplyItem> GetDraft()
        {
            string? data = HttpContext.Session.GetString("DraftItems");

            if (string.IsNullOrEmpty(data))
            {
                return new List<SupplyItem>();
            }

            return JsonSerializer.Deserialize<List<SupplyItem>>(data)
                   ?? new List<SupplyItem>();
        }

        // Save draft into Session
        private void SaveDraft(List<SupplyItem> items)
        {
            string data = JsonSerializer.Serialize(items);

            HttpContext.Session.SetString("DraftItems", data);
        }

        // Identify the current browser session
        private string GetEmployeeKey()
        {
            string? key = HttpContext.Session.GetString("EmployeeKey");

            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("EmployeeKey", key);
            }

            return key;
        }

        // Validate priority
        private bool IsValidPriority(string? priority)
        {
            return priority == "Normal"
                || priority == "Urgent"
                || priority == "Emergency";
        }

        // Determine fee using switch
        private decimal GetPreparationFee(string priority)
        {
            decimal fee;

            switch (priority)
            {
                case "Urgent":
                    fee = 5m;
                    break;

                case "Emergency":
                    fee = 10m;
                    break;

                default:
                    fee = 2m;
                    break;
            }

            return fee;
        }

        // All calculations happen in Controller
        private SupplyRequestSummary CalculateRequest(List<SupplyItem> items)
        {
            SupplyRequestSummary summary = new SupplyRequestSummary();

            summary.Items = items;

            decimal subtotal = 0;

            foreach (SupplyItem item in items)
            {
                subtotal += item.Quantity * item.UnitPrice;
            }

            subtotal = Math.Round(subtotal, 2);

            // Highest priority determines the request fee
            string priority = "Normal";

            if (items.Any(i => i.Priority == "Emergency"))
            {
                priority = "Emergency";
            }
            else if (items.Any(i => i.Priority == "Urgent"))
            {
                priority = "Urgent";
            }

            decimal fee = GetPreparationFee(priority);

            decimal discount = 0;

            if (subtotal > 200)
            {
                discount = subtotal * 0.15m;
            }
            else if (subtotal > 100)
            {
                discount = subtotal * 0.10m;
            }

            discount = Math.Round(discount, 2);

            decimal finalTotal = subtotal + fee - discount;

            summary.Subtotal = subtotal;
            summary.PreparationFee = fee;
            summary.Discount = discount;
            summary.FinalTotal = finalTotal;
            summary.Priority = priority;

            return summary;
        }

        // GET: SupplyRequest/Index
        [HttpGet]
        public IActionResult Index()
        {
            string? savedPriority =
                Request.Cookies["PreferredPriority"];

            if (!IsValidPriority(savedPriority))
            {
                savedPriority = "Normal";
            }

            SupplyItem item = new SupplyItem();
            item.Priority = savedPriority!;

            ViewBag.PreferredPriority = savedPriority;
            ViewBag.DraftCount = GetDraft().Count;

            return View(item);
        }

        // POST: SupplyRequest/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(SupplyItem item)
        {
            if (!IsValidPriority(item.Priority))
            {
                ModelState.AddModelError(
                    "Priority", "Invalid priority");
            }

            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                ModelState.AddModelError(
                    "ProductName", "Product name is required");
            }

            if (item.Quantity <= 0)
            {
                ModelState.AddModelError(
                    "Quantity", "Quantity must be greater than zero");
            }

            if (item.UnitPrice <= 0)
            {
                ModelState.AddModelError(
                    "UnitPrice", "Unit price must be greater than zero");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PreferredPriority =
                    Request.Cookies["PreferredPriority"] ?? "Normal";

                ViewBag.DraftCount = GetDraft().Count;

                return View("Index", item);
            }

            List<SupplyItem> items = GetDraft();

            items.Add(item);

            SaveDraft(items);

            TempData["Message"] = "Product added successfully!";

            return RedirectToAction("Index");
        }

        // GET: SupplyRequest/Review
        [HttpGet]
        public IActionResult Review()
        {
            List<SupplyItem> items = GetDraft();

            if (items.Count == 0)
            {
                TempData["Error"] = "Your supply request is empty.";

                return RedirectToAction("Index");
            }

            SupplyRequestSummary summary = CalculateRequest(items);

            return View(summary);
        }

        // POST: SupplyRequest/ConfirmRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmRequest()
        {
            List<SupplyItem> items = GetDraft();

            if (items.Count == 0)
            {
                TempData["Error"] = "Cannot confirm an empty request.";

                return RedirectToAction("Index");
            }

            SupplyRequestSummary summary = CalculateRequest(items);

            ConfirmedSupplyRequest request =
                new ConfirmedSupplyRequest();

            request.RequestID = Interlocked.Increment(ref nextRequestId);
            request.EmployeeKey = GetEmployeeKey();
            request.NumberOfProducts = items.Count;
            request.Subtotal = summary.Subtotal;
            request.PreparationFee = summary.PreparationFee;
            request.Discount = summary.Discount;
            request.FinalTotal = summary.FinalTotal;
            request.Status = "Confirmed";

            lock (requestsLock)
            {
                confirmedRequests.Add(request);
            }

            HttpContext.Session.Remove("DraftItems");

            TempData["Message"] = "Supply request confirmed successfully!";

            return RedirectToAction("MyRequests");
        }

        // POST: SupplyRequest/ClearDraft
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove("DraftItems");

            TempData["Message"] = "Draft cleared successfully.";

            return RedirectToAction("Index");
        }

        // POST: SupplyRequest/SavePreference
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePreference(string priority)
        {
            if (!IsValidPriority(priority))
            {
                TempData["Error"] = "Invalid priority.";
                return RedirectToAction("Index");
            }

            CookieOptions options = new CookieOptions();

            options.Expires = DateTimeOffset.UtcNow.AddDays(7);
            options.HttpOnly = true;
            options.IsEssential = true;
            options.SameSite = SameSiteMode.Lax;
            options.Secure = Request.IsHttps;

            Response.Cookies.Append(
                "PreferredPriority",
                priority,
                options
            );

            TempData["Message"] =
                "Your preferred request priority is " + priority + ".";

            return RedirectToAction("Index");
        }

        // GET: SupplyRequest/MyRequests
        [HttpGet]
        public IActionResult MyRequests()
        {
            string employeeKey = GetEmployeeKey();

            List<ConfirmedSupplyRequest> requests;

            lock (requestsLock)
            {
                requests = confirmedRequests
                    .Where(r => r.EmployeeKey == employeeKey)
                    .OrderByDescending(r => r.RequestID)
                    .ToList();
            }

            return View(requests);
        }

        // GET: SupplyRequest/About
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
