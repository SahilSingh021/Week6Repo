using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NortWindAPI.Models;
using NortWindAPI.Models.DTO;
using NortWindAPI.Services;

namespace NortWindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;
        private readonly ILogger _logger;

        public SuppliersController(ISupplierService service, ILogger<SuppliersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetSuppliers() => await _service.RetrieveSuppliersAsync();

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetSupplierWithProducts(int id)
        {
            if (!SupplierExists(id))
            {
                _logger.LogWarning($"No supplier with {id} id found.");
                return NotFound();
            }
            var suppliers = await _service.RetrieveProductsWithIdAsync(id);
            _logger.LogWarning($"Supplier with {id} id found.");
            return suppliers;
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplier(int id)
        {
            var supplier = await _service.RetrieveSupplierWithIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return Utils.SupplierToDTO(supplier);
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, SupplierDTO supplierDto)
        {
            if (id != supplierDto.SupplierId)
            {
                return BadRequest();
            }

            List<Product> products = new List<Product>();

            supplierDto.Products.ToList().ForEach(x => products.Add(new Product { ProductName = x.ProductName, UnitPrice = x.UnitPrice }));

            await _service.AddProductsToRangeAsync(products);
            await _service.SaveDBChangesAsync();

            Supplier supplier = new Supplier
            {
                SupplierId = id,
                CompanyName = supplierDto.CompanyName,
                ContactName = supplierDto.ContactName,
                ContactTitle = supplierDto.ContactTitle,
                Country = supplierDto.Country,
                Products = products
            };

            await _service.AddSupplierAsync(supplier);


            await _service.SetEntryState(supplier, EntityState.Modified);

            try
            {
                await _service.SaveDBChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> PostSupplier(SupplierDTO supplierDto)
        {
            List<Product> products = new List<Product>();

            supplierDto.Products.ToList().ForEach(x => products.Add(new Product { ProductName = x.ProductName, UnitPrice = x.UnitPrice }));

            await _service.AddProductsToRangeAsync(products);
            await _service.SaveDBChangesAsync();

            Supplier supplier = new Supplier
            {
                SupplierId = supplierDto.SupplierId,
                CompanyName = supplierDto.CompanyName,
                ContactName = supplierDto.ContactName,
                ContactTitle = supplierDto.ContactTitle,
                Country = supplierDto.Country,
                Products = products
            };

            await _service.AddSupplierToRangeAsync(supplier);
            await _service.SaveDBChangesAsync();

            supplierDto = await _service.GetSupplierIdToAsync(supplier);

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.SupplierId }, supplierDto);
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _service.FindSuppliersWithIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            await _service.RemoveSupplierAsync(supplier);
            await _service.SaveDBChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(int id)
        {
            return _service.ReturnIfSupplierIdExists(id);
        }
    }
}
