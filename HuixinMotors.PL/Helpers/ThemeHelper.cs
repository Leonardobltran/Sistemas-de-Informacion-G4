using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;

namespace HuixinMotors.PL.Helpers;

public static class ThemeHelper
{
    public static readonly Color Primary = Color.FromArgb(255, 13, 148, 136);
    public static readonly Color PrimaryLight = Color.FromArgb(255, 45, 212, 191);
    public static readonly Color PrimaryDark = Color.FromArgb(255, 15, 118, 110);
    public static readonly Color Accent = Color.FromArgb(255, 14, 116, 144);
    public static readonly Color Danger = Color.FromArgb(255, 211, 47, 47);
    public static readonly Color DangerLight = Color.FromArgb(255, 198, 40, 40);
    public static readonly Color Fondo = Color.FromArgb(255, 242, 246, 250);
    public static readonly Color CardBg = Color.FromArgb(255, 255, 255, 255);
    public static readonly Color TextDark = Color.FromArgb(255, 26, 39, 55);

    public static Image BrandLogo(double size = 38)
    {
        var img = new Image { Width = size, Height = size, Stretch = Stretch.Uniform };
        var file = Path.Combine(AppContext.BaseDirectory, "Assets", "logo_huixin.png");
        if (File.Exists(file)) img.Source = new Bitmap(file);
        return img;
    }

    private static void AddHover(Button b, Color normal, Color hover)
    {
        b.PointerEntered += (s, e) => b.Background = new SolidColorBrush(hover);
        b.PointerExited += (s, e) => b.Background = new SolidColorBrush(normal);
    }

    public static Button Button(string text)
    {
        Button b = new Button();
        b.Content = text;
        b.Height = 38;
        b.Width = 145;
        b.Background = new SolidColorBrush(Primary);
        b.Foreground = Brushes.White;
        b.FontSize = 13;
        b.FontWeight = FontWeight.Bold;
        b.BorderThickness = new Thickness(0);
        b.CornerRadius = new CornerRadius(8);
        b.Cursor = new Cursor(StandardCursorType.Hand);
        AddHover(b, Primary, PrimaryLight);
        return b;
    }

    public static Button DangerButton(string text)
    {
        Button b = Button(text);
        b.Background = new SolidColorBrush(Danger);
        AddHover(b, Danger, DangerLight);
        return b;
    }

    public static TextBlock Title(string text)
    {
        TextBlock tb = new TextBlock();
        tb.Text = text;
        tb.Height = 56;
        tb.VerticalAlignment = VerticalAlignment.Center;
        tb.Padding = new Thickness(24, 0, 0, 0);
        tb.FontSize = 22;
        tb.FontWeight = FontWeight.Bold;
        tb.Background = new SolidColorBrush(Primary);
        tb.Foreground = Brushes.White;
        return tb;
    }

    public static Grid TitleBar(string text, Window owner)
    {
        var bar = new Grid();
        bar.Height = 56;
        bar.Background = new SolidColorBrush(Primary);
        bar.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        bar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        var brand = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Margin = new Thickness(18, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        brand.Children.Add(BrandLogo(40));
        var tb = Title(text);
        tb.Padding = new Thickness(0);
        brand.Children.Add(tb);
        Grid.SetColumn(brand, 0);
        bar.Children.Add(brand);

        var back = new Button();
        back.Content = "<< Atras";
        back.Height = 34;
        back.Width = 110;
        back.Background = new SolidColorBrush(PrimaryLight);
        back.Foreground = Brushes.White;
        back.FontSize = 14;
        back.FontWeight = FontWeight.Bold;
        back.BorderThickness = new Thickness(0);
        back.CornerRadius = new CornerRadius(6);
        back.Cursor = new Cursor(StandardCursorType.Hand);
        back.VerticalAlignment = VerticalAlignment.Center;
        back.Margin = new Thickness(0, 0, 14, 0);
        back.PointerEntered += (s, e) => back.Background = new SolidColorBrush(PrimaryDark);
        back.PointerExited += (s, e) => back.Background = new SolidColorBrush(PrimaryLight);
        back.Click += (s, e) => owner.Close();
        Grid.SetColumn(back, 1);
        bar.Children.Add(back);

        return bar;
    }
}
