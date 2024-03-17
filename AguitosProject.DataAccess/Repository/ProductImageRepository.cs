using SummaBook.DataAccess.Data;
using SummaBook.DataAccess.Repository.IRepository;
using SummaBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummaBook.DataAccess.Repository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
	{
		private readonly ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(IProductImageRepository obj)
		{
			_db.Update(obj);
		}
	}
}
