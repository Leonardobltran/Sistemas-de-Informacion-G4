using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.PL.Controls;
using HuixinMotors.PL.Helpers;

namespace HuixinMotors.PL.Views;

public class MapaWindow : Window
{
    private readonly IVentaService _ventaService;
    private DataGrid grid;
    private MapaControl mapa;

    public MapaWindow(IVentaService ventaService)
    {
        _ventaService = ventaService;

        Title = "Mapa comercial Huixin Motors";
        Width = 1050;
        Height = 650;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI();
        _ = LoadDataAsync();
    }

    private void BuildUI()
    {
        var root = new DockPanel();
        root.Background = new SolidColorBrush(ThemeHelper.Fondo);

        var titleBar = ThemeHelper.TitleBar("Mapa de sucursales y ventas", this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        var split = new Grid();
        split.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(620, GridUnitType.Pixel)));
        split.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));

        var splitter = new GridSplitter();
        splitter.Width = 4;
        splitter.HorizontalAlignment = HorizontalAlignment.Right;
        Grid.SetColumn(splitter, 0);
        split.Children.Add(splitter);

        mapa = new MapaControl();
        Grid.SetColumn(mapa, 0);
        split.Children.Add(mapa);

        var gridBorder = new Border();
        gridBorder.Background = Brushes.White;
        gridBorder.CornerRadius = new CornerRadius(8);
        gridBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        gridBorder.BorderThickness = new Thickness(1);
        gridBorder.Margin = new Thickness(4);

        grid = new DataGrid();
        grid.IsReadOnly = true;
        grid.AutoGenerateColumns = true;
        grid.Background = Brushes.White;
        grid.Foreground = Brushes.Black;
        grid.BorderThickness = new Thickness(0);
        gridBorder.Child = grid;
        Grid.SetColumn(gridBorder, 1);
        split.Children.Add(gridBorder);

        root.Children.Add(split);
        Content = root;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var data = await _ventaService.GetVentasPorSucursalAsync();
            grid.ItemsSource = data;
            mapa.SetData(data);
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al cargar datos del mapa: " + ex.Message);
        }
    }
}
