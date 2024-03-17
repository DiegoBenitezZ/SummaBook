using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SummaBook.DataAccess.Data;
using SummaBook.DataAccess.Repository.IRepository;
using SummaBook.Models;
using SummaBook.Models.ViewModels;
using SummaBook.Utility;

namespace SummaBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
		public RoleManagementVM roleManagementVM {  get; set; }


        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ActionName("Role")]
        public IActionResult RoleManagement(string userId)
        {

            roleManagementVM = new()
            {
                applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId),
                roles = _roleManager.Roles.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Name,
                }),
                companies = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                })
			};

            roleManagementVM.applicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();

			return View(roleManagementVM);
        }

        [HttpPost]
		[ActionName("Role")]
		public IActionResult UpdateRole(RoleManagementVM roleManagementVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementVM.applicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementVM.applicationUser.Id);

            if (ModelState.IsValid && roleManagementVM.applicationUser.Role != oldRole)
            {
                // a role was updated

                if (roleManagementVM.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.applicationUser.CompanyId;
                }

                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.applicationUser.Role).GetAwaiter().GetResult();

                TempData["success"] = "User was updated successfully";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagementVM.applicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagementVM.applicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(roleManagementVM);
        }

		#region API CALLS
		public IActionResult GetAll()
        {
            List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach(var user in users)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if(user.Company == null)
                {
                    user.Company = new() { Name = "N/A" };
                }

                
            }

            return Json(new { data = users });
        }

        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            
            if(objFromDb == null)
            {
                return Json(new {success = false, message = "Error while Locking/Unlocking" });
            }

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            } else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Operation Successfully" });
        }
        #endregion
    }
}
