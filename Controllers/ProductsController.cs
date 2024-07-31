using CRUD_Operations_Project.Models;
using CRUD_Operations_Project.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Operations_Project.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p=>p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile","Image File is Required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            //Save Image File
            string New_File_Name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            New_File_Name += Path.GetExtension(productDto.ImageFile!.FileName);

            string Image_Full_Path = environment.WebRootPath + "/products/" + New_File_Name;
            using (var stream = System.IO.File.Create(Image_Full_Path))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // Save Product (convert from ProductDto)

            Product product = new Product
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = Image_Full_Path,
                CreatedAt = DateTime.Now,

            };

            context.Products.Add(product);          // save to DB
            context.SaveChanges();

            return RedirectToAction("Index","Products"); 
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }


            ProductDto productDTO = new ProductDto
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");



            return View(productDTO);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            if(!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                return View(productDto);
            }


                //save file
            string new_File_Name = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                //Save Image File
                string New_File_Name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                New_File_Name += Path.GetExtension(productDto.ImageFile!.FileName);

                string Image_Full_Path = environment.WebRootPath + "/products/" + New_File_Name;
                using (var stream = System.IO.File.Create(Image_Full_Path))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // delete old image 
                string old_image_path = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(old_image_path);

            }

            // update products in database

            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = new_File_Name;


            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete(int id)
        {

            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            string image_path = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(image_path);

            context.Products.Remove(product);
            context.SaveChanges(true);
            return RedirectToAction("Index", "Products");
        }
    }
}
