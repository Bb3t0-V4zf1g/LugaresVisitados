using LugaresVisitados.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace LugaresVisitados
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        public List<Lugar> Lugares { get; set; } = new List<Lugar>();
        private List<Lugar> LugaresOriginal { get; set; } = new List<Lugar>();

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://1dxpc21h-3000.usw3.devtunnels.ms/")
            };
            CargarLugares();
        }

        private async Task CargarLugares()
        {
            try
            {
                // Cargar desde la API en lugar del archivo JSON
                var response = await _httpClient.GetAsync("lugares");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // Configurar JsonSerializer para manejar propiedades en minúsculas
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var lugares = JsonSerializer.Deserialize<List<Lugar>>(json, options) ?? new List<Lugar>();

                    Lugares = lugares;
                    LugaresOriginal = new List<Lugar>(lugares); // Crear copia para búsquedas

                    // Actualizar la vista
                    lugaresCollectionView.ItemsSource = Lugares;
                }
                else
                {
                    await DisplayAlert("Error", $"Error del servidor: {response.StatusCode}", "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Error de conexión", $"No se pudo conectar al servidor: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los lugares: {ex.Message}", "OK");
            }
        }

        private async void AgregarLugar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("agregar");
        }

        private async void EditarLugar_Clicked(object sender, EventArgs e)
        {
            var boton = (Button)sender;
            var lugar = (Lugar)boton.BindingContext;
            await Shell.Current.GoToAsync($"editar?id={lugar.Id}");
        }

        private async void EliminarLugar_Clicked(object sender, EventArgs e)
        {
            var boton = (Button)sender;
            var lugar = (Lugar)boton.BindingContext;

            bool confirm = await DisplayAlert("Confirmar", $"¿Eliminar {lugar.Nombre}?", "Sí", "No");
            if (!confirm) return;

            try
            {
                // Usar la ruta GET de eliminación que tienes en tu servidor
                var response = await _httpClient.GetAsync($"eliminar?id={lugar.Id}");

                if (response.IsSuccessStatusCode)
                {
                    // Recargar los datos después de eliminar
                    await CargarLugares();
                    await DisplayAlert("Éxito", "Lugar eliminado correctamente", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar el lugar", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = searchBar.Text?.ToLower() ?? "";

            if (string.IsNullOrEmpty(filtro))
            {
                // Si no hay filtro, mostrar todos los lugares
                lugaresCollectionView.ItemsSource = LugaresOriginal;
            }
            else
            {
                // Filtrar los lugares
                var lugaresFiltrados = LugaresOriginal
                    .Where(l => l.Nombre?.ToLower().Contains(filtro) == true)
                    .ToList();

                lugaresCollectionView.ItemsSource = lugaresFiltrados;
            }
        }

        // Método para refrescar los datos (útil cuando vuelves de agregar/editar)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarLugares();
        }
    }
}