using apiCambiosMoneda.Core.Interfaces.Repositorios;
using apiCambiosMoneda.Core.Interfaces.Servicios;
using apiCambiosMoneda.Dominio.DTOs;
using apiCambiosMoneda.Dominio.Entidades;

namespace apiCambiosMoneda.Aplicacion.Servicios
{
    public class MonedaServicio : IMonedaServicio
    {
        private readonly IMonedaRepositorio repositorio;

        public MonedaServicio(IMonedaRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public async Task<Moneda> Obtener(int Id)
        {
            return await repositorio.Obtener(Id);
        }

        public async Task<IEnumerable<Moneda>> ObtenerTodos()
        {
            return await repositorio.ObtenerTodos();
        }
        public async Task<IEnumerable<Moneda>> Buscar(int Tipo, string Dato)
        {
            return await repositorio.Buscar(Tipo, Dato);
        }

        public async Task<Moneda> Agregar(Moneda Moneda)
        {
            return await repositorio.Agregar(Moneda);
        }

        public async Task<Moneda> Modificar(Moneda Moneda)
        {
            return await repositorio.Modificar(Moneda);
        }

        public async Task<bool> Eliminar(int Id)
        {
            return await repositorio.Eliminar(Id);
        }

        /********** CAMBIOS **********/
        public async Task<IEnumerable<CambioMoneda>> ObtenerCambios(int IdMoneda)
        {
            return await repositorio.ObtenerCambios(IdMoneda);
        }

        public async Task<CambioMoneda> BuscarCambio(int IdMoneda, DateTime Fecha)
        {
            return await repositorio.BuscarCambio(IdMoneda, Fecha);
        }

        public async Task<CambioMoneda> AgregarCambio(CambioMoneda Cambio)
        {
            return await repositorio.AgregarCambio(Cambio);
        }

        public async Task<CambioMoneda> ModificarCambio(CambioMoneda Cambio)
        {
            return await repositorio.ModificarCambio(Cambio);
        }

        public async Task<bool> EliminarCambio(int IdMoneda, DateTime Fecha)
        {
            return await repositorio.EliminarCambio(IdMoneda, Fecha);
        }

        /********** CONSULTAS **********/

        public async Task<CambioMoneda> ObtenerCambioActual(int IdMoneda)
        {
            return await repositorio.ObtenerCambioActual(IdMoneda);
        }

        public async Task<IEnumerable<CambioMoneda>> ObtenerHistorialCambios(int IdMoneda, DateTime Desde, DateTime Hasta)
        {
            return await repositorio.ObtenerHistorialCambios(IdMoneda, Desde, Hasta);
        }

        public async Task<IEnumerable<Pais>> ObtenerPaisesPorMoneda(int IdMoneda)
        {
            return await repositorio.ObtenerPaisesPorMoneda(IdMoneda);
        }

        /********** ANALISIS **********/

        public async Task<IEnumerable<AnalisisInversionDTO>> AnalizarInversionEntreMonedas(string SiglaMonedaA, string SiglaMonedaB,
                                                                                        DateTime Desde, DateTime Hasta)
        {
            var monedas = await repositorio.Buscar(1, SiglaMonedaA.ToLower());
            var monedaA = monedas.FirstOrDefault(m => m.Sigla.ToLower() == SiglaMonedaA.ToLower());

            monedas = await repositorio.Buscar(1, SiglaMonedaB.ToLower());
            var monedaB = monedas.FirstOrDefault(m => m.Sigla.ToLower() == SiglaMonedaB.ToLower());

            if (monedaA == null || monedaB == null)
                return Enumerable.Empty<AnalisisInversionDTO>();

            var cambiosA = await repositorio.ObtenerHistorialCambios(monedaA.Id, Desde, Hasta);
            var cambiosB = await repositorio.ObtenerHistorialCambios(monedaB.Id, Desde, Hasta);

            var fechas = cambiosA.Select(c => c.Fecha)
                                 .Intersect(cambiosB.Select(c => c.Fecha))
                                 .OrderBy(f => f)
                                 .ToList();

            if (!fechas.Any())
                return Enumerable.Empty<AnalisisInversionDTO>();

            var resultado = new List<AnalisisInversionDTO>();

            string? recomendacionActual = null;
            DateTime? fechaInicioActual = null;
            double cambioAInicio = 0, cambioBInicio = 0, cambioAFin = 0, cambioBFin = 0;

            foreach (var fecha in fechas)
            {
                var cambioA = cambiosA.FirstOrDefault(c => c.Fecha == fecha)?.Cambio ?? 0;
                var cambioB = cambiosB.FirstOrDefault(c => c.Fecha == fecha)?.Cambio ?? 0;

                if (cambioA == 0 || cambioB == 0) continue;

                var ratio = cambioA / cambioB;

                string recomendacion = ratio switch
                {
                    < 1 => "Invertir en A",
                    > 1 => "Invertir en B",
                    _ => "Sin cambio significativo"
                };

                if (recomendacion != recomendacionActual)
                {
                    if (recomendacionActual != null)
                    {
                        resultado.Add(new AnalisisInversionDTO
                        {
                            SiglaMonedaA = SiglaMonedaA,
                            SiglaMonedaB = SiglaMonedaB,
                            FechaInicio = fechaInicioActual!.Value,
                            FechaFin = fecha.AddDays(-1),
                            Recomendacion = recomendacionActual,
                            CambioMonedaAInicio = cambioAInicio,
                            CambioMonedaBInicio = cambioBInicio,
                            CambioMonedaAFin = cambioAFin,
                            CambioMonedaBFin = cambioBFin
                        });
                    }

                    // Comenzar nuevo bloque
                    recomendacionActual = recomendacion;
                    fechaInicioActual = fecha;
                    cambioAInicio = cambioA;
                    cambioBInicio = cambioB;
                }

                cambioAFin = cambioA;
                cambioBFin = cambioB;
            }

            // Agregar último bloque
            if (recomendacionActual != null)
            {
                resultado.Add(new AnalisisInversionDTO
                {
                    SiglaMonedaA = SiglaMonedaA,
                    SiglaMonedaB = SiglaMonedaB,
                    FechaInicio = fechaInicioActual!.Value,
                    FechaFin = fechas.Last(),
                    Recomendacion = recomendacionActual,
                    CambioMonedaAInicio = cambioAInicio,
                    CambioMonedaBInicio = cambioBInicio,
                    CambioMonedaAFin = cambioAFin,
                    CambioMonedaBFin = cambioBFin
                });
            }

            return resultado;
        }

        public async Task<IEnumerable<AnalisisInversionDTO>> AnalizarInversionDolar(
            string siglaMoneda,
            DateTime desde,
            DateTime hasta,
            double umbralCambioPorcentaje = 1.0 // 1% por defecto
        )
        {
            var moneda = (await repositorio.Buscar(1, siglaMoneda)).FirstOrDefault();
            if (moneda == null)
                throw new Exception($"Moneda con sigla '{siglaMoneda}' no encontrada.");

            var cambios = (await repositorio.ObtenerHistorialCambios(moneda.Id, desde, hasta))
                          .OrderBy(c => c.Fecha)
                          .ToList();

            if (!cambios.Any())
                throw new Exception("No hay datos de cambio disponibles para ese período.");

            var resultado = new List<AnalisisInversionDTO>();
            string? tendenciaActual = null;
            DateTime? fechaInicio = null;
            double cambioInicio = 0;

            for (int i = 1; i < cambios.Count; i++)
            {
                var anterior = cambios[i - 1];
                var actual = cambios[i];

                double variacionPorcentaje = Math.Abs((actual.Cambio - anterior.Cambio) / anterior.Cambio * 100);

                string nuevaTendencia = variacionPorcentaje >= umbralCambioPorcentaje
                    ? (actual.Cambio > anterior.Cambio ? "Vender USD" : "Comprar USD")
                    : tendenciaActual ?? "Sin cambio";

                if (nuevaTendencia != tendenciaActual)
                {
                    if (tendenciaActual != null && fechaInicio.HasValue)
                    {
                        resultado.Add(new AnalisisInversionDTO
                        {
                            SiglaMonedaA = siglaMoneda,
                            SiglaMonedaB = "USD",
                            FechaInicio = fechaInicio.Value,
                            FechaFin = anterior.Fecha,
                            CambioMonedaAInicio = cambioInicio,
                            CambioMonedaAFin = anterior.Cambio,
                            Recomendacion = tendenciaActual
                        });
                    }

                    fechaInicio = anterior.Fecha;
                    cambioInicio = anterior.Cambio;
                    tendenciaActual = nuevaTendencia;
                }
            }

            var ultimo = cambios.Last();
            if (fechaInicio.HasValue)
            {
                resultado.Add(new AnalisisInversionDTO
                {
                    SiglaMonedaA = siglaMoneda,
                    SiglaMonedaB = "USD",
                    FechaInicio = fechaInicio.Value,
                    FechaFin = ultimo.Fecha,
                    CambioMonedaAInicio = cambioInicio,
                    CambioMonedaAFin = ultimo.Cambio,
                    Recomendacion = tendenciaActual!
                });
            }

            return resultado;
        }


    }
}
