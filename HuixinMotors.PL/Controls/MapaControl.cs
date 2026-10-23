using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using HuixinMotors.PL.Helpers;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.PL.Controls;

public class MapaControl : Control
{
    private List<VentaPorSucursalDto> _data;
    private double _centerLat = -16.5;
    private double _centerLon = -63.5;
    private int _zoom = 6;

    private Point _lastPointerPos;
    private bool _isDragging;

    public bool SelectionMode { get; set; }
    public double SelectedLat { get; set; } = -16.5;
    public double SelectedLon { get; set; } = -63.5;

    private readonly ConcurrentDictionary<string, Bitmap> _tileCache = new();
    private readonly HashSet<string> _downloadingTiles = new();

    public MapaControl()
    {
        ClipToBounds = true;
    }

    public void SetData(List<VentaPorSucursalDto> data)
    {
        _data = data;
        if (data != null && data.Any())
        {
            // Center map on the first valid location if we have one
            var valid = data.FirstOrDefault(x => x.Latitud.HasValue && x.Longitud.HasValue);
            if (valid != null)
            {
                _centerLat = (double)valid.Latitud.Value;
                _centerLon = (double)valid.Longitud.Value;
                _zoom = 8;
            }
        }
        InvalidateVisual();
    }

    public void SetSelection(double lat, double lon)
    {
        SelectedLat = lat;
        SelectedLon = lon;
        _centerLat = lat;
        _centerLon = lon;
        _zoom = 13;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        double width = Bounds.Width;
        double height = Bounds.Height;

        if (width <= 0 || height <= 0) return;

        // Draw OSM Tiles
        double cx_pixel = LonToX(_centerLon, _zoom) * 256;
        double cy_pixel = LatToY(_centerLat, _zoom) * 256;

        double min_x_pixel = cx_pixel - width / 2;
        double max_x_pixel = cx_pixel + width / 2;
        double min_y_pixel = cy_pixel - height / 2;
        double max_y_pixel = cy_pixel + height / 2;

        int min_tile_x = (int)Math.Floor(min_x_pixel / 256);
        int max_tile_x = (int)Math.Floor(max_x_pixel / 256);
        int min_tile_y = (int)Math.Floor(min_y_pixel / 256);
        int max_tile_y = (int)Math.Floor(max_y_pixel / 256);

        int numTiles = (int)Math.Pow(2, _zoom);

        for (int tx = min_tile_x; tx <= max_tile_x; tx++)
        {
            for (int ty = min_tile_y; ty <= max_tile_y; ty++)
            {
                int wrappedTx = (tx % numTiles + numTiles) % numTiles;
                if (ty < 0 || ty >= numTiles) continue;

                double tileScreenX = width / 2 + (tx * 256 - cx_pixel);
                double tileScreenY = height / 2 + (ty * 256 - cy_pixel);

                Bitmap tileBmp = GetTile(_zoom, wrappedTx, ty);
                if (tileBmp != null)
                {
                    context.DrawImage(tileBmp, new Rect(0, 0, 256, 256), new Rect(tileScreenX, tileScreenY, 256, 256));
                }
                else
                {
                    context.FillRectangle(new SolidColorBrush(Color.FromArgb(255, 230, 230, 230)), new Rect(tileScreenX, tileScreenY, 256, 256));
                    context.DrawRectangle(null, new Pen(Brushes.White, 0.5), new Rect(tileScreenX, tileScreenY, 256, 256));
                }
            }
        }

        // Draw Branch Markers
        if (_data != null)
        {
            foreach (var r in _data)
            {
                if (r.Latitud.HasValue && r.Longitud.HasValue)
                {
                    double lat = (double)r.Latitud.Value;
                    double lon = (double)r.Longitud.Value;

                    double sx = width / 2 + (LonToX(lon, _zoom) * 256 - cx_pixel);
                    double sy = height / 2 + (LatToY(lat, _zoom) * 256 - cy_pixel);

                    string label = $"{r.Nombre}\n{r.Ciudad} ({r.CantidadVentas} ventas)";
                    DrawPin(context, sx, sy, label, ThemeHelper.Primary);
                }
            }
        }

        // Draw Selected Location Marker in Selection Mode
        if (SelectionMode)
        {
            double sx = width / 2 + (LonToX(SelectedLon, _zoom) * 256 - cx_pixel);
            double sy = height / 2 + (LatToY(SelectedLat, _zoom) * 256 - cy_pixel);
            DrawPin(context, sx, sy, "Ubicación seleccionada", Color.FromArgb(255, 255, 100, 0));
        }

        // Draw Instructions Banner
        var bannerBrush = new SolidColorBrush(Color.FromArgb(200, 33, 33, 33));
        context.FillRectangle(bannerBrush, new Rect(10, 10, width - 20, 35), 6f);

        string msg = SelectionMode 
            ? "Modo Selección: Haz clic en el mapa para marcar la sucursal y presiona Aceptar" 
            : "Mapa comercial: Arrastra para mover y usa la rueda del mouse para hacer zoom";

        var msgFt = new FormattedText(
            msg,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
            12,
            Brushes.White
        );
        context.DrawText(msgFt, new Point(20, 18));
    }

    private void DrawPin(DrawingContext context, double x, double y, string label, Color color)
    {
        var brush = new SolidColorBrush(color);
        var borderPen = new Pen(Brushes.White, 2);

        // Draw stem and pin head
        context.DrawLine(new Pen(brush, 3), new Point(x, y), new Point(x, y - 12));
        context.DrawEllipse(brush, borderPen, new Point(x, y - 16), 8, 8);

        if (!string.IsNullOrWhiteSpace(label))
        {
            var text = new FormattedText(
                label,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Bold),
                10,
                Brushes.Black
            );

            double tx = x - text.Width / 2;
            double ty = y - 36 - text.Height;
            context.FillRectangle(new SolidColorBrush(Color.FromArgb(210, 255, 255, 255)), new Rect(tx - 4, ty - 2, text.Width + 8, text.Height + 4), 4f);
            context.DrawRectangle(null, new Pen(new SolidColorBrush(color), 1), new Rect(tx - 4, ty - 2, text.Width + 8, text.Height + 4), 4.0, 4.0);
            context.DrawText(text, new Point(tx, ty));
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var properties = e.GetCurrentPoint(this).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _lastPointerPos = e.GetPosition(this);
            _isDragging = true;
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_isDragging)
        {
            var currentPos = e.GetPosition(this);
            double dx = currentPos.X - _lastPointerPos.X;
            double dy = currentPos.Y - _lastPointerPos.Y;

            double cx_pixel = LonToX(_centerLon, _zoom) * 256 - dx;
            double cy_pixel = LatToY(_centerLat, _zoom) * 256 - dy;

            _centerLon = XToLon(cx_pixel / 256.0, _zoom);
            _centerLat = YToLat(cy_pixel / 256.0, _zoom);

            _lastPointerPos = currentPos;
            InvalidateVisual();
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (SelectionMode)
        {
            var pos = e.GetPosition(this);
            double px = LonToX(_centerLon, _zoom) * 256 + (pos.X - Bounds.Width / 2);
            double py = LatToY(_centerLat, _zoom) * 256 + (pos.Y - Bounds.Height / 2);

            SelectedLat = YToLat(py / 256.0, _zoom);
            SelectedLon = XToLon(px / 256.0, _zoom);
            _isDragging = false;
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        if (_isDragging)
        {
            _isDragging = false;
            e.Handled = true;
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        double delta = e.Delta.Y;
        if (delta == 0) return;

        var mousePos = e.GetPosition(this);
        double mousePx = LonToX(_centerLon, _zoom) * 256 + (mousePos.X - Bounds.Width / 2);
        double mousePy = LatToY(_centerLat, _zoom) * 256 + (mousePos.Y - Bounds.Height / 2);

        double mouseLat = YToLat(mousePy / 256.0, _zoom);
        double mouseLon = XToLon(mousePx / 256.0, _zoom);

        int oldZoom = _zoom;
        if (delta > 0) _zoom = Math.Min(_zoom + 1, 18);
        else _zoom = Math.Max(_zoom - 1, 3);

        if (_zoom != oldZoom)
        {
            double newMousePx = LonToX(mouseLon, _zoom) * 256;
            double newMousePy = LatToY(mouseLat, _zoom) * 256;

            double newCx = newMousePx - (mousePos.X - Bounds.Width / 2);
            double newCy = newMousePy - (mousePos.Y - Bounds.Height / 2);

            _centerLon = XToLon(newCx / 256.0, _zoom);
            _centerLat = YToLat(newCy / 256.0, _zoom);

            InvalidateVisual();
        }
        e.Handled = true;
    }

    private Bitmap GetTile(int z, int x, int y)
    {
        string key = $"{z}_{x}_{y}";
        if (_tileCache.TryGetValue(key, out var bmp)) return bmp;

        if (!_downloadingTiles.Contains(key))
        {
            _downloadingTiles.Add(key);
            _ = DownloadTileAsync(z, x, y, key);
        }
        return null;
    }

    private async Task DownloadTileAsync(int z, int x, int y, string key)
    {
        try
        {
            string cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HuixinMotors", "MapCache");
            Directory.CreateDirectory(cacheDir);
            string cachePath = Path.Combine(cacheDir, $"{key}.png");

            if (File.Exists(cachePath))
            {
                try
                {
                    var bmp = new Bitmap(cachePath);
                    _tileCache[key] = bmp;
                    Dispatcher.UIThread.Post(() => InvalidateVisual());
                    return;
                }
                catch
                {
                    File.Delete(cachePath);
                }
            }

            string url = $"https://tile.openstreetmap.org/{z}/{x}/{y}.png";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("HuixinMotors/1.0 (contact@huixinmotors.com)");
            var bytes = await client.GetByteArrayAsync(url);

            await File.WriteAllBytesAsync(cachePath, bytes);

            using var ms = new MemoryStream(bytes);
            var downloadedBmp = new Bitmap(ms);
            _tileCache[key] = downloadedBmp;
            Dispatcher.UIThread.Post(() => InvalidateVisual());
        }
        catch
        {
            // Ignore fetch errors
        }
        finally
        {
            _downloadingTiles.Remove(key);
        }
    }

    // Web Mercator Math
    private static double LatToY(double lat, int zoom)
    {
        double latRad = lat * Math.PI / 180.0;
        return (1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * Math.Pow(2, zoom);
    }

    private static double LonToX(double lon, int zoom)
    {
        return (lon + 180.0) / 360.0 * Math.Pow(2, zoom);
    }

    private static double YToLat(double y, int zoom)
    {
        double n = Math.PI - 2.0 * Math.PI * y / Math.Pow(2, zoom);
        return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
    }

    private static double XToLon(double x, int zoom)
    {
        return x / Math.Pow(2, zoom) * 360.0 - 180.0;
    }
}
