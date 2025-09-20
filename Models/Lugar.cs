using System;
using System.Text.Json.Serialization;

namespace LugaresVisitados.Models
{
    public class Lugar
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("fecha_visita")]
        public DateTime FechaVisita { get; set; }

        [JsonPropertyName("URL_lugar")]
        public string ImagenUrl { get; set; } = string.Empty;

        // Propiedades calculadas o adicionales si las necesitas
        public string FechaVisitaFormateada => FechaVisita.ToString("dd/MM/yyyy");
    }
}
