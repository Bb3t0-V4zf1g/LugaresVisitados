using LugaresVisitados.Models;
using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace LugaresVisitados
{
    [QueryProperty(nameof(Id), "id")]
    public partial class EditarLugar : ContentPage
    {
        private readonly HttpClient _httpClient;
        private Lugar _lugar;

        public string Id { get; set; }

        public EditarLugar()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://1dxpc21h-3000.usw3.devtunnels.ms/")
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

                    _lugar = JsonSerializer.Deserialize<Lugar>(json, options);

                    if (_lugar != null)
                    {
                        nombreEntry.Text = _lugar.Nombre;
                        descripcionEditor.Text = _lugar.Descripcion;
                        fechaVisitaPicker.Date = _lugar.FechaVisita;
                        imagenUrlEntry.Text = _lugar.ImagenUrl;
                        previewImagen.Source = _lugar.ImagenUrl;
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
            if (_lugar == null) return;

            _lugar.Nombre = nombreEntry.Text;
            _lugar.Descripcion = descripcionEditor.Text;
            _lugar.FechaVisita = fechaVisitaPicker.Date;
            _lugar.ImagenUrl = imagenUrlEntry.Text;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("editar", _lugar);

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
