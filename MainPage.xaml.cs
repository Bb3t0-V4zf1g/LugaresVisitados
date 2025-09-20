using LugaresVisitados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace LugaresVisitados
{
    public partial class MainPage : ContentPage
    {
        public List<Lugar> Lugares { get; set; }
        private List<Lugar> LugaresOriginal { get; set; }

        public MainPage()
        {
            InitializeComponent();

            // Datos de prueba inyectados
            Lugares = new List<Lugar>
            {
                new Lugar { Id=1, Nombre="Tokyo", Descripcion="Recorrido por Shibuya y Asakusa", FechaVisita=DateTime.Now, ImagenUrl="https://www.gotokyo.org/es/plan/tokyo-outline/images/main_pxfree.webp" },
                new Lugar { Id=2, Nombre="Jalisco", Descripcion="Visita al centro de Guadalajara y Tequila", FechaVisita=DateTime.Now, ImagenUrl="https://imagenes.eleconomista.com.mx/files/image_768_768/uploads/2022/11/01/66e48e17c5cad.jpeg" },
                new Lugar { Id=3, Nombre="New Mexico", Descripcion="Exploración de los Badlands y naturaleza", FechaVisita=DateTime.Now, ImagenUrl="https://www.travelandleisure.com/thmb/aCoWceoIsbVI7oxWhKsprMbgZbE=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/1-74b48a357f69462c8a5725a82f28cd9c.jpg" }
            };

            LugaresOriginal = new List<Lugar>(Lugares);

            BindingContext = this;
        }

        private async void AgregarLugar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("agregar");
        }

        private async void EditarLugar_Clicked(object sender, EventArgs e)
        {
            var boton = (Button)sender;
            var lugar = (Lugar)boton.CommandParameter;
            // Pasar datos si quieres, ejemplo:
            // await Shell.Current.GoToAsync($"editar?id={lugar.Id}");
            await Shell.Current.GoToAsync("editar");
        }

        private void EliminarLugar_Clicked(object sender, EventArgs e)
        {
            var boton = (Button)sender;
            var lugar = (Lugar)boton.CommandParameter;

            Lugares.Remove(lugar);
            lugaresCollectionView.ItemsSource = null;
            lugaresCollectionView.ItemsSource = Lugares;
            LugaresOriginal = new List<Lugar>(Lugares);
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
