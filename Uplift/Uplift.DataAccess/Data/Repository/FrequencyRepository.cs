using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;

namespace Uplift.DataAccess.Data.Repository
{
    public class FrequencyRepository : Repository<Frequency>, IFrequencyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FrequencyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<SelectListItem> GetFrequencyListForDropDown()
        {
            return _dbContext.Frequency.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(Frequency frequency)
        {
            var objFromFrequency = _dbContext.Frequency.FirstOrDefault(i => i.Id == frequency.Id);

            objFromFrequency.Name = frequency.Name;
            objFromFrequency.FrequencyCount = frequency.FrequencyCount;

            _dbContext.SaveChanges();
        }
    }
}
