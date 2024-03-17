using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummaBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; set; }
        IProductRepository Product { get; set; }
        ICompanyRepository Company { get; set; }
        IShoppingCartRepository ShoppingCart { get; set; }
        IOrderHeaderRepository OrderHeader { get; set; }
        IOrderDetailRepository OrderDetail { get; set; }
        IProductImageRepository ProductImage { get; set; }

        IApplicationUserRepository ApplicationUser { get; set; }

        void Save();
    }
}
