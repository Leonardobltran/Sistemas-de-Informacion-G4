using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using HuixinMotors.PL.Helpers;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.PL.Views;

public class GenericCrudWindow : Window
{
    private readonly string _idColumn;
    private readonly List<FieldDef> _fields;
    private readonly Func<Task<IList>> _loadAll;
    private readonly Func<object, Dictionary<string, string>, Task> _save;
    private readonly Func<object, Task> _delete;
    private readonly Dictionary<string, Control> _inputs = new();
    private DataGrid _grid;
    private object _currentId;
    private IList _dataList;
    private TextBox _filterBox;
    private readonly bool _soloLectura;

    public GenericCrudWindow(
        string title,
        string idColumn,
        List<FieldDef> fields,
        Func<Task<IList>> loadAll,
        Func<object, Dictionary<string, string>, Task> save,
        Func<object, Task> delete,
        bool soloLectura = false)
    {
        Title = title;
        _idColumn = idColumn;
        _fields = fields;
        _loadAll = loadAll;
        _save = save;
        _delete = delete;
        _soloLectura = soloLectura;

        Width = 1150;
        Height = 720;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI(title);

        _ = LoadDataAsync();
    }

    private void BuildUI(string title)
    {
        var root = new DockPanel();
        root.Background = new SolidColorBrush(ThemeHelper.Fondo);

        var titleBar = ThemeHelper.TitleBar(title, this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        var topBorder = new Border();
        topBorder.Padding = new Thickness(20);
        topBorder.Background = Brushes.White;
        topBorder.CornerRadius = new CornerRadius(0, 0, 12, 12);
        DockPanel.SetDock(topBorder, Dock.Top);
        root.Children.Add(topBorder);

        var canvas = new Canvas();
        int labelW = 110;
        int inputW = 200;
        double x = 20, y = 15;
        int col = 0;

        foreach (FieldDef f in _fields)
        {
            var lbl = new TextBlock
            {
                Text = f.Label,
                Width = labelW,
                FontSize = 13,
                FontWeight = FontWeight.SemiBold,
                Foreground = new SolidColorBrush(ThemeHelper.TextDark),
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(lbl, x);
            Canvas.SetTop(lbl, y + 5);
            canvas.Children.Add(lbl);

            Control ctrl;
            if (f.Type == FieldType.Boolean)
            {
                ctrl = new CheckBox
                {
                    Content = "Activo",
                    Width = inputW,
                    Height = 30,
                    FontSize = 13,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            else if (f.Type == FieldType.Combo)
            {
                var cbo = new ComboBox
                {
                    Width = inputW,
                    Height = 30,
                    FontSize = 13,
                    CornerRadius = new CornerRadius(6)
                };
                cbo.ItemsSource = f.ComboItems;
                cbo.DisplayMemberBinding = new Avalonia.Data.Binding("Text");
                ctrl = cbo;
            }
            else if (f.Type == FieldType.StaticCombo)
            {
                var cbo = new ComboBox
                {
                    Width = inputW,
                    Height = 30,
                    FontSize = 13,
                    CornerRadius = new CornerRadius(6)
                };
                cbo.ItemsSource = f.StaticItems;
                ctrl = cbo;
            }
            else
            {
                ctrl = new TextBox
                {
                    Width = inputW,
                    Height = 30,
                    FontSize = 13,
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
                    CornerRadius = new CornerRadius(6)
                };
            }

            if (_soloLectura)
            {
                ctrl.IsEnabled = false;
            }

            Canvas.SetLeft(ctrl, x + labelW);
            Canvas.SetTop(ctrl, y);
            canvas.Children.Add(ctrl);
            _inputs[f.Column] = ctrl;

            col++;
            if (col == 3) { col = 0; x = 20; y += 42; }
            else { x += labelW + inputW + 30; }
        }

        double btnY = y + 42;
        double borderH = btnY + 85;

        var buttonsPanel = new WrapPanel();
        Canvas.SetLeft(buttonsPanel, 20);
        Canvas.SetTop(buttonsPanel, btnY);
        buttonsPanel.Width = 850;

        var actualizar = ThemeHelper.Button("Actualizar lista");
        actualizar.Click += async delegate { await LoadDataAsync(); };

        if (!_soloLectura)
        {
            var nuevo = ThemeHelper.Button("Nuevo");
            var guardar = ThemeHelper.Button("Guardar");
            var eliminar = ThemeHelper.DangerButton("Eliminar");

            nuevo.Click += delegate { ClearInputs(); };
            guardar.Click += async delegate { await SaveAsync(); };
            eliminar.Click += async delegate { await DeleteAsync(); };

            buttonsPanel.Children.Add(nuevo);
            buttonsPanel.Children.Add(guardar);
            buttonsPanel.Children.Add(eliminar);
        }

        buttonsPanel.Children.Add(actualizar);

        // Add custom button "Seleccionar en mapa" for Branches (Sucursales)
        if (_idColumn == "IdSucursal")
        {
            var btnMapa = ThemeHelper.Button("Ver en Mapa");
            btnMapa.Click += async delegate { await AbrirSeleccionMapa(); };
            buttonsPanel.Children.Add(btnMapa);
        }

        // Add custom buttons for Vehicles
        if (_idColumn == "IdVehiculo")
        {
            if (!_soloLectura)
            {
                var seleccionarFotoBtn = ThemeHelper.Button("Buscar foto");
                seleccionarFotoBtn.Click += async delegate { await SeleccionarFotoVehiculoAsync(); };
                buttonsPanel.Children.Add(seleccionarFotoBtn);
            }

            var verFichaBtn = ThemeHelper.Button("Ver Ficha / Foto");
            verFichaBtn.Click += delegate { AbrirFichaVehiculo(); };
            buttonsPanel.Children.Add(verFichaBtn);
        }

        canvas.Children.Add(buttonsPanel);
        topBorder.Child = canvas;
        topBorder.Height = borderH;

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

        _grid = new DataGrid();
        _grid.IsReadOnly = true;
        _grid.AutoGenerateColumns = true;
        _grid.SelectionMode = DataGridSelectionMode.Single;
        _grid.Background = Brushes.White;
        _grid.Foreground = Brushes.Black;
        _grid.BorderThickness = new Thickness(0);
        _grid.HeadersVisibility = DataGridHeadersVisibility.All;
        _grid.SelectionChanged += delegate { FillInputsFromGrid(); };

        // Double-click grid row to open details for Vehicles
        if (_idColumn == "IdVehiculo")
        {
            _grid.DoubleTapped += (s, e) => { AbrirFichaVehiculo(); };
        }

        // Configure Grid Columns Generating Event
        _grid.AutoGeneratingColumn += (s, e) =>
        {
            string name = e.PropertyName;

            // Hide Primary Key and Passwords
            if (name == _idColumn || name == "Clave" || name == "ClaveHash")
            {
                e.Cancel = true;
                return;
            }

            // Make headers human-friendly
            e.Column.Header = FormatHeaderName(name);

            // Display "Activo" / "Inactivo" for Boolean states
            if (name == "Estado" && e.PropertyType == typeof(bool))
            {
                if (e.Column is DataGridTextColumn tc)
                {
                    tc.Binding = new Avalonia.Data.Binding(name) { Converter = new EstadoConverter(), Mode = BindingMode.OneWay };
                }
            }
            // Display description names for foreign key IDs
            else if (name is "IdProveedor" or "IdCategoria" or "IdSucursal")
            {
                var field = _fields.FirstOrDefault(f => f.Column == name);
                if (field != null && field.ComboItems != null && e.Column is DataGridTextColumn tc)
                {
                    tc.Binding = new Avalonia.Data.Binding(name) { Converter = new ComboTextConverter(field.ComboItems), Mode = BindingMode.OneWay };
                }
            }
        };

        gridDock.Children.Add(_grid);
        gridBorder.Child = gridDock;
        root.Children.Add(gridBorder);

        Content = root;
    }

    private string FormatHeaderName(string name)
    {
        if (name == "Anio") return "Año";
        if (name == "Vin") return "VIN / Chasis";
        if (name == "Placa") return "Placa";
        if (name == "PrecioCompra") return "P. Compra";
        if (name == "PrecioVenta") return "P. Venta";
        if (name == "TipoCombustible") return "Combustible";
        if (name == "ImagenUrl") return "Imagen / Foto";
        
        var result = System.Text.RegularExpressions.Regex.Replace(name, "(\\B[A-Z])", " $1");
        return result;
    }

    private void ApplyFilter()
    {
        if (_dataList == null) return;
        string filter = _filterBox.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(filter))
        {
            _grid.ItemsSource = _dataList;
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
        _grid.ItemsSource = filtered;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var data = await _loadAll();
            _dataList = data;
            _grid.ItemsSource = data;
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "No se pudo cargar la informacion.\n" + ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Error);
        }
    }

    private void ClearInputs()
    {
        foreach (var ctrl in _inputs.Values)
        {
            if (ctrl is TextBox txt) txt.Text = string.Empty;
            else if (ctrl is CheckBox chk) chk.IsChecked = false;
            else if (ctrl is ComboBox cbo) cbo.SelectedIndex = -1;
        }
        _currentId = null;
    }

    private void FillInputsFromGrid()
    {
        if (_grid.SelectedItem == null) return;
        var item = _grid.SelectedItem;
        _currentId = item.GetType().GetProperty(_idColumn)?.GetValue(item);
        foreach (FieldDef f in _fields)
        {
            var prop = item.GetType().GetProperty(f.Column);
            if (prop == null) continue;
            var val = prop.GetValue(item);

            var ctrl = _inputs[f.Column];
            if (ctrl is TextBox txt)
            {
                txt.Text = val?.ToString() ?? string.Empty;
            }
            else if (ctrl is CheckBox chk)
            {
                if (val is bool b) chk.IsChecked = b;
                else chk.IsChecked = (val?.ToString() == "1" || val?.ToString() == "True" || val?.ToString() == "activo");
            }
            else if (ctrl is ComboBox cbo)
            {
                if (f.Type == FieldType.Combo && f.ComboItems != null && val != null)
                {
                    if (int.TryParse(val.ToString(), out int id))
                    {
                        cbo.SelectedItem = f.ComboItems.FirstOrDefault(x => x.Value == id);
                    }
                    else
                    {
                        cbo.SelectedIndex = -1;
                    }
                }
                else if (f.Type == FieldType.StaticCombo)
                {
                    cbo.SelectedItem = val?.ToString();
                }
            }
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var values = new Dictionary<string, string>();
            foreach (FieldDef f in _fields)
            {
                var ctrl = _inputs[f.Column];
                string valStr = string.Empty;

                if (ctrl is TextBox txt)
                {
                    valStr = txt.Text?.Trim() ?? string.Empty;
                }
                else if (ctrl is CheckBox chk)
                {
                    valStr = chk.IsChecked == true ? "1" : "0";
                }
                else if (ctrl is ComboBox cbo)
                {
                    if (f.Type == FieldType.Combo)
                    {
                        var comboItem = cbo.SelectedItem as ComboItemDto;
                        valStr = comboItem?.Value.ToString() ?? string.Empty;
                    }
                    else if (f.Type == FieldType.StaticCombo)
                    {
                        valStr = cbo.SelectedItem?.ToString() ?? string.Empty;
                    }
                }

                if (f.Required && string.IsNullOrWhiteSpace(valStr))
                {
                    await MessageBoxHelper.Show(this, "El campo " + f.Label + " es obligatorio.", "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
                    return;
                }
                values[f.Column] = valStr;
            }
            await _save(_currentId, values);
            await MessageBoxHelper.Show(this, "Registro guardado correctamente.", "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Information);
            ClearInputs();
            await LoadDataAsync();
        }
        catch (HuixinMotors.BRL.BusinessRuleException ex)
        {
            await MessageBoxHelper.Show(this, ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al guardar: " + ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Error);
        }
    }

    private async Task DeleteAsync()
    {
        if (_currentId == null)
        {
            await MessageBoxHelper.Show(this, "Seleccione un registro primero.", "Huixin Motors");
            return;
        }
        var result = await MessageBoxHelper.Show(this, "Desea eliminar el registro seleccionado?", "Confirmar", MsgBoxButton.YesNo, MsgBoxIcon.Question);
        if (result != MsgBoxResult.Yes) return;
        try
        {
            await _delete(_currentId);
            ClearInputs();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "No se puede eliminar. Puede tener registros relacionados.\n" + ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
        }
    }

    // Branch Coordinates Map Selector Dialog
    private async Task AbrirSeleccionMapa()
    {
        double? initLat = null;
        double? initLon = null;

        if (_inputs.TryGetValue("Latitud", out var cLat) && cLat is TextBox tLat &&
            double.TryParse(tLat.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double lat))
        {
            initLat = lat;
        }
        if (_inputs.TryGetValue("Longitud", out var cLon) && cLon is TextBox tLon &&
            double.TryParse(tLon.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
        {
            initLon = lon;
        }

        var mapWin = new MapSelectionWindow(initLat, initLon);
        await mapWin.ShowDialog(this);

        if (mapWin.Confirmed)
        {
            if (_inputs.TryGetValue("Latitud", out var ctrlL) && ctrlL is TextBox txtL)
                txtL.Text = mapWin.SelectedLatitude.ToString("0.000000", CultureInfo.InvariantCulture);

            if (_inputs.TryGetValue("Longitud", out var ctrlLo) && ctrlLo is TextBox txtLo)
                txtLo.Text = mapWin.SelectedLongitude.ToString("0.000000", CultureInfo.InvariantCulture);
        }
    }

    // Select a vehicle photo from Windows file explorer and copy it to a stable app folder
    private async Task SeleccionarFotoVehiculoAsync()
    {
        if (!_inputs.TryGetValue("ImagenUrl", out var ctrl) || ctrl is not TextBox txtImagen)
        {
            await MessageBoxHelper.Show(this, "No se encontró el campo Imagen URL.", "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
            return;
        }

        var archivos = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Seleccionar foto del vehículo",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Imágenes")
                {
                    Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.webp", "*.bmp" },
                    MimeTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/bmp" }
                }
            }
        });

        if (archivos == null || archivos.Count == 0)
            return;

        var archivo = archivos[0];
        var extension = Path.GetExtension(archivo.Name);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg";

        var carpetaFotos = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HuixinMotors",
            "FotosVehiculos");
        Directory.CreateDirectory(carpetaFotos);

        var nombreSeguro = Path.GetFileNameWithoutExtension(archivo.Name)
            .Replace(" ", "_")
            .Replace("/", "_")
            .Replace("\\", "_");

        var nombreFinal = $"{nombreSeguro}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
        var rutaDestino = Path.Combine(carpetaFotos, nombreFinal);

        await using (var origen = await archivo.OpenReadAsync())
        await using (var destino = File.Create(rutaDestino))
        {
            await origen.CopyToAsync(destino);
        }

        txtImagen.Text = rutaDestino;
        await MessageBoxHelper.Show(this,
            "Foto cargada correctamente. Ahora presiona Guardar para registrar el cambio.",
            "Huixin Motors",
            MsgBoxButton.Ok,
            MsgBoxIcon.Information);
    }

    // Vehicle Detail Card Window Modal
    private async void AbrirFichaVehiculo()
    {
        if (_grid.SelectedItem == null)
        {
            await MessageBoxHelper.Show(this, "Seleccione un vehículo de la tabla.", "Huixin Motors");
            return;
        }

        var veh = _grid.SelectedItem as Vehiculo;
        if (veh == null) return;

        string catName = "No especificado";
        var catField = _fields.FirstOrDefault(x => x.Column == "IdCategoria");
        if (catField != null && catField.ComboItems != null)
        {
            catName = catField.ComboItems.FirstOrDefault(x => x.Value == veh.IdCategoria)?.Text ?? veh.IdCategoria.ToString();
        }

        string provName = "No especificado";
        var provField = _fields.FirstOrDefault(x => x.Column == "IdProveedor");
        if (provField != null && provField.ComboItems != null)
        {
            provName = provField.ComboItems.FirstOrDefault(x => x.Value == veh.IdProveedor)?.Text ?? veh.IdProveedor.ToString();
        }

        string sucName = "No especificado";
        var sucField = _fields.FirstOrDefault(x => x.Column == "IdSucursal");
        if (sucField != null && sucField.ComboItems != null)
        {
            sucName = sucField.ComboItems.FirstOrDefault(x => x.Value == veh.IdSucursal)?.Text ?? veh.IdSucursal.ToString();
        }

        var detailWin = new VehicleDetailWindow(veh, catName, provName, sucName);
        await detailWin.ShowDialog(this);
    }
}

// Display Value Converters for Grid Cells
public class ComboTextConverter : IValueConverter
{
    private readonly List<ComboItemDto> _items;
    public ComboTextConverter(List<ComboItemDto> items) => _items = items;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || _items == null) return "-";
        if (int.TryParse(value.ToString(), out int id))
        {
            var item = _items.FirstOrDefault(x => x.Value == id);
            return item?.Text ?? value.ToString();
        }
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}

public class EstadoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b) return b ? "Activo" : "Inactivo";
        return value?.ToString() ?? "-";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
