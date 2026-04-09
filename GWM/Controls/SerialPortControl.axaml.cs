using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using GWM.Models;

namespace GWM.Controls;

public partial class SerialPortControl : UserControl
{
    private static readonly IBrush NormalBrush = SolidColorBrush.Parse("#CCCCCC");
    private readonly DispatcherTimer _blinkTimer;
    private bool _isBlinkOn;

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

        _blinkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _blinkTimer.Tick += (_, _) =>
        {
            _isBlinkOn = !_isBlinkOn;
            StatusBorder.Background = _isBlinkOn ? Brushes.Red : NormalBrush;
        };
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
        {
            if (!_blinkTimer.IsEnabled)
            {
                _isBlinkOn = false;
                _blinkTimer.Start();
            }
        }
        else
        {
            _blinkTimer.Stop();
            _isBlinkOn = false;
            StatusBorder.Background = NormalBrush;
        }

    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _blinkTimer.Stop();
        base.OnDetachedFromVisualTree(e);

    }
}