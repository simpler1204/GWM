using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using GWM.Models;

namespace GWM.Controls;

public partial class LanPortControl : UserControl
{
    private static readonly IBrush NormalBrush = SolidColorBrush.Parse("#CCCCCC");
    private readonly DispatcherTimer _errorBlinkTimer;
    private bool _isBlinkOn;

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

    public static readonly StyledProperty<bool> HasErrorProperty
        = AvaloniaProperty.Register<LanPortControl, bool>(nameof(HasError), false);

    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public LanPortControl()
    {
        InitializeComponent();

        _errorBlinkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _errorBlinkTimer.Tick += (_, _) =>
        {
            _isBlinkOn = !_isBlinkOn;
            StatusBorder.Background = _isBlinkOn ? Brushes.Red : NormalBrush;
        };
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
        
        else if (change.Property == HasErrorProperty && change.GetNewValue<bool>() is var newHasError)
        {
            UpdateStatusBorder(newHasError);
        }
    }

    private void UpdateStatusBorder(bool newHasError)
    {
        if (newHasError)
        {
            if (!_errorBlinkTimer.IsEnabled)
            {
                _isBlinkOn = false;
                _errorBlinkTimer.Start();
            }
            return;
        }

        _errorBlinkTimer.Stop();
        _isBlinkOn = false;
        StatusBorder.Background = NormalBrush;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _errorBlinkTimer.Stop();
        base.OnDetachedFromVisualTree(e);
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
                LinkLed.Fill = Brushes.LightGreen;
                ActiveLed.Fill = SolidColorBrush.Parse("#444");
                break;
            case PortState.Active:
                LinkLed.Fill = Brushes.Green;
                ActiveLed.Fill = Brushes.Orange;
                break;
            case PortState.Error:
                LinkLed.Fill = Brushes.LightGreen;
                ActiveLed.Fill = Brushes.Red;
                break;
        }
    }

   
}