using Avalonia;
using System;
using System.Windows.Input;
using AvaloniaControls = Avalonia.Controls;

namespace Usb.Net.Avalonia.Views.Controls;

/// <summary>
/// A custom UI class that inherits from <see cref="AvaloniaControls.TextBox" />.
/// </summary>
internal sealed class TextBox : AvaloniaControls.TextBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBox" /> class.
    /// </summary>
    public TextBox()
    {
        TextChanged += OnTextChanged;
    }

    /// <inheritdoc cref="StyledElement.StyleKeyOverride" />
    protected override Type StyleKeyOverride => typeof(AvaloniaControls.TextBox);

    /// <summary>
    /// Identifies the <see cref="TextChangedCommand"/> styled property. <br />
    /// This command is executed when text input occurs via the keyboard.
    /// </summary>
    public static readonly StyledProperty<ICommand?> TextChangedCommandProperty = AvaloniaProperty.Register<ComboBox, ICommand?>(nameof(TextChangedCommand), null);

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> that is executed when text input occurs.
    /// </summary>
    public ICommand? TextChangedCommand
    {
        get => GetValue(TextChangedCommandProperty);
        set => SetValue(TextChangedCommandProperty, value);
    }

    private void OnTextChanged(object? sender, AvaloniaControls.TextChangedEventArgs e)
    {
        if (TextChangedCommand is ICommand command)
        {
            command.Execute(Text);
        }
    }
}
