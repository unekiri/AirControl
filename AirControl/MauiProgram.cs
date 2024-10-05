using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AirControl.Subscribe;

namespace AirControl
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // appsetting.jsonを読み込む設定を追加
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<SubscribedMessage>();
            builder.Services.AddSingleton<Subscriber>();
            builder.Services.AddSingleton<ExeSubscriber>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            var mauiApp =  builder.Build();

            // 現在の状態を表示させる
            var exeSubscriber = mauiApp.Services.GetService<ExeSubscriber>();
            await exeSubscriber.Run();

            return mauiApp;
        }
    }
}
