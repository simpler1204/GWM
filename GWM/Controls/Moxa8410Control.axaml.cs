using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GWM.Models;

namespace GWM.Controls;

public partial class Moxa8410Control : UserControl
{
    //Connected
    public static readonly StyledProperty<bool> IsConnectedProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(IsConnected), false);   
    public bool IsConnected
    {
        get => GetValue(IsConnectedProperty);
        set => SetValue(IsConnectedProperty, value);
    }
    
    
    //Lan Ports
    public static readonly StyledProperty<string> Lan1NameProperty =
        AvaloniaProperty.Register<Moxa8410Control, string>(nameof(Lan1Name), "LAN 1");
    
    public string Lan1Name
    {
        get => GetValue(Lan1NameProperty);
        set => SetValue(Lan1NameProperty, value);
    }
    
    public static readonly StyledProperty<string> Lan2NameProperty =
        AvaloniaProperty.Register<Moxa8410Control, string>(nameof(Lan2Name), "LAN 2");

    public string Lan2Name
    {
        get=> GetValue(Lan2NameProperty);
        set => SetValue(Lan2NameProperty, value);
    }
    
    public static readonly StyledProperty<string> Lan3NameProperty =
        AvaloniaProperty.Register<Moxa8410Control, string>(nameof(Lan3Name), "LAN 3");
    
    public string Lan3Name
    {
        get => GetValue(Lan3NameProperty);
        set => SetValue(Lan3NameProperty, value);
    }

    private static readonly StyledProperty<bool> Lan1ErrorProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Lan1Error), false);

    public bool Lan1Error
    {
        get => GetValue(Lan1ErrorProperty);
        set => SetValue(Lan1ErrorProperty, value);
    }
    
    private static readonly StyledProperty<bool> Lan2ErrorProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Lan2Error), false);

    public bool Lan2Error
    {
        get => GetValue(Lan2ErrorProperty);
        set => SetValue(Lan2ErrorProperty, value);
    }
    
    private static readonly StyledProperty<bool> Lan3ErrorProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Lan3Error), false);

    public bool Lan3Error
    {
        get => GetValue(Lan3ErrorProperty);
        set => SetValue(Lan3ErrorProperty, value);
    }
    

    
    // Serial 1 Ports
    public static readonly StyledProperty<bool> Serial1StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial1State), false);
    
    public bool Serial1State
    {
        get => GetValue(Serial1StateProperty);
        set => SetValue(Serial1StateProperty, value);
    }
    
    // Serial 2 Ports
    public static readonly StyledProperty<bool> Serial2StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial2State), false);
    public bool Serial2State
    {
        get => GetValue(Serial2StateProperty);
        set => SetValue(Serial2StateProperty, value);
    }
    
    //Serial 3 Ports
    public static readonly StyledProperty<bool> Serial3StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial3State), false);
    public bool Serial3State
    {
        get => GetValue(Serial3StateProperty);
        set => SetValue(Serial3StateProperty, value);
    }
    
    // Serial 4 Ports
    public static readonly StyledProperty<bool> Serial4StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial4State), false);
    public bool Serial4State
    {
        get => GetValue(Serial4StateProperty);
        set => SetValue(Serial4StateProperty, value);
    }
    
    // Serial 5 Ports
    public static readonly StyledProperty<bool> Serial5StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial5State), false);
    public bool Serial5State
    {
        get => GetValue(Serial5StateProperty);
        set => SetValue(Serial5StateProperty, value);
    }
    
    // Serial 6 Ports
    public static readonly StyledProperty<bool> Serial6StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial6State), false);
    public bool Serial6State
    {
        get => GetValue(Serial6StateProperty);
        set => SetValue(Serial6StateProperty, value);
    }
    
    // Serial 7 Ports
    public static readonly StyledProperty<bool> Serial7StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial7State), false);    
    public bool Serial7State
    {
        get => GetValue(Serial7StateProperty);
        set => SetValue(Serial7StateProperty, value);
    }
    
    // Serial 8 Ports
    public static readonly StyledProperty<bool> Serial8StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Serial8State), false);
    public bool Serial8State
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
    
    public static readonly StyledProperty<bool> Lan1HasErrorProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Lan1HasError), false);
    
    public bool Lan1HasError
    {
        get => GetValue(Lan1ErrorProperty);
        set => SetValue(Lan1ErrorProperty, value);
    }
    
    
    // LAN 2 Ports
    public static readonly StyledProperty<PortState> Lan2StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Lan2State), PortState.Idle);
    public PortState Lan2State
    {
        get => GetValue(Lan2StateProperty);
        set => SetValue(Lan2StateProperty, value);
    }
    
    public static readonly StyledProperty<bool> Lan2HasErrorProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Lan2HasError), false);
    
    public bool Lan2HasError
    {
        get => GetValue(Lan2ErrorProperty);
        set => SetValue(Lan2ErrorProperty, value);
    }
    
    
    // LAN 3 Ports
    public static readonly StyledProperty<PortState> Lan3StateProperty =
        AvaloniaProperty.Register<Moxa8410Control, PortState>(nameof(Lan3State), PortState.Idle);
    public PortState Lan3State
    {
        get => GetValue(Lan3StateProperty);
        set => SetValue(Lan3StateProperty, value);
    }
    
    public static readonly StyledProperty<bool> Lan3HasErrorProperty =
        AvaloniaProperty.Register<Moxa8410Control, bool>(nameof(Lan3HasError), false);
    
    public bool Lan3HasError
    {
        get => GetValue(Lan3ErrorProperty);
        set => SetValue(Lan3ErrorProperty, value);
    }
    
    public Moxa8410Control()
    {
        InitializeComponent();
    }
}