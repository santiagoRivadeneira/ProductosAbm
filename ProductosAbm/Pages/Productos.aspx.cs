using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Services;
using Productos.Entities;
using Productos.Negocio;

namespace ProductosAbm.Pages
{
    public partial class Productos : System.Web.UI.Page
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["TiendaDbConnection"].ConnectionString;
        }

        [WebMethod]
        public static object GetProductos(int pageNumber, int pageSize)
        {
            try
            {
                var service = new ProductService(GetConnectionString());
                var result = service.GetProductsPaged(pageNumber, pageSize);

                return new
                {
                    products = result.products,
                    total = result.total
                };
            }
            catch
            {
                return new
                {
                    products = new List<Product>(),
                    total = 0
                };
            }
        }


        [WebMethod]
        public static string AddProducto(Product product)
        {
            try
            {
                var service = new ProductService(GetConnectionString());
                return service.AddProduct(product); 
            }
            catch (Exception ex)
            {
                return $"Error al agregar producto: {ex.Message}";
            }
        }

        [WebMethod]
        public static string UpdateProducto(Product product)
        {
            try
            {
                var service = new ProductService(GetConnectionString());
                return service.UpdateProduct(product);
            }
            catch (Exception ex)
            {
                return $"Error al actualizar producto: {ex.Message}";
            }
        }

        [WebMethod]
        public static string DeleteProducto(int id)
        {
            try
            {
                var service = new ProductService(GetConnectionString());
                return service.DeleteProduct(id);
            }
            catch (Exception ex)
            {
                return $"Error al eliminar producto: {ex.Message}";
            }
        }
    }
}
