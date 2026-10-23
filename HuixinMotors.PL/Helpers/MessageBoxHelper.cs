using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace HuixinMotors.PL.Helpers;

public enum MsgBoxButton { Ok, YesNo }
public enum MsgBoxIcon { None, Information, Question, Error, Warning }
public enum MsgBoxResult { Ok, Yes, No }

public static class MessageBoxHelper
{
    public static async Task<MsgBoxResult> Show(Window owner, string message, string title = "",
        MsgBoxButton button = MsgBoxButton.Ok, MsgBoxIcon icon = MsgBoxIcon.None)
    {
        var dialog = new Window
        {
            Width = 420,
            Height = 190,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            WindowDecorations = Avalonia.Controls.WindowDecorations.None,
            Background = new SolidColorBrush(ThemeHelper.Fondo)
        };

        var root = new DockPanel();

        var titleBar = new Border
        {
            Height = 40,
            Background = new SolidColorBrush(ThemeHelper.Primary),
            Child = new TextBlock
            {
                Text = title,
                FontSize = 15,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(14, 0, 0, 0)
            }
        };
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);

        var bodyPanel = new StackPanel
        {
            Margin = new Thickness(20, 16, 20, 12),
            Spacing = 12
        };

        var msgPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12
        };

        if (icon != MsgBoxIcon.None)
        {
            var iconText = new TextBlock
            {
                FontSize = 28,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 36,
                TextAlignment = TextAlignment.Center
            };
            iconText.Text = icon switch
            {
                MsgBoxIcon.Information => "\u2139",
                MsgBoxIcon.Question => "\u2753",
                MsgBoxIcon.Error => "\u2716",
                MsgBoxIcon.Warning => "\u26A0",
                _ => ""
            };
            iconText.Foreground = icon switch
            {
                MsgBoxIcon.Information => new SolidColorBrush(ThemeHelper.Primary),
                MsgBoxIcon.Question => new SolidColorBrush(ThemeHelper.Accent),
                MsgBoxIcon.Error => new SolidColorBrush(ThemeHelper.Danger),
                MsgBoxIcon.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 152, 0)),
                _ => Brushes.Black
            };
            msgPanel.Children.Add(iconText);
        }

        var msgBlock = new TextBlock
        {
            Text = message,
            FontSize = 14,
            TextWrapping = TextWrapping.Wrap,
            Foreground = new SolidColorBrush(ThemeHelper.TextDark),
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 300
        };
        msgPanel.Children.Add(msgBlock);
        bodyPanel.Children.Add(msgPanel);

        var btnPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 12,
            Margin = new Thickness(0, 6, 0, 0)
        };

        var result = MsgBoxResult.Ok;

        if (button == MsgBoxButton.Ok)
        {
            var btn = ThemeHelper.Button("Aceptar");
            btn.Width = 110;
            btn.Height = 36;
            btn.Click += (s, e) => { result = MsgBoxResult.Ok; dialog.Close(); };
            btnPanel.Children.Add(btn);
        }
        else
        {
            var btnSi = ThemeHelper.Button("Si");
            btnSi.Width = 100;
            btnSi.Height = 36;
            btnSi.Click += (s, e) => { result = MsgBoxResult.Yes; dialog.Close(); };
            btnPanel.Children.Add(btnSi);

            var btnNo = ThemeHelper.DangerButton("No");
            btnNo.Width = 100;
            btnNo.Height = 36;
            btnNo.Click += (s, e) => { result = MsgBoxResult.No; dialog.Close(); };
            btnPanel.Children.Add(btnNo);
        }

        bodyPanel.Children.Add(btnPanel);
        root.Children.Add(bodyPanel);
        dialog.Content = root;

        await dialog.ShowDialog(owner);
        return result;
    }
}
