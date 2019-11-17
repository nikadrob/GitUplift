using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models.ViewModels;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;


        [BindProperty]
        public ServiceVM ServVM { get; set; }

        public ServiceController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ServVM = new ServiceVM()
            {
                Service = new Models.Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropDown()
            };

            if (id != null)
            {
                ServVM.Service = _unitOfWork.Service.Get(id.GetValueOrDefault());
            }

            return View(ServVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                // New Service
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (ServVM.Service.Id == 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = webRootPath + @"\images\services\";
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(uploads + fileName + extension, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    ServVM.Service.ImageUrl = @"\images\services\" + fileName + extension;

                    _unitOfWork.Service.Add(ServVM.Service);
                }
                else
                {
                    //Edit Service
                    var objFromDb = _unitOfWork.Service.Get(ServVM.Service.Id);
                    if (files.Count > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = webRootPath + @"\images\services\";
                        var extension_new = Path.GetExtension(files[0].FileName);

                        var imagePath = webRootPath + objFromDb.ImageUrl.TrimStart('\\');
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                        using (var fileStream = new FileStream(uploads + fileName + extension_new, FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        ServVM.Service.ImageUrl = @"\images\services\" + fileName + extension_new;
                    }
                    else
                    {
                        ServVM.Service.ImageUrl = objFromDb.ImageUrl;
                    }

                    _unitOfWork.Service.Update(ServVM.Service);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                ServVM.CategoryList = _unitOfWork.Category.GetCategoryListForDropDown();
                ServVM.FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropDown();
                return View(ServVM);
            }
        }

        #region API call
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Service.GetAll(includeProperties: "Category,Frequency") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Service.Get(id);

            string webRootPath = _hostEnvironment.WebRootPath;

            var imagePath = webRootPath + "\\" + objFromDb.ImageUrl.TrimStart('\\');
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            if (objFromDb == null)
            {
                return Json(new { success = false, mesage = "Error while deleting" });
            }
            _unitOfWork.Service.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted" });
        }
        #endregion
    }
}