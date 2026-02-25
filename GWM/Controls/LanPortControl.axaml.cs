using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using GWM.Models;

namespace GWM.Controls;

public partial class LanPortControl : UserControl
{
    public static readonly StyledProperty<string> PortNameProperty
        = AvaloniaProperty.Register<LanPortControl, string>(nameof(PortName), "LAN");

    public string PortName
    {
        get => GetValue(PortNameProperty);
        set => SetValue(PortNameProperty, value);
    }

    public static readonly StyledProperty<PortState> StateProperty
        = AvaloniaProperty.Register<LanPortControl, PortState>(nameof(PortState), PortState.Idle);
    
    public PortState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public LanPortControl()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PortNameProperty && PortLabel != null)
        {
            PortLabel.Text = change.GetNewValue<string>();
        }
        
        if(change.Property == StateProperty && change.GetNewValue<PortState>() is var newState)
        {
            UpdateLeds(newState);
        }
    }

    private void UpdateLeds(PortState newState)
    {
        if (LinkLed == null || ActiveLed == null) return;
       
        switch (newState)
        {
            case PortState.Idle:
                LinkLed.Fill = SolidColorBrush.Parse("#444");
                ActiveLed.Fill = SolidColorBrush.Parse("#444");
                break;
            case PortState.Connected:
                LinkLed.Fill = Brushes.Green;
                ActiveLed.Fill = SolidColorBrush.Parse("#444");
                break;
            case PortState.Active:
                LinkLed.Fill = Brushes.Green;
                ActiveLed.Fill = Brushes.Orange;
                break;
            case PortState.Error:
                LinkLed.Fill = Brushes.Red;
                ActiveLed.Fill = Brushes.Red;
                break;
        }
    }
}