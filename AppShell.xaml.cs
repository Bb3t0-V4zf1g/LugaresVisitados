namespace LugaresVisitados
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("agregar", typeof(AgregarLugar));
            Routing.RegisterRoute("editar", typeof(EditarLugar));

            Routing.RegisterRoute("main", typeof(MainPage));

        }
    }
}
