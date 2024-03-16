using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SummaBook.DataAccess.Repository.IRepository;
using SummaBook.Models;
using SummaBook.Models.ViewModels;
using SummaBook.Utility;

namespace SummaBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(products);
        }

		public IActionResult Upsert (int? id)
		{
			ProductVM productVM = new ProductVM()
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = (id == null || id == 0) ? new Product() : _unitOfWork.Product.Get(u => u.Id == id)
			};

			return View(productVM);
		}

		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid) {
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				
				if(file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"img\product");

					if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

						if(System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\img\product\" + fileName;
				}

				_unitOfWork.Product.Update(productVM.Product);
				_unitOfWork.Save();
				TempData["success"] = "Product upsert successfully";

				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});

				return View(productVM);
			}
		}

   //     public IActionResult Create()
   //     {
   //         //ViewBag.CategoryList = CategoryList;
   //         //ViewData["CategoryList"] = CategoryList;
   //         ProductVM productVM = new ProductVM()
   //         {
   //             CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
			//	{
			//		Text = u.Name,
			//		Value = u.Id.ToString()
			//	}),
   //             Product = new Product()
   //         };
			//return View(productVM);
   //     }

   //     [HttpPost]
   //     public IActionResult Create(ProductVM productMV)
   //     {
   //         if(ModelState.IsValid)
   //         {
   //             _unitOfWork.Product.Add(productMV.Product);
   //             _unitOfWork.Save();
   //             TempData["success"] = "Product created successfully";
   //             return RedirectToAction("Index");
   //         } 
   //         else
   //         {
   //             productMV.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
   //             {
   //                 Text = u.Name,
   //                 Value = u.Id.ToString()
   //             });

			//	return View(productMV);
			//}

            
   //     }

   //     public IActionResult Edit(int id)
   //     {
   //         if(id == 0 || id == null)
   //         {
   //             return NotFound();
   //         }

   //         Product product = _unitOfWork.Product.Get(u => u.Id == id);

			//if (product == null)
			//{
			//	return NotFound();
			//}

			//return View(product);
   //     }

   //     [HttpPost]
   //     public IActionResult Edit(Product product)
   //     {
   //         if(ModelState.IsValid)
   //         {
   //             _unitOfWork.Product.Update(product);
   //             _unitOfWork.Save();
   //             TempData["success"] = "Product updated successfully";

   //             return RedirectToAction("Index");

			//}

   //         return View();
   //     }

  //      public IActionResult Delete(int id)
  //      {
		//	if (id == 0 || id == null)
		//	{
		//		return NotFound();
		//	}

		//	Product product = _unitOfWork.Product.Get(u => u.Id == id);

  //          if(product == null)
  //          {
		//		return NotFound();
		//	}

		//	return View(product);
		//}

  //      [HttpPost]
  //      public IActionResult Delete(int? id)
  //      {
		//	Product obj = _unitOfWork.Product.Get(u => u.Id == id);

		//	if (obj == null)
		//	{
		//		return NotFound();
		//	}

		//	if (ModelState.IsValid)
		//	{
		//		_unitOfWork.Product.Remove(obj);
		//		_unitOfWork.Save();

		//		TempData["success"] = "Product deleted successfully";
		//		return RedirectToAction("Index", "Product");
		//	}

		//	return View();

		//}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProductList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

			if(productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
