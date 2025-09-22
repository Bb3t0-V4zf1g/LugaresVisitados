using LugaresVisitados.Models;
using System.Net.Http.Json;

namespace LugaresVisitados
{
    public partial class EditarLugar : ContentPage, IQueryAttributable
    {
        private readonly HttpClient _httpClient;

        public EditarLugar()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://5qc0m05r-3000.usw3.devtunnels.ms/")
            };

            // Suscribirse al evento de cambio de texto en la URL de imagen
            imagenUrlEntry.TextChanged += OnImagenUrlChanged;
        }

        public int lugarId;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("id"))
            {
                lugarId = Convert.ToInt32(query["id"]);
            }

            if (query.ContainsKey("nombre"))
            {
                nombreEntry.Text = query["nombre"]?.ToString();
            }

            if (query.ContainsKey("descripcion"))
            {
                descripcionEntry.Text = query["descripcion"]?.ToString();
            }

            if (query.ContainsKey("fechaVisita"))
            {
                if (DateTime.TryParse(query["fechaVisita"]?.ToString(), out DateTime fecha))
                {
                    fechaVisitaPicker.Date = fecha;
                }
            }

            if (query.ContainsKey("imagenUrl"))
            {
                imagenUrlEntry.Text = query["imagenUrl"]?.ToString();

                // Cargar imagen de vista previa si hay una URL
                if (!string.IsNullOrWhiteSpace(imagenUrlEntry.Text) &&
                    (imagenUrlEntry.Text.StartsWith("http://") || imagenUrlEntry.Text.StartsWith("https://")))
                {
                    previewImagen.Source = ImageSource.FromUri(new Uri(imagenUrlEntry.Text));
                }
            }
        }

        private void OnImagenUrlChanged(object sender, TextChangedEventArgs e)
        {
            // Mostrar vista previa si hay una URL válida
            if (!string.IsNullOrWhiteSpace(imagenUrlEntry.Text) &&
                (imagenUrlEntry.Text.StartsWith("http://") || imagenUrlEntry.Text.StartsWith("https://")))
            {
                previewImagen.Source = ImageSource.FromUri(new Uri(imagenUrlEntry.Text));
            }
        }

        private async void Guardar_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreEntry.Text) ||
                    string.IsNullOrWhiteSpace(descripcionEntry.Text) ||
                    string.IsNullOrWhiteSpace(imagenUrlEntry.Text))
                {
                    await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                    return;
                }

                if (fechaVisitaPicker.Date > DateTime.Today)
                {
                    await DisplayAlert("Error", "La fecha de visita no puede ser posterior a hoy.", "OK");
                    return;
                }

                var response = await _httpClient.GetAsync($"editar?id={lugarId}" +
                    $"&nombre={Uri.EscapeDataString(nombreEntry.Text)}" +
                    $"&descripcion={Uri.EscapeDataString(descripcionEntry.Text)}" +
                    $"&fecha_visita={fechaVisitaPicker.Date:yyyy-MM-dd}" +
                    $"&URL_lugar={Uri.EscapeDataString(imagenUrlEntry.Text)}");

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Lugar actualizado correctamente", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar el lugar", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
            }
        }

        private async void Cancelar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}