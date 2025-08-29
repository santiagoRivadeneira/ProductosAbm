using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Productos.Entities
{

        public class Product
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Unidad { get; set; }
            public decimal Precio { get; set; }
            public bool Estado { get; set; }
            public DateTime FechaCreacion { get; set; }
            public DateTime? FechaModificacion { get; set; }
        }
    }

