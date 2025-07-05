using apiCambiosMoneda.Dominio.Entidades;
using System.ComponentModel.DataAnnotations.Schema;


namespace apiCambiosMoneda.Dominio.Entidades
{
    [Table("cambiomoneda")]
    public class CambioMoneda
    {
        [Column("idmoneda")]
        public int IdMoneda { get; set; }
        public Moneda? Moneda { get; set; }
        [Column("fecha")]
        public DateTime Fecha { get; set; }
        [Column("cambio")]
        public double Cambio { get; set; }
    }
}
