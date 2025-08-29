using Productos.Datos;
using Productos.Entities;
using System;
using System.Collections.Generic;

namespace Productos.Negocio
{
    public class ProductService
    {
        private readonly ProductRepository _ProductoRepositorio;

        public ProductService(string connectionString = "Server=TIAGO;Database=TiendaDB;Trusted_Connection=True;TrustServerCertificate=True;")
        {
            _ProductoRepositorio = new ProductRepository(connectionString);
        }

        public (List<Product> products, int total) GetProductsPaged(int pageNumber, int pageSize)
        {
            try
            {
                return _ProductoRepositorio.GetPaged(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return (new List<Product>(), 0);
            }
        }

        
        public Product GetProduct(int id)
        {
            try
            {
                return _ProductoRepositorio.GetById(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Codigo)) return "El código es obligatorio.";
            if (string.IsNullOrWhiteSpace(product.Nombre)) return "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(product.Unidad)) return "La unidad es obligatoria.";
            if (product.Precio < 0) return "El precio no puede ser negativo.";

            try
            {
                _ProductoRepositorio.Add(product);
                return "Producto agregado correctamente.";
            }
            catch (Exception ex)
            {
                return $"Error al agregar producto: {ex.Message}";
            }
        }

        public string UpdateProduct(Product product)
        {
            if (product.Id <= 0) return "El Id del producto es inválido.";
            if (string.IsNullOrWhiteSpace(product.Codigo)) return "El código es obligatorio.";
            if (string.IsNullOrWhiteSpace(product.Nombre)) return "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(product.Unidad)) return "La unidad es obligatoria.";
            if (product.Precio < 0) return "El precio no puede ser negativo.";

            try
            {
                _ProductoRepositorio.Update(product);
                return "Producto actualizado correctamente.";
            }
            catch (Exception ex)
            {
                return $"Error al actualizar producto: {ex.Message}";
            }
        }

        public string DeleteProduct(int id)
        {
            if (id <= 0) return "El Id del producto es inválido.";

            try
            {
                _ProductoRepositorio.Delete(id);
                return "Producto eliminado correctamente.";
            }
            catch (Exception ex)
            {
                return $"Error al eliminar producto: {ex.Message}";
            }
        }
    }
}
