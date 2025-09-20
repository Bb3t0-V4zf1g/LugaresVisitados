using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using LugaresVisitados.Models;

namespace LugaresVisitados
{
    public partial class AgregarLugar : ContentPage
    {
        HttpClient cliente = new HttpClient { BaseAddress = new Uri("https://1dxpc21h-3000.usw3.devtunnels.ms/") };

        public AgregarLugar()
        {
            InitializeComponent();
            fechaVisita.Date = DateTime.Now; // Establecer fecha actual por defecto
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            string nombreTxt = nombre.Text;
            string descripcionTxt = descripcion.Text;
            DateTime fechaVisitaDate = fechaVisita.Date;
            string imagenUrlTxt = imagenUrl.Text;

            if (string.IsNullOrWhiteSpace(nombreTxt) ||
                string.IsNullOrWhiteSpace(descripcionTxt) ||
                string.IsNullOrWhiteSpace(imagenUrlTxt))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            // Validación: no permitir fechas futuras
            if (fechaVisitaDate > DateTime.Today)
            {
                await DisplayAlert("Error", "La fecha de visita no puede ser posterior a hoy.", "OK");
                return;
            }

            try
            {
                // Construir la URL con parámetros GET (como en tu API Node.js)
                string url = $"agregar?nombre={Uri.EscapeDataString(nombreTxt)}" +
                            $"&descripcion={Uri.EscapeDataString(descripcionTxt)}" +
                            $"&fecha_visita={fechaVisitaDate:yyyy-MM-dd}" +
                            $"&URL_lugar={Uri.EscapeDataString(imagenUrlTxt)}";

                var response = await cliente.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Lugar agregado correctamente", "OK");

                    // Volver a la página anterior
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"No se pudo agregar el lugar. Error: {errorContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }
    }
}