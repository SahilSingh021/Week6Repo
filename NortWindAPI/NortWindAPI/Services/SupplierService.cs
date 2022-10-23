using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NortWindAPI.Controllers;
using NortWindAPI.Models;
using NortWindAPI.Models.DTO;

namespace NortWindAPI.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly NorthwindContext _context;

        public SupplierService() { }
        public SupplierService(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<SupplierDTO>>> RetrieveSuppliersAsync()
        {
            var suppliers = await _context.Suppliers.Include(x => x.Products).Select(x => Utils.SupplierToDTO(x)).ToListAsync();
            return suppliers;
        }
        public async Task<ActionResult<IEnumerable<ProductDTO>>> RetrieveProductsWithIdAsync(int id)
        {
            var products = await _context.Products.Where(p => p.SupplierId == id).Select(p => Utils.ProductToDTO(p)).ToListAsync();
            return products;
        }

        public async Task<Supplier> RetrieveSupplierWithIdAsync(int id)
        {
            var suppliers = await _context.Suppliers.Where(c => c.SupplierId == id).Include(s => s.Products)
                .FirstOrDefaultAsync();
            return suppliers;
        }

        public async Task AddProductsToRangeAsync(List<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
        }

        public async Task AddSupplierToRangeAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
        }

        public async Task SetEntryState(Supplier supplier, EntityState state)
        {
            _context.Entry(supplier).State = state;
        }

        public async Task<SupplierDTO> GetSupplierIdToAsync(Supplier supplier)
        {
            return await _context.Suppliers.Where(s => s.SupplierId == supplier.SupplierId)
                .Include(p => p.Products)
                .Select(x => Utils.SupplierToDTO(x))
                .FirstOrDefaultAsync();
        }

        public async Task<Supplier> FindSuppliersWithIdAsync(int id)
        {
            var result = await _context.Suppliers.FindAsync(id);
            return result;
        }

        public async Task RemoveSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
        }

        public bool ReturnIfSupplierIdExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }

        public async Task SaveDBChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
