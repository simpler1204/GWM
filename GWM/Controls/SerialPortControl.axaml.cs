using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using GWM.Models;

namespace GWM.Controls;

public partial class SerialPortControl : UserControl
{
    public static readonly StyledProperty<int> PortNumberProperty =
        AvaloniaProperty.Register<SerialPortControl, int>(nameof(PortNumber), 1);

    public int PortNumber
    {
        get => GetValue(PortNumberProperty);
        set => SetValue(PortNumberProperty, value);
    }
    
    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<SerialPortControl, bool>(nameof(HasError), false);

    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public SerialPortControl()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PortNumberProperty && PortLabel != null)
        {
            PortLabel.Text = $"P{change.GetNewValue<int>()}";
        }
        else if (change.Property == HasErrorProperty && change.GetNewValue<bool>() is var newState)
        {
           UpdateStatusBorder(newState);
        }
        
    }

    private void UpdateStatusBorder(bool newState)
    {
        if (newState)
            StatusBorder.Background = Brushes.Red;
        else
            StatusBorder.Background = SolidColorBrush.Parse("#CCCCCC");


    }
}