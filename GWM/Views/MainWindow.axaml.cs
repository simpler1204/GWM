using System;
using System.Diagnostics;
using Avalonia.Controls;
using GWM.ViewModels;

namespace GWM.Views;

public partial class MainWindow : Window
{
    private bool _isClosingHandled;

    public MainWindow()
    {
        InitializeComponent();
        Closing += OnMainWindowClosing;
    }

    private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_isClosingHandled)
        {
            return;
        }

        _isClosingHandled = true;

        if (DataContext is MainWindowViewModel vm)
        {
            try
            {
                // 종료 전에 연결 정리를 완료해 리소스 누수를 방지
                vm.DisconnectOnExitAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disconnect on close failed: {ex.Message}");
            }
        }
    }
}