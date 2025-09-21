using LugaresVisitados.Models;
using System.Text.Json;

namespace LugaresVisitados
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        public List<Lugar> Lugares { get; set; } = new List<Lugar>();

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://md1w2gfx-3000.usw3.devtunnels.ms/")
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

                    // Limpiar caché y forzar actualización
                    lugaresCollectionView.ItemsSource = null;
                    
                    // Verificar si hay filtro activo en la barra de búsqueda
                    if (!string.IsNullOrEmpty(searchBar.Text))
                    {
                        // Si hay filtro, aplicarlo
                        SearchBar_TextChanged(searchBar, new TextChangedEventArgs(searchBar.Text, searchBar.Text));
                    }
                    else
                    {
                        // Si no hay filtro, mostrar todos los lugares
                        lugaresCollectionView.ItemsSource = Lugares;
                    }
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
           
            var parametros = new Dictionary<string, object>
            {
                ["id"] = lugar.Id.ToString(),
                ["nombre"] = lugar.Nombre ?? "",
                ["descripcion"] = lugar.Descripcion ?? "",
                ["fechaVisita"] = lugar.FechaVisita.ToString("yyyy-MM-dd"),
                ["imagenUrl"] = lugar.ImagenUrl ?? ""
            };

            await Shell.Current.GoToAsync("editar", parametros);
        }

        private async void EliminarLugar_Clicked(object sender, EventArgs e)
        {
            var boton = (Button)sender;
            var lugar = (Lugar)boton.BindingContext;

            bool confirm = await DisplayAlert("Confirmar", $"¿Eliminar {lugar.Nombre}?", "Sí", "No");
            if (!confirm) return;

            try
            {
                var response = await _httpClient.GetAsync($"eliminar?id={lugar.Id}");

                if (response.IsSuccessStatusCode)
                {
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
                // Si no hay filtro, mostrar todos los lugares actualizados
                lugaresCollectionView.ItemsSource = null;
                lugaresCollectionView.ItemsSource = Lugares; // Usar Lugares, no LugaresOriginal
            }
            else
            {
                // Filtrar los lugares actualizados
                var lugaresFiltrados = Lugares // Usar Lugares, no LugaresOriginal
                    .Where(l => l.Nombre?.ToLower().Contains(filtro) == true)
                    .ToList();

                lugaresCollectionView.ItemsSource = null;
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