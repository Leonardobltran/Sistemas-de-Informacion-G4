using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.BRL.Services;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.DAL.Repositories;
using HuixinMotors.PL.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HuixinMotors.PL;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<LoginWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
        {
            path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        }
        string json = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(json);
        string connStr = doc.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("HuixinMotors")
            .GetString()!;

        services.AddSingleton(new DbConnectionFactory(connStr));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<ISucursalRepository, SucursalRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IVehiculoRepository, VehiculoRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IProveedorService, ProveedorService>();
        services.AddScoped<ISucursalService, SucursalService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IVehiculoService, VehiculoService>();
        services.AddScoped<IVentaService, VentaService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<VentaWindow>();
        services.AddTransient<DashboardWindow>();
        services.AddTransient<ReportesWindow>();
        services.AddTransient<MapaWindow>();
    }
}
