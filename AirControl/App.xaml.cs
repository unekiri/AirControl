namespace AirControl
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Diコンテナを使ってMainPageを生成
            MainPage = new AppShell();
        }
    }
}
