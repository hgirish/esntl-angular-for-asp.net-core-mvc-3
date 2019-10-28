using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;
using System.Collections.Generic;

namespace ServerApp.Controllers
{
    [Route("api/suppliers")]
    [ApiController]
    public class SupplierValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public SupplierValuesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Supplier> GetSuppliers()
        {
            return _context.Suppliers;
        }

        [HttpPost]
        public IActionResult CreateSupplier(SupplierData sData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier s = sData.Supplier;
            _context.Add(s);
            _context.SaveChanges();
            return Ok(s.SupplierId);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceSupplier(long id, SupplierData sData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Supplier s = sData.Supplier;
            s.SupplierId = id;
            _context.Update(s);
            _context.SaveChanges();
            return Ok();
        }
    }
}