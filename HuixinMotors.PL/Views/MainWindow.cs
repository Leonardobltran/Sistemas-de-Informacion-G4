using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.PL.Helpers;
using HuixinMotors.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HuixinMotors.PL.Views;

public class MainWindow : Window
{
    private readonly IClienteService _clienteService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IProveedorService _proveedorService;
    private readonly ISucursalService _sucursalService;
    private readonly ICategoriaService _categoriaService;
    private readonly IVehiculoService _vehiculoService;
    private readonly IVentaService _ventaService;
    private readonly IDashboardService _dashboardService;
    private readonly Empleado _usuarioActual;

    private bool EsAdministrador =>
        (_usuarioActual?.Usuario?.Equals("admin", StringComparison.OrdinalIgnoreCase) == true) ||
        ((_usuarioActual?.Cargo ?? "").IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0);

    private string RolActual => EsAdministrador ? "Administrador" : "Cliente";

    public MainWindow(
        Empleado usuarioActual,
        IClienteService clienteService,
        IEmpleadoService empleadoService,
        IProveedorService proveedorService,
        ISucursalService sucursalService,
        ICategoriaService categoriaService,
        IVehiculoService vehiculoService,
        IVentaService ventaService,
        IDashboardService dashboardService)
    {
        _usuarioActual = usuarioActual;
        _clienteService = clienteService;
        _empleadoService = empleadoService;
        _proveedorService = proveedorService;
        _sucursalService = sucursalService;
        _categoriaService = categoriaService;
        _vehiculoService = vehiculoService;
        _ventaService = ventaService;
        _dashboardService = dashboardService;

        Title = "Huixin Motors - Sistema de venta de autos";
        Width = 1000;
        Height = 640;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI();
    }

    private void BuildUI()
    {
        var root = new DockPanel();
        root.Background = new SolidColorBrush(ThemeHelper.Fondo);

        var sessionBar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 10,
            Margin = new Thickness(0, 10, 20, 0)
        };

        sessionBar.Children.Add(new TextBlock
        {
            Text = $"Usuario: {_usuarioActual?.Nombres} {_usuarioActual?.Apellidos}  |  Modo: {RolActual}",
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(ThemeHelper.TextDark),
            VerticalAlignment = VerticalAlignment.Center
        });

        var btnCerrarSesion = ThemeHelper.DangerButton("Cerrar sesión");
        btnCerrarSesion.Width = 130;
        btnCerrarSesion.Click += (s, e) => CerrarSesion();
        sessionBar.Children.Add(btnCerrarSesion);

        DockPanel.SetDock(sessionBar, Dock.Top);
        root.Children.Add(sessionBar);

        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 12,
            Margin = new Thickness(0, 18, 0, 5)
        };

        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo_huixin.png");
        if (File.Exists(logoPath))
        {
            header.Children.Add(new Image
            {
                Source = new Bitmap(logoPath),
                Width = 58,
                Height = 58,
                Stretch = Stretch.Uniform
            });
        }

        header.Children.Add(new TextBlock
        {
            Text = "HUIXIN MOTORS - Sistema de escritorio",
            FontSize = 25,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(ThemeHelper.Primary),
            VerticalAlignment = VerticalAlignment.Center
        });

        DockPanel.SetDock(header, Dock.Top);
        root.Children.Add(header);

        var subtitle = new TextBlock();
        subtitle.Text = "Gestion de clientes, vehiculos, proveedores, empleados, ventas, dashboard, mapa y reportes.";
        subtitle.Height = 45;
        subtitle.VerticalAlignment = VerticalAlignment.Center;
        subtitle.HorizontalAlignment = HorizontalAlignment.Center;
        subtitle.FontSize = 15;
        subtitle.Foreground = new SolidColorBrush(ThemeHelper.TextDark);
        DockPanel.SetDock(subtitle, Dock.Top);
        root.Children.Add(subtitle);

        var sv = new ScrollViewer();
        var panel = new WrapPanel();
        panel.Margin = new Thickness(60, 35, 60, 35);
        panel.Background = new SolidColorBrush(ThemeHelper.Fondo);
        sv.Content = panel;
        root.Children.Add(sv);

        if (EsAdministrador)
        {
            AddButton(panel, "Clientes", async (s, e) => await OpenClientes());
            AddButton(panel, "Vehiculos", async (s, e) => await OpenVehiculos(false));
            AddButton(panel, "Ventas", async (s, e) => await new VentaWindow(_ventaService, _vehiculoService, _clienteService, _empleadoService).ShowDialog(this));
            AddButton(panel, "Empleados", async (s, e) => await OpenEmpleados());
            AddButton(panel, "Proveedores", async (s, e) => await OpenProveedores());
            AddButton(panel, "Sucursales", async (s, e) => await OpenSucursales());
            AddButton(panel, "Categorias", async (s, e) => await OpenCategorias());
            AddButton(panel, "Dashboard", async (s, e) => await new DashboardWindow(_dashboardService).ShowDialog(this));
            AddButton(panel, "Mapa", async (s, e) => await new MapaWindow(_ventaService).ShowDialog(this));
            AddButton(panel, "Reportes", async (s, e) => await new ReportesWindow(_ventaService).ShowDialog(this));
        }
        else
        {
            AddButton(panel, "Ver vehículos", async (s, e) => await OpenVehiculos(true));
            AddButton(panel, "Mapa", async (s, e) => await new MapaWindow(_ventaService).ShowDialog(this));
        }

        Content = root;
    }

    private void CerrarSesion()
    {
        var loginWin = App.Services.GetRequiredService<LoginWindow>();
        loginWin.Show();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = loginWin;
        }

        Close();
    }

    private void AddButton(WrapPanel panel, string text, EventHandler<RoutedEventArgs> click)
    {
        var b = new Border();
        b.Width = 240;
        b.Height = 86;
        b.Margin = new Thickness(14);
        b.Background = new SolidColorBrush(ThemeHelper.Primary);
        b.CornerRadius = new CornerRadius(16);
        b.Cursor = new Cursor(StandardCursorType.Hand);

        var tb = new TextBlock();
        var iconos = new Dictionary<string, string>
        {
            ["Clientes"] = "👥",
            ["Vehiculos"] = "🚗",
            ["Ventas"] = "🧾",
            ["Empleados"] = "🪪",
            ["Proveedores"] = "🚚",
            ["Sucursales"] = "🏢",
            ["Categorias"] = "🏷",
            ["Dashboard"] = "📊",
            ["Mapa"] = "🗺",
            ["Reportes"] = "📈",
            ["Ver vehículos"] = "🚗",
        };
        tb.Text = (iconos.TryGetValue(text, out var icono) ? icono + "  " : "") + text;
        tb.Foreground = Brushes.White;
        tb.FontSize = 17;
        tb.FontWeight = FontWeight.Bold;
        tb.VerticalAlignment = VerticalAlignment.Center;
        tb.HorizontalAlignment = HorizontalAlignment.Center;
        b.Child = tb;

        b.PointerEntered += (s, e) => b.Background = new SolidColorBrush(ThemeHelper.PrimaryDark);
        b.PointerExited += (s, e) => b.Background = new SolidColorBrush(ThemeHelper.Primary);
        b.PointerPressed += (s, e) => click(s, e);
        panel.Children.Add(b);
    }

    private async Task OpenClientes()
    {
        var window = new GenericCrudWindow(
            "CRUD Clientes", "IdCliente", ClienteFields(),
            loadAll: async () => await _clienteService.GetAllAsync(),
            save: async (id, values) =>
            {
                var c = MapToCliente(values);
                if (id != null) c.IdCliente = (int)id;
                if (id == null) await _clienteService.InsertAsync(c);
                else await _clienteService.UpdateAsync(c);
            },
            delete: async (id) => await _clienteService.DeleteAsync((int)id));
        await window.ShowDialog(this);
    }

    private async Task OpenEmpleados()
    {
        var window = new GenericCrudWindow(
            "CRUD Empleados", "IdEmpleado", EmpleadoFields(),
            loadAll: async () => await _empleadoService.GetAllAsync(),
            save: async (id, values) =>
            {
                var e = MapToEmpleado(values);
                if (id != null) e.IdEmpleado = (int)id;
                if (id == null) await _empleadoService.InsertAsync(e);
                else await _empleadoService.UpdateAsync(e);
            },
            delete: async (id) => await _empleadoService.DeleteAsync((int)id));
        await window.ShowDialog(this);
    }

    private async Task OpenProveedores()
    {
        var window = new GenericCrudWindow(
            "CRUD Proveedores", "IdProveedor", ProveedorFields(),
            loadAll: async () => await _proveedorService.GetAllAsync(),
            save: async (id, values) =>
            {
                var p = MapToProveedor(values);
                if (id != null) p.IdProveedor = (int)id;
                if (id == null) await _proveedorService.InsertAsync(p);
                else await _proveedorService.UpdateAsync(p);
            },
            delete: async (id) => await _proveedorService.DeleteAsync((int)id));
        await window.ShowDialog(this);
    }

    private async Task OpenSucursales()
    {
        var window = new GenericCrudWindow(
            "CRUD Sucursales", "IdSucursal", SucursalFields(),
            loadAll: async () => await _sucursalService.GetAllAsync(),
            save: async (id, values) =>
            {
                var s = MapToSucursal(values);
                if (id != null) s.IdSucursal = (int)id;
                if (id == null) await _sucursalService.InsertAsync(s);
                else await _sucursalService.UpdateAsync(s);
            },
            delete: async (id) => await _sucursalService.DeleteAsync((int)id));
        await window.ShowDialog(this);
    }

    private async Task OpenCategorias()
    {
        var window = new GenericCrudWindow(
            "CRUD Categorias", "IdCategoria", CategoriaFields(),
            loadAll: async () => await _categoriaService.GetAllAsync(),
            save: async (id, values) =>
            {
                var c = MapToCategoria(values);
                if (id != null) c.IdCategoria = (int)id;
                if (id == null) await _categoriaService.InsertAsync(c);
                else await _categoriaService.UpdateAsync(c);
            },
            delete: async (id) => await _categoriaService.DeleteAsync((int)id));
        await window.ShowDialog(this);
    }

    private async Task OpenVehiculos(bool soloLectura = false)
    {
        var provCombo = await _proveedorService.GetComboAsync();
        var catCombo = await _categoriaService.GetComboAsync();
        var sucCombo = await _sucursalService.GetComboAsync();
        var estados = new List<string> { "Disponible", "Vendido", "Mantenimiento", "Reservado" };

        var fields = new List<FieldDef>
        {
            new("IdProveedor", "Proveedor", true, FieldType.Combo, comboItems: provCombo),
            new("IdCategoria", "Categoría", true, FieldType.Combo, comboItems: catCombo),
            new("IdSucursal", "Sucursal", true, FieldType.Combo, comboItems: sucCombo),
            new("Marca", "Marca", true),
            new("Modelo", "Modelo", true),
            new("Anio", "Año", true),
            new("Color", "Color", false),
            new("Vin", "VIN", true),
            new("Placa", "Placa", false),
            new("PrecioCompra", "P. Compra", true),
            new("PrecioVenta", "P. Venta", true),
            new("Estado", "Estado", true, FieldType.StaticCombo, staticItems: estados),
            new("Kilometraje", "Kilometraje", true),
            new("TipoCombustible", "Combustible", false),
            new("Transmision", "Transmisión", false),
            new("ImagenUrl", "Foto", false)
        };

        var window = new GenericCrudWindow(
            "CRUD Vehiculos", "IdVehiculo", fields,
            loadAll: async () => await _vehiculoService.GetAllAsync(),
            save: async (id, values) =>
            {
                var v = MapToVehiculo(values);
                if (id != null) v.IdVehiculo = (int)id;
                if (id == null) await _vehiculoService.InsertAsync(v);
                else await _vehiculoService.UpdateAsync(v);
            },
            delete: async (id) => await _vehiculoService.DeleteAsync((int)id),
            soloLectura: soloLectura);
        await window.ShowDialog(this);
    }

    private static Cliente MapToCliente(Dictionary<string, string> v) => new()
    {
        Nombres = Get(v, "Nombres"),
        Apellidos = Get(v, "Apellidos"),
        Ci = Get(v, "Ci"),
        Telefono = Get(v, "Telefono"),
        Email = Get(v, "Email"),
        Direccion = Get(v, "Direccion"),
        Estado = GetBool(v, "Estado"),
    };

    private static Empleado MapToEmpleado(Dictionary<string, string> v) => new()
    {
        Nombres = Get(v, "Nombres"),
        Apellidos = Get(v, "Apellidos"),
        Cargo = Get(v, "Cargo"),
        Telefono = Get(v, "Telefono"),
        Email = Get(v, "Email"),
        Usuario = Get(v, "Usuario"),
        Clave = Get(v, "Clave"),
        Estado = GetBool(v, "Estado"),
    };

    private static Proveedor MapToProveedor(Dictionary<string, string> v) => new()
    {
        Nombre = Get(v, "Nombre"),
        Nit = Get(v, "Nit"),
        Telefono = Get(v, "Telefono"),
        Email = Get(v, "Email"),
        Direccion = Get(v, "Direccion"),
        Pais = Get(v, "Pais"),
        Estado = GetBool(v, "Estado"),
    };

    private static Sucursal MapToSucursal(Dictionary<string, string> v) => new()
    {
        Nombre = Get(v, "Nombre"),
        Ciudad = Get(v, "Ciudad"),
        Direccion = Get(v, "Direccion"),
        Latitud = GetDecimal(v, "Latitud"),
        Longitud = GetDecimal(v, "Longitud"),
        Estado = GetBool(v, "Estado"),
    };

    private static CategoriaVehiculo MapToCategoria(Dictionary<string, string> v) => new()
    {
        Nombre = Get(v, "Nombre"),
        Descripcion = Get(v, "Descripcion"),
        Estado = GetBool(v, "Estado"),
    };

    private static Vehiculo MapToVehiculo(Dictionary<string, string> v) => new()
    {
        IdProveedor = GetInt(v, "IdProveedor"),
        IdCategoria = GetInt(v, "IdCategoria"),
        IdSucursal = GetInt(v, "IdSucursal"),
        Marca = Get(v, "Marca"),
        Modelo = Get(v, "Modelo"),
        Anio = GetInt(v, "Anio"),
        Color = Get(v, "Color"),
        Vin = Get(v, "Vin"),
        Placa = Get(v, "Placa"),
        PrecioCompra = GetDecimal(v, "PrecioCompra"),
        PrecioVenta = GetDecimal(v, "PrecioVenta"),
        Estado = Get(v, "Estado"),
        Kilometraje = GetInt(v, "Kilometraje"),
        TipoCombustible = Get(v, "TipoCombustible"),
        Transmision = Get(v, "Transmision"),
        ImagenUrl = Get(v, "ImagenUrl"),
    };

    private static string Get(Dictionary<string, string> v, string key) =>
        v.TryGetValue(key, out var val) ? val ?? "" : "";

    private static int GetInt(Dictionary<string, string> v, string key) =>
        v.TryGetValue(key, out var val) && int.TryParse(val, out var i) ? i : 0;

    private static decimal GetDecimal(Dictionary<string, string> v, string key) =>
        v.TryGetValue(key, out var val) && decimal.TryParse(val?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;

    private static bool GetBool(Dictionary<string, string> v, string key) =>
        v.TryGetValue(key, out var val) && (val is "1" or "true" or "True" or "activo" or "si" or "Si");

    private static List<FieldDef> ClienteFields() => new()
    {
        new("Nombres", "Nombres", true), new("Apellidos", "Apellidos", true),
        new("Ci", "CI/NIT", true), new("Telefono", "Telefono", false),
        new("Email", "Email", false), new("Direccion", "Direccion", false),
        new("Estado", "Estado", true, FieldType.Boolean),
    };

    private static List<FieldDef> EmpleadoFields() => new()
    {
        new("Nombres", "Nombres", true), new("Apellidos", "Apellidos", true),
        new("Cargo", "Cargo", true), new("Telefono", "Telefono", false),
        new("Email", "Email", false), new("Usuario", "Usuario", true),
        new("Clave", "Clave", true), new("Estado", "Estado", true, FieldType.Boolean),
    };

    private static List<FieldDef> ProveedorFields() => new()
    {
        new("Nombre", "Nombre", true), new("Nit", "NIT", false),
        new("Telefono", "Telefono", false), new("Email", "Email", false),
        new("Direccion", "Direccion", false), new("Pais", "Pais", true),
        new("Estado", "Estado", true, FieldType.Boolean),
    };

    private static List<FieldDef> SucursalFields() => new()
    {
        new("Nombre", "Nombre", true), new("Ciudad", "Ciudad", true),
        new("Direccion", "Direccion", true), new("Latitud", "Latitud", false),
        new("Longitud", "Longitud", false), new("Estado", "Estado", true, FieldType.Boolean),
    };

    private static List<FieldDef> CategoriaFields() => new()
    {
        new("Nombre", "Nombre", true), new("Descripcion", "Descripcion", false),
        new("Estado", "Estado", true, FieldType.Boolean),
    };
}
