using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AirControl.Subscribe;
using System.Reflection;

namespace AirControl
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // appsetting.jsonの読み込み設定
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("AirControl.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddTransient<MainPage>();
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
            return builder.Build();
        }
    }
}
