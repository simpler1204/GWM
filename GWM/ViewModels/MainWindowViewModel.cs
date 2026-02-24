using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GWM.Models;

namespace GWM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _portName2 = "LAN";

    [ObservableProperty] 
    private PortState _portState2 = Models.PortState.Idle;
    
    [RelayCommand]
    private void ChangePortName()
    {
        PortName2 = "sids--";
    }
    

    [RelayCommand]
    private void ChangePortState()
    {
        PortState2 = PortState.Connected;
    }
    
}