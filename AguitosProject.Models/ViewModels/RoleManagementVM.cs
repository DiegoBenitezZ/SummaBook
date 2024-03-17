using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummaBook.Models.ViewModels
{
	public class RoleManagementVM
	{
        public ApplicationUser applicationUser { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> roles { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> companies { get; set; }
		
	}
}
