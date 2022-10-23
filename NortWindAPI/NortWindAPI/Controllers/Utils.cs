﻿using NortWindAPI.Models;
using NortWindAPI.Models.DTO;

namespace NortWindAPI.Controllers
{
    public static class Utils
    {
        public static SupplierDTO SupplierToDTO(Supplier supplier) => new SupplierDTO
        {
            SupplierId = supplier.SupplierId,
            CompanyName = supplier.CompanyName,
            ContactTitle = supplier.ContactTitle,
            ContactName = supplier.ContactName,
            Country = supplier.Country,
            TotalProducts = supplier.Products.Count,
            Products = supplier.Products.Select(x => ProductToDTO(x)).ToList()
        };

        public static ProductDTO ProductToDTO(Product product) => new ProductDTO
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            SupplierId = product.SupplierId,
            CategoryId = product.CategoryId,
            UnitPrice = product.UnitPrice
        };
    }
}
