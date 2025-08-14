using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            }; 
            if(id == null || id == 0)
            {
                // Create Product
                return View(productVM);
            }
            else
            {
                // Update Product
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);

            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {



            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // If a file is uploaded, we need to process it
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product added successfully";
                return RedirectToAction("Index");

            }
            else
            {
                // If the model state is invalid, we need to repopulate the CategoryList
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id); //This is one way to show how you can do it

      
            if (ProductFromDb == null)
            {
                return NotFound();
            }

            return View(ProductFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
