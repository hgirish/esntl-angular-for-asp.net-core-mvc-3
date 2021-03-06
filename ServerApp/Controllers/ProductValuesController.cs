using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ServerApp.Models.BindingTargets;
using Microsoft.AspNetCore.JsonPatch;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ServerApp.Controllers
{

    [Route("api/products")]
    [Authorize(Roles = "Administrator")]
    [ApiController][AutoValidateAntiforgeryToken]
    public class ProductValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductValuesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")][AllowAnonymous]
        public Product GetProduct(long id)
        {
            //System.Threading.Thread.Sleep(5000);
            Product result =  _context.Products
                .Include(p => p.Supplier).ThenInclude(s=>s.Products)
                .Include(p => p.Ratings)
                .FirstOrDefault(p => p.ProductId == id);

            if (result != null)
            {
                if (result.Supplier != null)
                {
                    result.Supplier.Products =
                        result.Supplier.Products.Select(p => new Product
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Category = p.Category,
                            Description = p.Description,
                            Price = p.Price
                        });
                }

                if (result.Ratings != null)
                {
                    foreach (var r in result.Ratings)
                    {
                        r.Product = null; 
                    }
                }
            }
            return result;

        }
        [HttpGet][AllowAnonymous]
        public IActionResult GetProducts(
            string category, string search,
            bool related = false, bool metadata = false)
        {
            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrWhiteSpace(category))
            {
                string catLower = category.ToLower();
                query = query.Where(p => p.Category.ToLower().Contains(catLower));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower));
            }

            if (related && HttpContext.User.IsInRole("Administrator"))
            {
                query = query.Include(p => p.Supplier)
                    .Include(p => p.Ratings);
                List<Product> data = query.ToList();
                data.ForEach(p =>
                {
                    if (p.Supplier != null)
                    {
                        p.Supplier.Products = null;
                    }
                    if (p.Ratings != null)
                    {
                        p.Ratings.ForEach(r => r.Product = null);
                    }
                });
                return metadata ? CreateMetadata(data) : Ok(data);
            }
            else
            {
                return metadata ? CreateMetadata(query) : Ok(query);
            }
        }

        private IActionResult CreateMetadata(IEnumerable<Product> products)
        {
            return Ok(new
            {
                data = products,
                categories = _context.Products.Select(p => p.Category)
                .Distinct().OrderBy(c => c)
            });
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductData pData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Product p = pData.Product;
            if (p.Supplier != null && p.Supplier.SupplierId != 0)
            {
                _context.Attach(p.Supplier);
            }
            _context.Add(p);
            _context.SaveChanges();
            return Ok(p.ProductId);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceProduct(long id, ProductData pData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Product p = pData.Product;
            p.ProductId = id;
            if (p.Supplier != null && p.Supplier.SupplierId != 0)
            {
                _context.Attach(p.Supplier);
            }
            _context.Update(p);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateProduct(long id, 
            JsonPatchDocument<ProductData> patch)
        {
            Product product = _context.Products
                .Include(p => p.Supplier)
                .First(p => p.ProductId == id);
            ProductData pData = new ProductData { Product = product };

            patch.ApplyTo(pData, ModelState);

            if (ModelState.IsValid && TryValidateModel(pData))
            {
                if (product.Supplier != null && product.Supplier.SupplierId  != 0)
                {
                    _context.Attach(product.Supplier);
                }
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public void DeleteProduct(long id)
        {
            _context.Products.Remove(new Product { ProductId = id });
            _context.SaveChanges();
        }
    }
}
