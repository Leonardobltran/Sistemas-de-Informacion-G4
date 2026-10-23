using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.PL.Helpers;

namespace HuixinMotors.PL.Views;

public class DashboardWindow : Window
{
    private readonly IDashboardService _dashboardService;
    private WrapPanel cards;
    private DataGrid grid;

    public DashboardWindow(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;

        Title = "Dashboard Huixin Motors";
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

        var titleBar = ThemeHelper.TitleBar("Dashboard de informacion", this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        var sv = new ScrollViewer();
        sv.Margin = new Thickness(10, 5, 10, 0);
        cards = new WrapPanel();
        cards.Margin = new Thickness(10);
        cards.Background = new SolidColorBrush(ThemeHelper.Fondo);
        sv.Content = cards;
        DockPanel.SetDock(sv, Dock.Top);
        sv.Height = 170;
        root.Children.Add(sv);

        var gridBorder = new Border();
        gridBorder.Margin = new Thickness(10, 5, 10, 10);
        gridBorder.Background = Brushes.White;
        gridBorder.CornerRadius = new CornerRadius(8);
        gridBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        gridBorder.BorderThickness = new Thickness(1);

        grid = new DataGrid();
        grid.IsReadOnly = true;
        grid.AutoGenerateColumns = true;
        grid.Background = Brushes.White;
        grid.Foreground = Brushes.Black;
        grid.BorderThickness = new Thickness(0);
        gridBorder.Child = grid;
        root.Children.Add(gridBorder);

        Content = root;
    }

    private void AddCard(string title, string value)
    {
        var p = new Border();
        p.Width = 150;
        p.Height = 100;
        p.Background = Brushes.White;
        p.Margin = new Thickness(8);
        p.CornerRadius = new CornerRadius(10);

        var stack = new StackPanel();
        stack.VerticalAlignment = VerticalAlignment.Center;

        var a = new TextBlock
        {
            Text = title,
            Height = 35,
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(ThemeHelper.Primary),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            LineHeight = 16
        };
        var b = new TextBlock
        {
            Text = value,
            FontSize = 22,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(ThemeHelper.Accent),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        stack.Children.Add(a);
        stack.Children.Add(b);
        p.Child = stack;
        cards.Children.Add(p);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var resumen = await _dashboardService.GetResumenAsync();
            if (resumen != null)
            {
                AddCard("Clientes", resumen.ClientesActivos.ToString());
                AddCard("Disponibles", resumen.VehiculosDisponibles.ToString());
                AddCard("Vendidos", resumen.VehiculosVendidos.ToString());
                AddCard("Ventas", resumen.TotalVentas.ToString());
                AddCard("Ingresos", "$ " + resumen.IngresosTotales.ToString("0.00"));
                AddCard("Promedio", "$ " + resumen.PromedioVenta.ToString("0.00"));
            }

            var ultimos = await _dashboardService.GetUltimosIngresosAsync(10);
            grid.ItemsSource = ultimos;
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al cargar dashboard: " + ex.Message);
        }
    }
}
