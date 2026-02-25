using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GWM.Models;

namespace GWM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _portName = "LAN";

    [ObservableProperty] 
    private PortState _portState = Models.PortState.Idle;
    
    [RelayCommand]
    private void ChangePortName()
    {
        PortName = "sids--";
    }
    

    [RelayCommand]
    private void ChangePortState()
    {
        PortState = PortState.Connected;
    }
    
}