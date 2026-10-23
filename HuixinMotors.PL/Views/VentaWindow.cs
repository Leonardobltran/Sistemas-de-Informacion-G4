using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.PL.Helpers;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.PL.Views;

public class VentaWindow : Window
{
    private readonly IVentaService _ventaService;
    private readonly IVehiculoService _vehiculoService;
    private readonly IClienteService _clienteService;
    private readonly IEmpleadoService _empleadoService;

    private ComboBox cboCliente, cboEmpleado, cboVehiculo, cboPago;
    private TextBox txtFactura, txtPrecio, txtDescuento, txtImpuesto, txtTotal, txtObs;
    private DataGrid grid;
    private List<ReporteVentaDto> _dataList;
    private TextBox _filterBox;
    private object currentVentaId;
    private object currentVehiculoId;
    private const int lw = 105;

    public VentaWindow(
        IVentaService ventaService,
        IVehiculoService vehiculoService,
        IClienteService clienteService,
        IEmpleadoService empleadoService)
    {
        _ventaService = ventaService;
        _vehiculoService = vehiculoService;
        _clienteService = clienteService;
        _empleadoService = empleadoService;

        Title = "Gestion de Ventas";
        Width = 1150;
        Height = 720;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI();
        _ = LoadCombosAsync();
        _ = LoadDataAsync();
    }

    private void BuildUI()
    {
        var root = new DockPanel();
        root.Background = new SolidColorBrush(ThemeHelper.Fondo);

        var titleBar = ThemeHelper.TitleBar("CRUD Ventas - Huixin Motors", this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        var topBorder = new Border();
        topBorder.Padding = new Thickness(20);
        topBorder.Background = Brushes.White;
        topBorder.CornerRadius = new CornerRadius(0, 0, 12, 12);
        topBorder.Height = 330;
        DockPanel.SetDock(topBorder, Dock.Top);
        root.Children.Add(topBorder);

        var canvas = new Canvas();
        topBorder.Child = canvas;

        int cw = 220;
        int gap = 30;
        int col1 = 15;
        int col2 = col1 + lw + cw + gap;
        int col3 = col2 + lw + cw + gap;

        double y0 = 12, y1 = 58, y2 = 104, y3 = 150, y4 = 200;

        cboCliente = AddCombo(canvas, "Cliente", col1, y0, cw);
        cboEmpleado = AddCombo(canvas, "Empleado", col2, y0, cw);
        cboVehiculo = AddCombo(canvas, "Vehiculo disponible", col3, y0, cw);

        txtFactura = AddText(canvas, "Factura", col1, y1, cw);
        cboPago = AddCombo(canvas, "Metodo pago", col2, y1, cw);
        cboPago.Items.Add("Efectivo");
        cboPago.Items.Add("Transferencia");
        cboPago.Items.Add("Tarjeta");
        cboPago.Items.Add("Credito");
        cboPago.SelectedIndex = 0;
        txtPrecio = AddText(canvas, "Precio final", col3, y1, cw);

        txtDescuento = AddText(canvas, "Descuento", col1, y2, cw);
        txtImpuesto = AddText(canvas, "Impuesto", col2, y2, cw);
        txtTotal = AddText(canvas, "Total", col3, y2, cw);

        txtObs = AddText(canvas, "Observacion", col1, y3, cw * 2 + lw + gap);

        var calcular = ThemeHelper.Button("Calcular total");
        Canvas.SetLeft(calcular, col1); Canvas.SetTop(calcular, y4);

        var guardar = ThemeHelper.Button("Guardar venta");
        Canvas.SetLeft(guardar, col1 + 165); Canvas.SetTop(guardar, y4);

        var nuevo = ThemeHelper.Button("Nuevo");
        Canvas.SetLeft(nuevo, col1 + 330); Canvas.SetTop(nuevo, y4);

        var eliminar = ThemeHelper.DangerButton("Eliminar venta");
        Canvas.SetLeft(eliminar, col1 + 495); Canvas.SetTop(eliminar, y4);

        cboVehiculo.SelectionChanged += (s, e) => CalcularTotal();
        calcular.Click += (s, e) => CalcularTotal();
        guardar.Click += async (s, e) => await GuardarAsync();
        eliminar.Click += async (s, e) => await EliminarAsync();
        nuevo.Click += (s, e) => Limpiar();

        canvas.Children.Add(calcular);
        canvas.Children.Add(guardar);
        canvas.Children.Add(eliminar);
        canvas.Children.Add(nuevo);

        var filterPanel = new WrapPanel();
        filterPanel.Margin = new Thickness(10, 8, 10, 5);

        _filterBox = new TextBox
        {
            Width = 280,
            Height = 30,
            PlaceholderText = "Buscar...",
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
        grid.SelectionMode = DataGridSelectionMode.Single;
        grid.Background = Brushes.White;
        grid.Foreground = Brushes.Black;
        grid.BorderThickness = new Thickness(0);
        grid.SelectionChanged += (s, e) => Seleccionar();
        gridDock.Children.Add(grid);
        gridBorder.Child = gridDock;
        root.Children.Add(gridBorder);

        Content = root;
    }

    private TextBox AddText(Canvas p, string label, double x, double y, double width)
    {
        var l = new TextBlock { Text = label, Width = lw, FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = new SolidColorBrush(ThemeHelper.TextDark), VerticalAlignment = VerticalAlignment.Center };
        Canvas.SetLeft(l, x); Canvas.SetTop(l, y + 5);
        var t = new TextBox { Width = width, Height = 30, FontSize = 13, BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)), CornerRadius = new CornerRadius(6) };
        Canvas.SetLeft(t, x + lw); Canvas.SetTop(t, y);
        p.Children.Add(l); p.Children.Add(t);
        return t;
    }

    private ComboBox AddCombo(Canvas p, string label, double x, double y, double width)
    {
        var l = new TextBlock { Text = label, Width = lw, FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = new SolidColorBrush(ThemeHelper.TextDark), VerticalAlignment = VerticalAlignment.Center };
        Canvas.SetLeft(l, x); Canvas.SetTop(l, y + 5);
        var c = new ComboBox { Width = width, Height = 30, FontSize = 13 };
        Canvas.SetLeft(c, x + lw); Canvas.SetTop(c, y);
        p.Children.Add(l); p.Children.Add(c);
        return c;
    }

    private async Task LoadCombosAsync()
    {
        try
        {
            var clientes = await _clienteService.GetComboAsync();
            LoadCombo(cboCliente, clientes);

            var empleados = await _empleadoService.GetComboAsync();
            LoadCombo(cboEmpleado, empleados);

            var vehiculos = await _vehiculoService.GetDisponiblesComboAsync();
            LoadCombo(cboVehiculo, vehiculos);
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al cargar combos: " + ex.Message);
        }
    }

    private void LoadCombo(ComboBox cbo, List<ComboItemDto> items)
    {
        cbo.Items.Clear();
        foreach (var item in items)
            cbo.Items.Add(item);
        if (cbo.Items.Count > 0)
            cbo.SelectedIndex = 0;
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
        var filtered = _dataList
            .Where(r =>
                (r.Cliente?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.NumeroFactura?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.Marca?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();
        grid.ItemsSource = filtered;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _dataList = await _ventaService.GetReporteVentasAsync(
                DateTime.Today.AddYears(-1), DateTime.Today.AddDays(1));
            grid.ItemsSource = _dataList;
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al cargar datos: " + ex.Message);
        }
    }

    private void CalcularTotal()
    {
        if (cboVehiculo.SelectedItem is not ComboItemDto item) return;

        try
        {
            decimal precio = 0;
            if (decimal.TryParse(txtPrecio.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal p))
                precio = p;

            decimal descuento = Parse(txtDescuento.Text);
            decimal impuesto = Parse(txtImpuesto.Text);
            txtTotal.Text = _ventaService.CalcularTotal(precio, descuento, impuesto).ToString("0.00");
        }
        catch { }
    }

    private decimal Parse(string text)
    {
        if (decimal.TryParse((text ?? "0").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
            return value;
        return 0;
    }

    private async Task GuardarAsync()
    {
        try
        {
            if (cboCliente.SelectedItem is not ComboItemDto cli) return;
            if (cboEmpleado.SelectedItem is not ComboItemDto emp) return;
            if (cboVehiculo.SelectedItem is not ComboItemDto veh) return;

            var venta = new Venta
            {
                IdCliente = cli.Value,
                IdEmpleado = emp.Value,
                IdVehiculo = veh.Value,
                NumeroFactura = (txtFactura.Text ?? string.Empty).Trim(),
                MetodoPago = cboPago.Text ?? string.Empty,
                PrecioFinal = Parse(txtPrecio.Text),
                Descuento = Parse(txtDescuento.Text),
                Impuesto = Parse(txtImpuesto.Text),
                Observacion = (txtObs.Text ?? string.Empty).Trim()
            };

            await _ventaService.RegistrarVenta(venta);
            await MessageBoxHelper.Show(this, "Venta registrada correctamente.");
            Limpiar();
            await LoadCombosAsync();
            await LoadDataAsync();
        }
        catch (HuixinMotors.BRL.BusinessRuleException ex)
        {
            await MessageBoxHelper.Show(this, ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al guardar venta: " + ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Error);
        }
    }

    private void Seleccionar()
    {
        if (grid.SelectedItem is not ReporteVentaDto row) return;

        currentVentaId = row.IdVenta;
        txtFactura.Text = row.NumeroFactura?.ToString() ?? "";
        txtPrecio.Text = row.PrecioFinal.ToString("0.00");
        txtDescuento.Text = row.Descuento.ToString("0.00");
        txtImpuesto.Text = row.Impuesto.ToString("0.00");
        txtTotal.Text = row.Total.ToString("0.00");
        txtObs.Text = row.Observacion ?? "";
    }

    private async Task EliminarAsync()
    {
        if (currentVentaId == null) { await MessageBoxHelper.Show(this, "Seleccione una venta."); return; }
        var result = await MessageBoxHelper.Show(this, "Eliminar venta seleccionada? El vehiculo volvera a Disponible.", "Confirmar", MsgBoxButton.YesNo);
        if (result == MsgBoxResult.No) return;
        try
        {
            await _ventaService.EliminarVenta((int)currentVentaId, (int)(currentVehiculoId ?? 0));
            Limpiar();
            await LoadCombosAsync();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al eliminar: " + ex.Message);
        }
    }

    private void Limpiar()
    {
        currentVentaId = null; currentVehiculoId = null;
        txtFactura.Text = "FAC-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        txtPrecio.Text = ""; txtDescuento.Text = "0"; txtImpuesto.Text = "0"; txtTotal.Text = ""; txtObs.Text = "";
    }
}
