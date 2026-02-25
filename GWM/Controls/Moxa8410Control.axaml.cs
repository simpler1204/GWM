using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GWM.Models;

namespace GWM.Controls;

public partial class Moxa8410Control : UserControl
{
    // Serial 1 Ports
    public static readonly StyledProperty<PortState> Serial1StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial1State), PortState.Idle);
    public PortState Serial1State
    {
        get => GetValue(Serial1StateProperty);
        set => SetValue(Serial1StateProperty, value);
    }
    
    // Serial 2 Ports
    public static readonly StyledProperty<PortState> Serial2StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial2State), PortState.Idle);
    public PortState Serial2State
    {
        get => GetValue(Serial2StateProperty);
        set => SetValue(Serial2StateProperty, value);
    }
    
    //Serial 3 Ports
    public static readonly StyledProperty<PortState> Serial3StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial3State), PortState.Idle);
    public PortState Serial3State
    {
        get => GetValue(Serial3StateProperty);
        set => SetValue(Serial3StateProperty, value);
    }
    
    // Serial 4 Ports
    public static readonly StyledProperty<PortState> Serial4StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial4State), PortState.Idle);
    public PortState Serial4State
    {
        get => GetValue(Serial4StateProperty);
        set => SetValue(Serial4StateProperty, value);
    }
    
    // Serial 5 Ports
    public static readonly StyledProperty<PortState> Serial5StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial5State), PortState.Idle);
    public PortState Serial5State
    {
        get => GetValue(Serial5StateProperty);
        set => SetValue(Serial5StateProperty, value);
    }
    
    // Serial 6 Ports
    public static readonly StyledProperty<PortState> Serial6StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial6State), PortState.Idle);
    public PortState Serial6State
    {
        get => GetValue(Serial6StateProperty);
        set => SetValue(Serial6StateProperty, value);
    }
    
    // Serial 7 Ports
    public static readonly StyledProperty<PortState> Serial7StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial7State), PortState.Idle);    
    public PortState Serial7State
    {
        get => GetValue(Serial7StateProperty);
        set => SetValue(Serial7StateProperty, value);
    }
    
    // Serial 8 Ports
    public static readonly StyledProperty<PortState> Serial8StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Serial8State), PortState.Idle);
    public PortState Serial8State
    {
        get => GetValue(Serial8StateProperty);
        set => SetValue(Serial8StateProperty, value);
    }
    
    
    // LAN 1 Ports
    public static readonly StyledProperty<PortState> Lan1StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Lan1State), PortState.Idle);
    public PortState Lan1State
    {
        get => GetValue(Lan1StateProperty);
        set => SetValue(Lan1StateProperty, value);
    }
    
    // LAN 2 Ports
    public static readonly StyledProperty<PortState> Lan2StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Lan2State), PortState.Idle);
    public PortState Lan2State
    {
        get => GetValue(Lan2StateProperty);
        set => SetValue(Lan2StateProperty, value);
    }
    
    // LAN 3 Ports
    public static readonly StyledProperty<PortState> Lan3StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Lan3State), PortState.Idle);
    public PortState Lan3State
    {
        get => GetValue(Lan3StateProperty);
        set => SetValue(Lan3StateProperty, value);
    }
    
    public Moxa8410Control()
    {
        InitializeComponent();
    }
}