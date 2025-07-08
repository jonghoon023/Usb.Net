using Avalonia;
using System;
using System.Windows.Input;
using AvaloniaControls = Avalonia.Controls;

namespace Usb.Net.Avalonia.Views.Controls;

/// <summary>
/// A custom UI class that inherits from <see cref="AvaloniaControls.ComboBox" />.
/// </summary>
internal sealed class ComboBox : AvaloniaControls.ComboBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComboBox" /> class.
    /// </summary>
    public ComboBox()
    {
        SelectionChanged += OnSelectionChanged;
    }

    /// <inheritdoc cref="StyledElement.StyleKeyOverride" />
    protected override Type StyleKeyOverride => typeof(AvaloniaControls.ComboBox);

    /// <summary>
    /// Identifies the <see cref="SelectionChangedCommand" /> dependency property. <br />
    /// This command is executed when the selection is changed.
    /// </summary>
    public static readonly StyledProperty<ICommand?> SelectionChangedCommandProperty = AvaloniaProperty.Register<ComboBox, ICommand?>(nameof(SelectionChangedCommand), null);

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> that is executed when the selection is changed.
    /// </summary>
    public ICommand? SelectionChangedCommand
    {
        get => GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    private void OnSelectionChanged(object? sender, AvaloniaControls.SelectionChangedEventArgs e)
    {
        if (SelectionChangedCommand is ICommand command)
        {
            object? commandParameter = SelectionMode switch
            {
                AvaloniaControls.SelectionMode.Single => SelectedItem,
                AvaloniaControls.SelectionMode.Multiple => SelectedItems,
                AvaloniaControls.SelectionMode.Toggle => null,
                AvaloniaControls.SelectionMode.AlwaysSelected => null,
                _ => null
            };

            command.Execute(commandParameter);
        }
    }
}
