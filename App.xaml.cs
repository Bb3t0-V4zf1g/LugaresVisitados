namespace LugaresVisitados
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell(); // Esto activa Shell
        }

    }
}