using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklad.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CatalogNumber { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
    }
}
