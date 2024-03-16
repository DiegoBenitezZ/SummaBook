using SummaBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SummaBook.DataAccess.Repository;
using SummaBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SummaBook.Utility;
using Microsoft.AspNetCore.Http;

namespace SummaBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count()
                );
            }

            IEnumerable<Product> products = _unitOfWork.Product.GetAll();
            return View(products);
        }

        public IActionResult Details(int? productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart shoppingCartOfDb = _unitOfWork.ShoppingCart.Get(u =>
                u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId
            );

            if(shoppingCartOfDb != null)
            {
                shoppingCartOfDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(shoppingCartOfDb);
                _unitOfWork.Save();
            } else {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
					_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count()
                );
            }

            
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
