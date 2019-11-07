using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;

namespace Uplift.DataAccess.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<SelectListItem> GetCategoryListForDropDown()
        {
            return _dbContext.Category.Select(i => new SelectListItem() { 
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(Category category)
        {
            var objFromCategory = _dbContext.Category.FirstOrDefault(s => s.Id == category.Id);

            objFromCategory.Name = category.Name;
            objFromCategory.DisplayOrder = category.DisplayOrder;

            _dbContext.SaveChanges();
        }
    }
}
