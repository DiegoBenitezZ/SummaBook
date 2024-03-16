using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummaBook.DataAccess.Data;
using SummaBook.DataAccess.Repository.IRepository;
using SummaBook.Models;
using SummaBook.Utility;

namespace SummaBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(ApplicationDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Company> companies = _unitOfWork.Company.GetAll();
            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            Company company = (id == 0 || id == null) ? new Company() : _unitOfWork.Company.Get(u => u.Id == id); 
            return View(company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Company.Update(company);
                _unitOfWork.Save();
				TempData["success"] = "Company upsert successfully";

				return RedirectToAction("Index");
			}

            return View();
		}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Company> companies = _unitOfWork.Company.GetAll();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);

            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
			}

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}
        #endregion
    }
}
