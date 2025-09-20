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
        public List<Lugar> Lugares { get; set; }
        private List<Lugar> LugaresOriginal { get; set; }

        public MainPage()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://dx2hgfq2-3000.usw3.devtunnels.ms/")
            };

            CargarLugares();
        }

        private async Task CargarLugares()
        {
            try
            {
                // Ruta del archivo JSON (asegúrate de que está en Resources/Raw/lugares.json)
                using var stream = await FileSystem.OpenAppPackageFileAsync("lugares.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                // Deserializamos
                var lugares = JsonSerializer.Deserialize<List<Lugar>>(json);

                // Mostramos en el ListView
                lugaresCollectionView.ItemsSource = lugares;
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

            var response = await _httpClient.DeleteAsync($"lugares/{lugar.Id}");
            if (response.IsSuccessStatusCode)
            {
                Lugares.Remove(lugar);
                lugaresCollectionView.ItemsSource = null;
                lugaresCollectionView.ItemsSource = Lugares;
                LugaresOriginal = new List<Lugar>(Lugares);
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el lugar", "OK");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = searchBar.Text?.ToLower() ?? "";
            Lugares = LugaresOriginal
                        .Where(l => l.Nombre.ToLower().Contains(filtro))
                        .ToList();
            lugaresCollectionView.ItemsSource = null;
            lugaresCollectionView.ItemsSource = Lugares;
        }
    }
}
