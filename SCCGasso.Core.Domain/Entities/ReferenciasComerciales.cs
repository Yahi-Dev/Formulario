namespace SCCGasso.Core.Domain.Entities
{
    public class ReferenciasComerciales
    {
        public int Id { get; set; }
        public string? Empresa { get; set; }
        public string? Telefono { get; set; }


        //Navegation Property
        public int? IdFormulario { get; set; }
        public Formulario? Formulario { get; set; }
    }
}
