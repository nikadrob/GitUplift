using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;

namespace Uplift.DataAccess.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Service service)
        {
            var objFromService = _dbContext.Service.FirstOrDefault(s => s.Id == service.Id);

            objFromService.Name = service.Name;
            objFromService.LongDesc = service.LongDesc;
            objFromService.ImageUrl = service.ImageUrl;
            objFromService.Price = service.Price;
            objFromService.CategoryId = service.CategoryId;
            objFromService.FrequencyId = service.FrequencyId;

            _dbContext.SaveChanges();
        }
    }
}
