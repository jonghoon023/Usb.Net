using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Usb.Net.Avalonia.Views.Converters;

/// <summary>
/// A value converter that inverts a <see cref="bool" /> value.
/// </summary>
internal sealed class BoolInvertConverter : IValueConverter
{
    /// <inheritdoc cref="IValueConverter.Convert(object?, Type, object?, CultureInfo)" />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        throw new NotSupportedException($"{nameof(value)} must be of type {nameof(Boolean)}.");
    }

    /// <inheritdoc cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo)" />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, CultureInfo.CurrentCulture);
    }
}
