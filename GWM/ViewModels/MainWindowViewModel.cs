using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GWM.Models;
using Renci.SshNet;


namespace GWM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
   [ObservableProperty] private string _host = "192.168.4.127";

   [ObservableProperty] private string _username = "moxa";

   [ObservableProperty] private string _password = "moxa";

   [ObservableProperty] private bool _isConnected;

   [ObservableProperty] private string _statusMessage = "Ready";

   [ObservableProperty] private string _cpuUsageText = "0%";

   [ObservableProperty] private string _memoryUsageText = "0%";

   [ObservableProperty] private string _diskUsageText = "0%";

   // Port States for UI Binding
   [ObservableProperty] private PortState _lan1State;
   [ObservableProperty] private PortState _lan2State;
   [ObservableProperty] private PortState _lan3State;
    
   [ObservableProperty] private PortState _serial1State;
   [ObservableProperty] private PortState _serial2State;
   [ObservableProperty] private PortState _serial3State;
   [ObservableProperty] private PortState _serial4State;
   [ObservableProperty] private PortState _serial5State;
   [ObservableProperty] private PortState _serial6State;
   [ObservableProperty] private PortState _serial7State;
   [ObservableProperty] private PortState _serial8State;

   private SshClient? _sshClient;
   private bool _isMonitoring;
  
   
   [RelayCommand]
   private async Task ToggleConnectionAsync()
   {
       if (IsConnected)
       {
           await DisconnectAsync();
       }
       else
       {
           await ConnectAsync();
       }
   }

   private async Task ConnectAsync()
   {
       try
       {
           StatusMessage = "Connecting...";
            
           await Task.Run(() =>
           {
               _sshClient = new SshClient(Host, Username, Password);
               _sshClient.Connect();
           });

           if (_sshClient != null && _sshClient.IsConnected)
           {
               IsConnected = true;
               StatusMessage = "Connected";
               _isMonitoring = true;
               _ = MonitorDevice();
           }
       }
       catch (Exception ex)
       {
           StatusMessage = $"Connection failed: {ex.Message}";
           IsConnected = false;
       }
   }

   private async Task DisconnectAsync()
   {
       _isMonitoring = false;
       if (_sshClient != null)
       {
           await Task.Run(() =>
           {
               _sshClient.Disconnect();
               _sshClient.Dispose();
           });
           _sshClient = null;
       }
       IsConnected = false;
       StatusMessage = "Disconnected";
        
       // Reset states
       Lan1State = PortState.Idle;
       Lan2State = PortState.Idle;
       Lan3State = PortState.Idle;
       Serial1State = PortState.Idle;
       // ... reset others if needed
   }
   
    private async Task MonitorDevice()
    {
        while (_isMonitoring && _sshClient != null && _sshClient.IsConnected)
        {
            try
            {
                var status = await Task.Run(() => GetDeviceStatus());
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    CpuUsageText = $"{status.CpuUsage:F1}%";
                    MemoryUsageText = $"{status.MemoryUsage:F1}%";
                    DiskUsageText = $"{status.DiskUsage:F1}%";
                    
                    // Update Port States
                    Lan1State = status.Lan1State;
                    Lan2State = status.Lan2State;
                    Lan3State = status.Lan3State;
                    
                    // Serial states (mocked or from status)
                    Serial1State = status.Serial1State;
                    Serial2State = status.Serial2State;
                    Serial3State = status.Serial3State;
                    Serial4State = status.Serial4State;
                    Serial5State = status.Serial5State;
                    Serial6State = status.Serial6State;
                    Serial7State = status.Serial7State;
                    Serial8State = status.Serial8State;
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusMessage = $"Error reading status: {ex.Message}";
                });
            }

            await Task.Delay(1000);
        }
    }
    
    private DeviceStatus GetDeviceStatus()
    {
        if (_sshClient == null || !_sshClient.IsConnected) return new DeviceStatus();

        var info = new Dictionary<string, string>();
        
        // Note: Added checks for serial ports if possible, e.g., checking /proc/tty/driver/serial or similar
        // For now, we keep the existing command set and map LAN states
        string combinedCommand = 
            "echo 'ETH0_CARRIER:'; cat /sys/class/net/eth0/carrier 2>/dev/null || echo '0'; " +
            "echo 'ETH1_CARRIER:'; cat /sys/class/net/eth1/carrier 2>/dev/null || echo '0'; " +
            "echo 'ETH2_CARRIER:'; cat /sys/class/net/eth2/carrier 2>/dev/null || echo '0'; " +
            "echo 'ETH0_IP:'; ip -4 addr show eth0 | grep -oP '(?<=inet\\s)\\d+(\\.\\d+){3}' 2>/dev/null || echo 'N/A'; " +
            "echo 'ETH1_IP:'; ip -4 addr show eth1 | grep -oP '(?<=inet\\s)\\d+(\\.\\d+){3}' 2>/dev/null || echo 'N/A'; " +
            "echo 'ETH2_IP:'; ip -4 addr show eth2 | grep -oP '(?<=inet\\s)\\d+(\\.\\d+){3}' 2>/dev/null || echo 'N/A'; " +
            "echo 'CPU_USAGE:'; " +
            "grep 'cpu ' /proc/stat | awk '{print $2\" \"$3\" \"$4\" \"$5}' > /tmp/cpu1; " +
            "sleep 0.5; " + // Reduced sleep for faster UI updates
            "grep 'cpu ' /proc/stat | awk '{print $2\" \"$3\" \"$4\" \"$5}' > /tmp/cpu2; " +
            "awk 'NR==FNR{a1=$1;a2=$2;a3=$3;a4=$4;next} " +
            "{total=($1+$2+$3+$4)-(a1+a2+a3+a4); idle=$4-a4; used=total-idle; " +
            "if(total>0) printf \"%.1f\", (used/total)*100; else print \"0.0\"}' /tmp/cpu1 /tmp/cpu2; " +
            "echo ''; " +
            "echo 'MEM_USAGE:'; free | grep Mem | awk '{printf \"%.1f\", ($3/$2) * 100.0}'; echo ''; " +
            "echo 'DISK_USAGE:'; df -h / | tail -1 | awk '{print $5}' | sed 's/%//'; ";

        var combinedCmd = _sshClient.CreateCommand(combinedCommand);
        var combinedResult = combinedCmd.Execute();
        
        var lines = combinedResult.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string? currentKey = null;
                
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.EndsWith(":"))
            {
                currentKey = trimmedLine.TrimEnd(':').ToLower().Replace("_", "");
            }
            else if (currentKey != null && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                info[currentKey] = trimmedLine;
                currentKey = null;
            }
        }
        
        // Default values
        if (!info.ContainsKey("cpuusage")) info["cpuusage"] = "0";
        if (!info.ContainsKey("memusage")) info["memusage"] = "0";
        if (!info.ContainsKey("diskusage")) info["diskusage"] = "0";
        if (!info.ContainsKey("eth0carrier")) info["eth0carrier"] = "0";
        if (!info.ContainsKey("eth1carrier")) info["eth1carrier"] = "0";
        if (!info.ContainsKey("eth2carrier")) info["eth2carrier"] = "0";

        double.TryParse(info["cpuusage"], out double cpuUsage);
        double.TryParse(info["memusage"], out double memUsage);
        double.TryParse(info["diskusage"], out double diskUsage);
        
        // Carrier status: "1" is connected, "0" is disconnected
        bool isEth0Live = info["eth0carrier"] == "1";
        bool isEth1Live = info["eth1carrier"] == "1";
        bool isEth2Live = info["eth2carrier"] == "1";
        
        // Simulate some serial activity for demo purposes (randomly active if connected)
        // In real scenario, you would parse /proc/tty/driver/serial to check tx/rx counts
        var rnd = new Random();
        
        return new DeviceStatus
        {
            Timestamp = DateTime.Now,
            CpuUsage = cpuUsage,
            MemoryUsage = memUsage,
            DiskUsage = diskUsage,
            Eth0 = info.GetValueOrDefault("eth0ip", "N/A"),
            Eth1 = info.GetValueOrDefault("eth1ip", "N/A"),
            Eth2 = info.GetValueOrDefault("eth2ip", "N/A"),
            IsEth0live = isEth0Live,
            IsEth1live = isEth1Live,
            IsEth2live = isEth2Live,
            
            Lan1State = isEth0Live ? PortState.Connected : PortState.Idle,
            Lan2State = isEth1Live ? PortState.Connected : PortState.Idle,
            Lan3State = isEth2Live ? PortState.Connected : PortState.Idle,
            
            // Demo: Randomly set serial ports to active/connected
            Serial1State = PortState.Connected,
            Serial2State = rnd.Next(0, 10) > 8 ? PortState.Active : PortState.Connected,
            Serial3State = PortState.Idle,
            Serial4State = PortState.Idle,
            Serial5State = PortState.Idle,
            Serial6State = PortState.Idle,
            Serial7State = PortState.Idle,
            Serial8State = PortState.Idle
        };
    }
}