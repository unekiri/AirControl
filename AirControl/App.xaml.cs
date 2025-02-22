using AirControl.Services.Subscribe;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Hosting;

namespace AirControl
{
    public partial class App : Application
    {
        private readonly AtFirstExeSubscribe _atFirstExeSubscribe;
        public App(AtFirstExeSubscribe atFirstExeSubscribe)
        {
            InitializeComponent();
            MainPage = new AppShell();
            _atFirstExeSubscribe = atFirstExeSubscribe;
        }

        protected override async void OnStart()
        {
            // 前回電源off時のエアコンの状態を表示する
            await this._atFirstExeSubscribe.Run();
        }
    }
}