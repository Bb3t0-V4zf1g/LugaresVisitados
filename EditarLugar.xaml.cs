using LugaresVisitados.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace LugaresVisitados
{
    [QueryProperty(nameof(Id), "id")]
    public partial class EditarLugar : ContentPage
    {
        private readonly HttpClient _httpClient;
        private Lugar lugarEditar;

        public string Id { get; set; }

        public EditarLugar()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://md1w2gfx-3000.usw3.devtunnels.ms/")
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(Id))
            {
                await CargarLugar(int.Parse(Id));
            }
        }

        private async Task CargarLugar(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"lugares/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    lugarEditar = JsonSerializer.Deserialize<Lugar>(json, options);

                    if (lugarEditar != null)
                    {
                        nombreEntry.Text = lugarEditar.Nombre;
                        descripcionEntry.Text = lugarEditar.Descripcion;
                        fechaVisitaPicker.Date = lugarEditar.FechaVisita;
                        imagenUrlEntry.Text = lugarEditar.ImagenUrl;
                        previewImagen.Source = lugarEditar.ImagenUrl;
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar el lugar", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void Guardar_Clicked(object sender, EventArgs e)
        {
            if (lugarEditar == null) return;

            lugarEditar.Nombre = nombreEntry.Text;
            lugarEditar.Descripcion = descripcionEntry.Text;
            lugarEditar.FechaVisita = fechaVisitaPicker.Date;
            lugarEditar.ImagenUrl = imagenUrlEntry.Text;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("editar", lugarEditar);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Lugar actualizado correctamente", "OK");
                    await Shell.Current.GoToAsync(".."); // volver a la página anterior
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
