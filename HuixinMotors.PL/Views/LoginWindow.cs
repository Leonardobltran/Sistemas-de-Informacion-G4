using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using HuixinMotors.BRL.Interfaces;
using HuixinMotors.PL.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HuixinMotors.PL.Views;

public class LoginWindow : Window
{
    private readonly IEmpleadoService _empleadoService;
    private TextBox txtUsuario;
    private TextBox txtClave;

    public LoginWindow(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
        Title = "Huixin Motors - Iniciar Sesión";
        Width = 400;
        Height = 450;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = false;
        Background = new SolidColorBrush(ThemeHelper.Fondo);
        BuildUI();
    }

    private void BuildUI()
    {
        var root = new StackPanel
        {
            Margin = new Thickness(35),
            Spacing = 12,
            VerticalAlignment = VerticalAlignment.Center
        };


        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo_huixin.png");
        if (File.Exists(logoPath))
        {
            var logo = new Image
            {
                Source = new Bitmap(logoPath),
                Width = 125,
                Height = 105,
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };
            root.Children.Add(logo);
        }

        var title = new TextBlock
        {
            Text = "HUIXIN MOTORS",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(ThemeHelper.Primary),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 5)
        };
        root.Children.Add(title);

        var subtitle = new TextBlock
        {
            Text = "Ingrese sus credenciales de acceso",
            FontSize = 13,
            Foreground = new SolidColorBrush(ThemeHelper.TextDark),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 15)
        };
        root.Children.Add(subtitle);

        // Usuario
        var lblUsuario = new TextBlock
        {
            Text = "Usuario:",
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(ThemeHelper.TextDark)
        };
        root.Children.Add(lblUsuario);

        txtUsuario = new TextBox
        {
            Height = 32,
            FontSize = 13,
            CornerRadius = new CornerRadius(6)
        };
        root.Children.Add(txtUsuario);

        // Clave
        var lblClave = new TextBlock
        {
            Text = "Contraseña:",
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(ThemeHelper.TextDark)
        };
        root.Children.Add(lblClave);

        txtClave = new TextBox
        {
            Height = 32,
            FontSize = 13,
            PasswordChar = '*',
            CornerRadius = new CornerRadius(6)
        };
        txtClave.KeyDown += async (s, e) =>
        {
            if (e.Key == Key.Enter) await DoLoginAsync();
        };
        root.Children.Add(txtClave);

        // Button
        var btnLogin = ThemeHelper.Button("Iniciar Sesión");
        btnLogin.HorizontalAlignment = HorizontalAlignment.Center;
        btnLogin.Margin = new Thickness(0, 15, 0, 0);
        btnLogin.Click += async (s, e) => await DoLoginAsync();
        root.Children.Add(btnLogin);

        Content = root;
    }

    private async Task DoLoginAsync()
    {
        string user = txtUsuario.Text?.Trim();
        string pass = txtClave.Text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            await MessageBoxHelper.Show(this, "Por favor ingrese usuario y contraseña.", "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
            return;
        }

        try
        {
            var emp = await _empleadoService.AuthenticateAsync(user, pass);
            
            // Switch windows and pass current user to the main window
            var mainWin = ActivatorUtilities.CreateInstance<MainWindow>(App.Services, emp);
            mainWin.Show();
            
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = mainWin;
            }
            this.Close();
        }
        catch (HuixinMotors.BRL.BusinessRuleException ex)
        {
            await MessageBoxHelper.Show(this, ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            await MessageBoxHelper.Show(this, "Error al iniciar sesión: " + ex.Message, "Huixin Motors", MsgBoxButton.Ok, MsgBoxIcon.Error);
        }
    }
}
