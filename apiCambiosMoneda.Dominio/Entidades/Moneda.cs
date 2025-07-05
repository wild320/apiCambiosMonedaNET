using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiCambiosMoneda.Dominio.Entidades
{
    [Table("moneda")]
    public class Moneda
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("moneda"), StringLength(100)]
        public String Nombre { get; set; }

        [Column("sigla"), StringLength(5)]
        public String Sigla { get; set; }

        [Column("emisor"), StringLength(100)]
        public String? Emisor { get; set; }

        [Column("simbolo"), StringLength(5)]
        public String? Simbolo { get; set; }


    }
}
