namespace Sklad.Models
{
    public class Subtype
    {
        public int Id { get; set; }
        public string Type { get; set; }  // Typ, např. "ND" nebo "Material"
        public string Name { get; set; }  // Název podtypu, např. "Trafo", "Kabel"
    }
}
