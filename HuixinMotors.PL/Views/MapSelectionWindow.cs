using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using HuixinMotors.PL.Controls;
using HuixinMotors.PL.Helpers;

namespace HuixinMotors.PL.Views;

public class MapSelectionWindow : Window
{
    private MapaControl mapa;
    public bool Confirmed { get; private set; }
    public double SelectedLatitude => mapa.SelectedLat;
    public double SelectedLongitude => mapa.SelectedLon;

    public MapSelectionWindow(double? initialLat = null, double? initialLon = null)
    {
        Title = "Seleccionar ubicación en el mapa de Bolivia";
        Width = 800;
        Height = 550;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI(initialLat, initialLon);
    }

    private void BuildUI(double? initialLat, double? initialLon)
    {
        var root = new DockPanel();

        // Title bar
        var titleBar = ThemeHelper.TitleBar("Selector de coordenadas geográficas", this);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        // Buttons at the bottom
        var btnPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 15,
            Margin = new Thickness(15),
            Height = 45
        };
        DockPanel.SetDock(btnPanel, Dock.Bottom);

        var btnConfirmar = ThemeHelper.Button("Confirmar");
        btnConfirmar.Click += (s, e) =>
        {
            Confirmed = true;
            this.Close();
        };

        var btnCancelar = ThemeHelper.DangerButton("Cancelar");
        btnCancelar.Click += (s, e) =>
        {
            Confirmed = false;
            this.Close();
        };

        btnPanel.Children.Add(btnConfirmar);
        btnPanel.Children.Add(btnCancelar);
        root.Children.Add(btnPanel);

        // Map Control in the center
        mapa = new MapaControl();
        mapa.SelectionMode = true;

        if (initialLat.HasValue && initialLon.HasValue && initialLat != 0 && initialLon != 0)
        {
            mapa.SetSelection(initialLat.Value, initialLon.Value);
        }
        else
        {
            // Default center is Bolivia center
            mapa.SetSelection(-16.5, -63.5);
        }

        root.Children.Add(mapa);
        Content = root;
    }
}
