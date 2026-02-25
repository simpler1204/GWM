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
    
    public static readonly StyledProperty<PortState> StateProperty =
        AvaloniaProperty.Register<SerialPortControl, PortState>(nameof(State), PortState.Idle);

    public PortState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
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
        else if (change.Property == StateProperty && change.GetNewValue<PortState>() is var newState)
        {
            UpdateLeds(newState);
        }
        
    }

    private void UpdateLeds(PortState newState)
    {
        if(TxLed == null || RxLed == null) return;
        switch (newState)
        {
            case PortState.Idle:
                TxLed.Fill = SolidColorBrush.Parse("#444");
                RxLed.Fill = SolidColorBrush.Parse("#444");
                break;
            case PortState.Connected:
                TxLed.Fill = Brushes.Green;
                RxLed.Fill = Brushes.Green;
                break;
            case PortState.Active:
                TxLed.Fill = Brushes.Orange;
                RxLed.Fill = Brushes.Orange;
                break;
            case PortState.Error:
                TxLed.Fill = Brushes.Red;
                RxLed.Fill = Brushes.Red;
                break;
        }
    }
}