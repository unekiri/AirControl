using AirControl.Subscribe;
using Microsoft.Maui.Hosting;

namespace AirControl
{
    public partial class App : Application
    {
        private readonly ExeSubscriber _exeSubscriber;
        public App(ExeSubscriber exeSubscriber)
        {
            InitializeComponent();
            MainPage = new AppShell();
            this._exeSubscriber = exeSubscriber;
        }

        protected override async void OnStart()
        {
            // 現在の状態を表示させる
            await this._exeSubscriber?.Run();
        }
    }
}
