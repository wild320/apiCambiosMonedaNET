using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiCambiosMoneda.Dominio.Entidades
{
    [Table("pais")]
    public class Pais
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("pais"), StringLength(50)]
        public string Nombre { get; set; }
        [Column("codigoalfa2"), StringLength(5)]
        public string CodigoAlfa2 { get; set; }
        [Column("codigoalfa3"), StringLength(5)]
        public string CodigoAlfa3 { get; set; }
        [Column("idmoneda")]
        public int IdMoneda { get; set; }
        public Moneda? Moneda { get; set; }
    }
}
