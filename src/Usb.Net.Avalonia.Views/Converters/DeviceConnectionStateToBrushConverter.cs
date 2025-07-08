using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Usb.Net.Avalonia.Views.Converters;

internal sealed class DeviceConnectionStateToBrushConverter : IValueConverter
{
    private const string Attached = nameof(Attached);
    private const string Detached = nameof(Detached);

    /// <inheritdoc cref="IValueConverter.Convert(object?, Type, object?, CultureInfo)" />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string valueString)
        {
            return valueString.Contains(Attached, StringComparison.CurrentCulture) ? Brushes.ForestGreen : Brushes.Crimson;
        }

        return null;
    }

    /// <inheritdoc cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo)" />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
