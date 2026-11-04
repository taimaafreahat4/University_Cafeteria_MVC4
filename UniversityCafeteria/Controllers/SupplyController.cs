
using Microsoft.AspNetCore.Mvc;

namespace UniversityCafeteria.Controllers
{
    public class SupplyController : Controller
    {

        public IActionResult Index()
        {
            string[] categories =
            {
        "Drinks",
        "Food",
        "Snacks",
        "Hot Drinks",
        "Cold Drinks"
    };

            ViewBag.Categories = categories;

            return View();
        }



        public IActionResult Products()
        {
            ViewData["Title"] = "Cafeteria Products";

            ViewData["ManagerName"] = "Ahmad Ali";

            ViewData["NumberOfProducts"] = 5;

            string[] products =
            {
        "Water",
        "Orange Juice",
        "Coffee",
        "Sandwich",
        "Chocolate"
    };

            ViewData["Products"] = products;

            return View();
        }




        public IActionResult Suppliers()
        {
            ViewBag.Title = "Our Suppliers";
            ViewBag.NumberOfSuppliers = 3;

            string[] suppliers =
            {
        "Fresh Food Company",
        "Happy Drinks Company",
        "Daily Supplies Company"
    };

            ViewBag.Suppliers = suppliers;

            return View();
        }

        // GET: Display the form
        [HttpGet]
        public IActionResult CreateRequest()
        {
            return View();
        }

        // POST: Receive the form data

        [HttpPost]
        public IActionResult SubmitRequest()
        {
            string employeeName =
                Request.Form["employeeName"].ToString().Trim();

            string productName =
                Request.Form["productName"].ToString();

            string quantityText =
                Request.Form["quantity"].ToString();

            string requiredDate =
                Request.Form["requiredDate"].ToString();

            // Check Employee Name
            if (string.IsNullOrWhiteSpace(employeeName))
            {
                ViewBag.Error = "Employee Name is required!";
                return View("SubmitRequest");
            }

            // Check other fields
            int quantity;

            if (string.IsNullOrWhiteSpace(productName) ||
                !int.TryParse(quantityText, out quantity) ||
                quantity <= 0 ||
                string.IsNullOrWhiteSpace(requiredDate))
            {
                ViewBag.Error = "Please enter valid request information!";
                return View("SubmitRequest");
            }

            // Success Message

            string orderType;

            if (quantity < 20)
            {
                orderType = "Small Order";
            }
            else if (quantity <= 50)
            {
                orderType = "Medium Order";
            }
            else
            {
                orderType = "Large Order";
            }

            TempData["OrderType"] = orderType;

            TempData["Message"] =
                "Supply request submitted successfully!";

            return RedirectToAction("About");

        }


        // Display the About page
        public IActionResult About()
        {
            return View();
        }


        [HttpGet]
        public IActionResult CheckQuantity(int quantity)
        {
            string orderType;

            if (quantity < 20)
            {

                orderType = "Small Order";
            }
            else if (quantity <= 50)
            {
                orderType = "Medium Order";
            }
            else
            {
                orderType = "Large Order";
            }

            return Content(orderType);
        }

    }
}