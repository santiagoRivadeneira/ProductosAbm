using Productos.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Productos.Datos
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public (List<Product> products, int total) GetPaged(int pageNumber, int pageSize)
        {
            var products = new List<Product>();
            int total = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("GetProductosPaginados", conn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
                da.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                //sirve para cargar una tabla en memoria y cerra la conexion
                //es mas eficiente cuando son varias tablas
                //esto te permite crear relaciones,etc
                DataSet ds = new DataSet();
                da.Fill(ds);

                
                DataTable dtProducts = ds.Tables[0];
                foreach (DataRow row in dtProducts.Rows)
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Codigo = row["Codigo"].ToString(),
                        Nombre = row["Nombre"].ToString(),
                        Unidad = row["Unidad"].ToString(),
                        Precio = Convert.ToDecimal(row["Precio"]),
                        Estado = Convert.ToBoolean(row["Estado"]),
                        FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                        FechaModificacion = row["FechaModificacion"] == DBNull.Value
                                          ? (DateTime?)null
                                          : Convert.ToDateTime(row["FechaModificacion"])
                    });
                }

                // Segunda tabla: total
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    total = Convert.ToInt32(ds.Tables[1].Rows[0]["Total"]);
                }
            }

            return (products, total);
        }




        public Product GetById(int id)
        {
            Product product = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Codigo = reader["Codigo"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Unidad = reader["Unidad"].ToString(),
                            Precio = Convert.ToDecimal(reader["Precio"]),
                            Estado = Convert.ToBoolean(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaModificacion = reader["FechaModificacion"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["FechaModificacion"])
                        };
                    }
                }
            }

            return product;
        }

       
        public string Add(Product product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("InsertProducto", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Codigo", product.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", product.Nombre);
                cmd.Parameters.AddWithValue("@Unidad", product.Unidad);
                cmd.Parameters.AddWithValue("@Precio", product.Precio);
                cmd.Parameters.AddWithValue("@Estado", product.Estado);

                conn.Open();
                var result = cmd.ExecuteScalar(); 
                return result?.ToString() ?? "Producto agregado";
            }
        }

        public string Update(Product product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateProducto", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", product.Id);
                cmd.Parameters.AddWithValue("@Codigo", product.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", product.Nombre);
                cmd.Parameters.AddWithValue("@Unidad", product.Unidad);
                cmd.Parameters.AddWithValue("@Precio", product.Precio);
                cmd.Parameters.AddWithValue("@Estado", product.Estado);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? "Producto actualizado";
            }
        }

        public string Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteProducto", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                var result = cmd.ExecuteScalar(); 
                return result?.ToString() ?? "Producto eliminado";
            }
        }
    }
}
