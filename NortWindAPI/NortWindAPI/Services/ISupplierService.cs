using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NortWindAPI.Models;
using NortWindAPI.Models.DTO;

namespace NortWindAPI.Services
{
    public interface ISupplierService
    {
        public Task<ActionResult<IEnumerable<SupplierDTO>>> RetrieveSuppliersAsync();
        public Task<ActionResult<IEnumerable<ProductDTO>>> RetrieveProductsWithIdAsync(int id);
        public Task<Supplier> RetrieveSupplierWithIdAsync(int id);
        public Task AddProductsToRangeAsync(List<Product> products);
        public Task AddSupplierAsync(Supplier supplier);
        public Task AddSupplierToRangeAsync(Supplier supplier);
        public Task SetEntryState (Supplier supplier, EntityState state);
        public Task<SupplierDTO> GetSupplierIdToAsync(Supplier supplier);
        public Task<Supplier> FindSuppliersWithIdAsync(int id);
        public Task RemoveSupplierAsync(Supplier supplier);
        public bool ReturnIfSupplierIdExists(int id);
        public Task SaveDBChangesAsync();

    }
}
