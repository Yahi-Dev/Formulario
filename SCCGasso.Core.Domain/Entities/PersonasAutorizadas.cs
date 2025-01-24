namespace SCCGasso.Core.Domain.Entities
{
    public class PersonasAutorizadas
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Cedula { get; set; }


        //Navegation Property
        public int? IdFormulario { get; set; }
        public Formulario? Formulario { get; set; }
    }
}
