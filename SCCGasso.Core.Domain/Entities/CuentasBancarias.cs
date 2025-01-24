namespace SCCGasso.Core.Domain.Entities
{
    public class CuentasBancarias
    {
        public int Id { get; set; }
        public string? Banco { get; set; }
        public string? CuentaNo { get; set; }
        public string? Sucursal { get; set; }


        //Navegation Property
        public int? IdFormulario { get; set; }
        public Formulario? Formulario { get; set; }
    }
}
