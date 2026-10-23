using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using HuixinMotors.PL.Helpers;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.PL.Views;

public class VehicleDetailWindow : Window
{
    private readonly Vehiculo _vehiculo;
    private readonly string _categoria;
    private readonly string _proveedor;
    private readonly string _sucursal;

    public VehicleDetailWindow(Vehiculo v, string categoria, string proveedor, string sucursal)
    {
        _vehiculo = v;
        _categoria = categoria;
        _proveedor = proveedor;
        _sucursal = sucursal;

        Title = $"Detalles del Auto: {v.Marca} {v.Modelo}";
        Width = 850;
        Height = 550;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = new SolidColorBrush(ThemeHelper.Fondo);

        BuildUI();
    }

    private void BuildUI()
    {
        var root = new DockPanel();

        // Title bar
        var titleBar = ThemeHelper.TitleBar($"{_vehiculo.Marca} {_vehiculo.Modelo} ({_vehiculo.Anio})", this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        // Footer button
        var footer = new Border
        {
            Padding = new Thickness(15),
            Background = new SolidColorBrush(Color.FromArgb(255, 230, 235, 240)),
            Child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Children = { ThemeHelper.Button("Cerrar") }
            }
        };
        ((Button)((StackPanel)footer.Child).Children[0]).Click += (s, e) => this.Close();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);

        // Main content split
        var content = new Grid();
        content.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(400, GridUnitType.Pixel)));
        content.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        content.Margin = new Thickness(20);

        // Left Column: Photo
        var imgBorder = new Border
        {
            Margin = new Thickness(10),
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 210, 215, 220)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12),
            ClipToBounds = true
        };

        var imgGrid = new Grid();
        var imgPlaceholder = new TextBlock
        {
            Text = "Cargando imagen...",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14,
            Foreground = Brushes.Gray
        };
        imgGrid.Children.Add(imgPlaceholder);

        var imgControl = new Image
        {
            Stretch = Stretch.UniformToFill,
            IsVisible = false
        };
        imgGrid.Children.Add(imgControl);
        imgBorder.Child = imgGrid;
        Grid.SetColumn(imgBorder, 0);
        content.Children.Add(imgBorder);

        // Right Column: Details
        var detailsBorder = new Border
        {
            Margin = new Thickness(10),
            Padding = new Thickness(20),
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 210, 215, 220)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12)
        };

        var detailsScroll = new ScrollViewer();
        var detailsPanel = new StackPanel { Spacing = 15 };

        // Price Tag
        var pricePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
        pricePanel.Children.Add(new TextBlock
        {
            Text = "PRECIO DE VENTA:",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Gray,
            VerticalAlignment = VerticalAlignment.Bottom
        });
        pricePanel.Children.Add(new TextBlock
        {
            Text = $"${_vehiculo.PrecioVenta:N2}",
            FontSize = 26,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(ThemeHelper.Primary),
            VerticalAlignment = VerticalAlignment.Bottom
        });
        detailsPanel.Children.Add(pricePanel);

        // Separator
        detailsPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230)) });

        // Specs Grid
        var specs = new Grid();
        specs.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        specs.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Marca
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Modelo
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Año
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Kilometraje
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Combustible
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Transmisión
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Color
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Placa
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // VIN
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Categoría
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Proveedor
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Sucursal
        specs.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Estado

        int row = 0;
        AddSpecRow(specs, ref row, "Marca:", _vehiculo.Marca);
        AddSpecRow(specs, ref row, "Modelo:", _vehiculo.Modelo);
        AddSpecRow(specs, ref row, "Año:", _vehiculo.Anio.ToString());
        AddSpecRow(specs, ref row, "Kilometraje:", $"{_vehiculo.Kilometraje:N0} km");
        AddSpecRow(specs, ref row, "Combustible:", _vehiculo.TipoCombustible);
        AddSpecRow(specs, ref row, "Transmisión:", _vehiculo.Transmision);
        AddSpecRow(specs, ref row, "Color:", _vehiculo.Color);
        AddSpecRow(specs, ref row, "Placa:", _vehiculo.Placa);
        AddSpecRow(specs, ref row, "VIN / Chasis:", _vehiculo.Vin);
        AddSpecRow(specs, ref row, "Categoría:", _categoria);
        AddSpecRow(specs, ref row, "Proveedor:", _proveedor);
        AddSpecRow(specs, ref row, "Sucursal:", _sucursal);
        AddSpecRow(specs, ref row, "Estado:", _vehiculo.Estado, true);

        detailsPanel.Children.Add(specs);
        detailsScroll.Content = detailsPanel;
        detailsBorder.Child = detailsScroll;
        Grid.SetColumn(detailsBorder, 1);
        content.Children.Add(detailsBorder);

        root.Children.Add(content);
        Content = root;

        // Async Image Loading
        _ = LoadImageAsync(_vehiculo.ImagenUrl, imgControl, imgPlaceholder);
    }

    private void AddSpecRow(Grid grid, ref int row, string label, string value, bool highlightValue = false)
    {
        var lbl = new TextBlock
        {
            Text = label,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 4, 15, 4),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(lbl, 0);
        Grid.SetRow(lbl, row);
        grid.Children.Add(lbl);

        var val = new TextBlock
        {
            Text = string.IsNullOrWhiteSpace(value) ? "-" : value,
            FontWeight = highlightValue ? FontWeight.Bold : FontWeight.Normal,
            Foreground = highlightValue ? new SolidColorBrush(ThemeHelper.Primary) : new SolidColorBrush(ThemeHelper.TextDark),
            Margin = new Thickness(0, 4, 0, 4),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(val, 1);
        Grid.SetRow(val, row);
        grid.Children.Add(val);

        row++;
    }

    private async Task LoadImageAsync(string pathOrUrl, Image imgControl, TextBlock txtPlaceholder)
    {
        if (string.IsNullOrWhiteSpace(pathOrUrl))
        {
            txtPlaceholder.Text = "Sin Foto";
            return;
        }

        try
        {
            Bitmap bmp = null;
            if (pathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                pathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("HuixinMotors/1.0");
                var bytes = await client.GetByteArrayAsync(pathOrUrl);
                using var ms = new MemoryStream(bytes);
                bmp = new Bitmap(ms);
            }
            else if (File.Exists(pathOrUrl))
            {
                bmp = new Bitmap(pathOrUrl);
            }

            if (bmp != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    imgControl.Source = bmp;
                    imgControl.IsVisible = true;
                    txtPlaceholder.IsVisible = false;
                });
            }
            else
            {
                Dispatcher.UIThread.Post(() => txtPlaceholder.Text = "No se pudo cargar la imagen");
            }
        }
        catch
        {
            Dispatcher.UIThread.Post(() => txtPlaceholder.Text = "Error al cargar la imagen");
        }
    }
}
