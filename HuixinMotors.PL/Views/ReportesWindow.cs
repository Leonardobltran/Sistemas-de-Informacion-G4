using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.PL.Helpers;

namespace HuixinMotors.PL.Views;

public class ReportesWindow : Window
{
    private readonly IVentaService _ventaService;
    private DatePicker desde, hasta;
    private ComboBox tipo;
    private DataGrid grid;
    private TextBox _filterBox;
    private IList _dataList;

    public ReportesWindow(IVentaService ventaService)
    {
        _ventaService = ventaService;

        Title = "Reportes Huixin Motors";
        Width = 1150;
        Height = 700;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI();
    }

    private void BuildUI()
    {
        var root = new DockPanel();
        root.Background = new SolidColorBrush(ThemeHelper.Fondo);

        var titleBar = ThemeHelper.TitleBar("Reportes de informacion", this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        var filterBorder = new Border();
        filterBorder.Background = Brushes.White;
        filterBorder.Padding = new Thickness(20, 12);
        filterBorder.Margin = new Thickness(10, 5, 10, 5);
        filterBorder.CornerRadius = new CornerRadius(10);

        var f = new WrapPanel();
        f.VerticalAlignment = VerticalAlignment.Center;

        tipo = new ComboBox { Width = 220, Height = 32, FontSize = 13 };
        tipo.Items.Add("Ventas por fechas");
        tipo.Items.Add("Inventario de vehiculos");
        tipo.Items.Add("Ventas por sucursal");
        tipo.SelectedIndex = 0;

        desde = new DatePicker { Width = 140 };
        desde.SelectedDate = DateTime.Today.AddMonths(-1);
        hasta = new DatePicker { Width = 140 };
        hasta.SelectedDate = DateTime.Today.AddDays(1);

        var generar = ThemeHelper.Button("Generar");
        generar.Click += (s, e) => Generar();

        var exportar = ThemeHelper.Button("Exportar CSV");
        exportar.Click += async (s, e) => await ExportarCsvAsync();

        f.Children.Add(new TextBlock { Text = "Tipo:", Width = 45, VerticalAlignment = VerticalAlignment.Center, FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = new SolidColorBrush(ThemeHelper.TextDark) });
        f.Children.Add(tipo);
        f.Children.Add(new TextBlock { Text = "Desde:", Width = 55, VerticalAlignment = VerticalAlignment.Center, FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = new SolidColorBrush(ThemeHelper.TextDark) });
        f.Children.Add(desde);
        f.Children.Add(new TextBlock { Text = "Hasta:", Width = 55, VerticalAlignment = VerticalAlignment.Center, FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = new SolidColorBrush(ThemeHelper.TextDark) });
        f.Children.Add(hasta);
        f.Children.Add(generar);
        f.Children.Add(exportar);

        filterBorder.Child = f;
        DockPanel.SetDock(filterBorder, Dock.Top);
        root.Children.Add(filterBorder);

        var filterPanel = new WrapPanel();
        filterPanel.Margin = new Thickness(10, 0, 10, 5);

        _filterBox = new TextBox
        {
            Width = 280,
            Height = 30,
            PlaceholderText = "Buscar en reporte...",
            FontSize = 13,
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
            CornerRadius = new CornerRadius(6)
        };
        _filterBox.TextChanged += (s, e) => ApplyFilter();

        var filterLabel = new TextBlock
        {
            Text = "Filtrar: ",
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(ThemeHelper.TextDark),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 6, 0)
        };

        filterPanel.Children.Add(filterLabel);
        filterPanel.Children.Add(_filterBox);

        var gridBorder = new Border();
        gridBorder.Margin = new Thickness(10, 0, 10, 10);
        gridBorder.Background = Brushes.White;
        gridBorder.CornerRadius = new CornerRadius(8);
        gridBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        gridBorder.BorderThickness = new Thickness(1);

        var gridDock = new DockPanel();
        DockPanel.SetDock(filterPanel, Dock.Top);
        gridDock.Children.Add(filterPanel);

        grid = new DataGrid();
        grid.IsReadOnly = true;
        grid.AutoGenerateColumns = true;
        grid.Background = Brushes.White;
        grid.Foreground = Brushes.Black;
        grid.BorderThickness = new Thickness(0);
        gridDock.Children.Add(grid);
        gridBorder.Child = gridDock;
        root.Children.Add(gridBorder);

        Content = root;
    }

    private void ApplyFilter()
    {
        if (_dataList == null) return;
        string filter = _filterBox.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(filter))
        {
            grid.ItemsSource = _dataList;
            return;
        }

        var filtered = _dataList.Cast<object>()
            .Where(item =>
            {
                var props = item.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string));
                foreach (var prop in props)
                {
                    var val = prop.GetValue(item) as string;
                    if (val != null && val.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            })
            .ToList();

        grid.ItemsSource = filtered;
    }

    private async void Generar()
    {
        try
        {
            if (tipo.SelectedIndex == 0)
            {
                _dataList = (System.Collections.IList)
                    await _ventaService.GetReporteVentasAsync(
                        desde.SelectedDate?.Date ?? DateTime.Today.AddMonths(-1),
                        hasta.SelectedDate?.Date ?? DateTime.Today);
            }
            else if (tipo.SelectedIndex == 1)
            {
                _dataList = (System.Collections.IList)
                    await _ventaService.GetInventarioAsync();
            }
            else
            {
                _dataList = (System.Collections.IList)
                    await _ventaService.GetVentasPorSucursalAsync();
            }
            _filterBox.Text = "";
            grid.ItemsSource = _dataList;
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al generar reporte: " + ex.Message);
        }
    }

    private async Task ExportarCsvAsync()
    {
        if (grid.ItemsSource == null) { await MessageBoxHelper.Show(this, "Primero genere un reporte."); return; }

        var options = new FilePickerSaveOptions
        {
            DefaultExtension = "csv",
            FileTypeChoices = new[] { new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } } },
            SuggestedFileName = "reporte_huixin_motors.csv"
        };

        var file = await StorageProvider.SaveFilePickerAsync(options);
        if (file == null) return;

        var items = grid.ItemsSource.Cast<object>().ToList();
        if (items.Count == 0) return;

        var props = items[0].GetType().GetProperties();
        var sb = new StringBuilder();

        foreach (var prop in props)
            sb.Append(prop.Name).Append(';');
        sb.AppendLine();

        foreach (var item in items)
        {
            foreach (var prop in props)
            {
                var val = prop.GetValue(item);
                sb.Append(val?.ToString()?.Replace(";", ",") ?? "").Append(';');
            }
            sb.AppendLine();
        }

        await using var stream = await file.OpenWriteAsync();
        await using var writer = new StreamWriter(stream, Encoding.UTF8);
        await writer.WriteAsync(sb.ToString());

        await MessageBoxHelper.Show(this, "Reporte exportado correctamente.");
    }
}
