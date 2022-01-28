using Microsoft.Maui.LifecycleEvents;

namespace WeatherTwentyOne;

using (SentrySdk.Init(o => 
{
    // Tells which project in Sentry to send events to:
    o.Dsn = "https://f05bf621e53c48fc83bad61fd76eb1b9@o1129392.ingest.sentry.io/6173501";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
    // We recommend adjusting this value in production.
    o.TracesSampleRate = 1.0; 
}))
{
    // App code goes here - Disposing will flush events out
}

using Sentry;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
            });
        builder.ConfigureLifecycleEvents(lifecycle => {
#if WINDOWS
        lifecycle
            .AddWindows(windows =>
                windows.OnNativeMessage((app, args) => {
                    if (WindowExtensions.Hwnd == IntPtr.Zero)
                    {
                        WindowExtensions.Hwnd = args.Hwnd;
                        WindowExtensions.SetIcon("Platforms/Windows/trayicon.ico");
                    }
                    app.ExtendsContentIntoTitleBar = false;
                }));
#endif
        });

        var services = builder.Services;
#if WINDOWS
            services.AddSingleton<ITrayService, WinUI.TrayService>();
            services.AddSingleton<INotificationService, WinUI.NotificationService>();
#elif MACCATALYST
            services.AddSingleton<ITrayService, MacCatalyst.TrayService>();
            services.AddSingleton<INotificationService, MacCatalyst.NotificationService>();
#endif



        try
        {
            throw null;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        return builder.Build();
    }
}