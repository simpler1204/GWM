using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using GWM.Models;

namespace GWM.Controls;

public partial class LanPortControl : UserControl
{
    private CancellationTokenSource? _errorBlinkCts;
    private bool _isErrorBlinkOn;

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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        StopErrorBlink();
        base.OnDetachedFromVisualTree(e);
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

        if (newState != PortState.Error)
        {
            StopErrorBlink();
            _isErrorBlinkOn = false;
        }
       
        switch (newState)
        {
            case PortState.Idle:
                LinkLed.Fill = SolidColorBrush.Parse("#444");
               // ActiveLed.Fill = SolidColorBrush.Parse("#444");
                break;
            case PortState.Connected:
                LinkLed.Fill = Brushes.LightGreen;
                //ActiveLed.Fill = SolidColorBrush.Parse("#444");
                break;
            case PortState.Active:
                LinkLed.Fill = Brushes.Green;
               // ActiveLed.Fill = Brushes.Orange;
                break;
            case PortState.Error:
              //  LinkLed.Fill = Brushes.LightGreen;
                ActiveLed.Fill = Brushes.Red;
                _isErrorBlinkOn = true;
                StartErrorBlink();
                break;
        }
    }

    private void StartErrorBlink()
    {
        StopErrorBlink();
        _errorBlinkCts = new CancellationTokenSource();
        _ = BlinkErrorAsync(_errorBlinkCts.Token);
    }

    private void StopErrorBlink()
    {
        if (_errorBlinkCts is null)
        {
            return;
        }

        _errorBlinkCts.Cancel();
        _errorBlinkCts.Dispose();
        _errorBlinkCts = null;
    }

    private async Task BlinkErrorAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // UI 스레드를 블로킹하지 않도록 ConfigureAwait(false)
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            if (cancellationToken.IsCancellationRequested) break;

            // UI 업데이트는 반드시 UI 스레드에서 실행
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (ActiveLed == null || cancellationToken.IsCancellationRequested) return;
                _isErrorBlinkOn = !_isErrorBlinkOn;
                ActiveLed.Fill = _isErrorBlinkOn ? Brushes.Red : SolidColorBrush.Parse("#444");
            });
        }
    }
}