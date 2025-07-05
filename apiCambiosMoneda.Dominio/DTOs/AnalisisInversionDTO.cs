namespace apiCambiosMoneda.Dominio.DTOs
{
    public class AnalisisInversionDTO
    {
        public string SiglaMonedaA { get; set; } = "";
        public string SiglaMonedaB { get; set; } = "";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Recomendacion { get; set; } = "";
        public double CambioMonedaAInicio { get; set; }
        public double CambioMonedaBInicio { get; set; }
        public double CambioMonedaAFin { get; set; }
        public double CambioMonedaBFin { get; set; }
    }


}
