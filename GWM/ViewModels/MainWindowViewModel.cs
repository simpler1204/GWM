using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GWM.Dto;
using GWM.Models;
using Newtonsoft.Json;
using Renci.SshNet;


namespace GWM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private const int SocketServerPort = 3131;

    [ObservableProperty] private string _host = "192.168.5.127";
    [ObservableProperty] private string _username = "root";
    [ObservableProperty] private string _password = "sids";
    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private string _cpuUsageText = "0%";
    [ObservableProperty] private string _memoryUsageText = "0%";
    [ObservableProperty] private string _diskUsageText = "0%";
    [ObservableProperty] private string _eth0Ip = "N/A";
    [ObservableProperty] private string _eth1Ip = "N/A";
    [ObservableProperty] private string _eth2Ip = "N/A";

    // Port States for UI Binding
    [ObservableProperty] private PortState _lan1State;
    [ObservableProperty] private PortState _lan2State;
    [ObservableProperty] private PortState _lan3State;
    
    [ObservableProperty] private bool _lan1HasError;
    [ObservableProperty] private bool _lan2HasError;
    [ObservableProperty] private bool _lan3HasError;

    [ObservableProperty] private bool _serial1State;
    [ObservableProperty] private bool _serial2State;
    [ObservableProperty] private bool _serial3State;
    [ObservableProperty] private bool _serial4State;
    [ObservableProperty] private bool _serial5State;
    [ObservableProperty] private bool _serial6State;
    [ObservableProperty] private bool _serial7State;
    [ObservableProperty] private bool _serial8State;
    
// 기존 SerialStates 프로퍼티 제거 후 추가
    private bool this[int index]
    {
        get => index switch
        {
            0 => Serial1State,
            1 => Serial2State,
            2 => Serial3State,
            3 => Serial4State,
            4 => Serial5State,
            5 => Serial6State,
            6 => Serial7State,
            7 => Serial8State,
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Serial index must be 0..7.")
        };
        set
        {
            switch (index)
            {
                case 0: Serial1State = value; break;
                case 1: Serial2State = value; break;
                case 2: Serial3State = value; break;
                case 3: Serial4State = value; break;
                case 4: Serial5State = value; break;
                case 5: Serial6State = value; break;
                case 6: Serial7State = value; break;
                case 7: Serial8State = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), "Serial index must be 0..7.");
            }
        }
    }

    private SshClient? _sshClient;
    private bool _isMonitoring;
    private readonly SemaphoreSlim _disconnectLock = new(1, 1);
    private TcpClient? _tcpClient = null;
    private AllErrorsResponse? _allErrorsResponse = null;
    

    [RelayCommand]
    private async Task Port1Clicked()
    {
        //Serial2State = true;
        //SerialStates[1] = true;
        await GetAllErrorList();
        
    }


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
                _sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(3);
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
            StatusMessage = $"{ex.Message}";
            IsConnected = false;
        }
    }

    public async Task DisconnectOnExitAsync()
    {
        try
        {
            await DisconnectAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DisconnectOnExitAsync failed: {ex.Message}");
        }
    }

    private async Task DisconnectAsync()
    {
        await _disconnectLock.WaitAsync();
        try
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

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
                _tcpClient = null;
                Debug.WriteLine("TCP connection closed");
            }

            Eth0Ip = "N/A";
            Eth1Ip = "N/A";
            Eth2Ip = "N/A";

            Lan1State = PortState.Idle;
            Lan2State = PortState.Idle;
            Lan3State = PortState.Idle;

            Serial1State = false;

            CpuUsageText = "0%";
            MemoryUsageText = "0%";
            DiskUsageText = "0%";
        }
        finally
        {
            _disconnectLock.Release();
        }
    }

    private async Task MonitorDevice()
    {
        while (_isMonitoring && _sshClient != null && _sshClient.IsConnected)
        {
            try
            {
                var status = await Task.Run(GetDeviceStatus);

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    CpuUsageText = $"{status.CpuUsage:F1}%";
                    // Debug.WriteLine($"{status.CpuUsage:F1}%");
                    MemoryUsageText = $"{status.MemoryUsage:F1}%";
                    DiskUsageText = $"{status.DiskUsage:F1}%";

                    // Update Port States 
                    Eth0Ip = status.Eth0;
                    Eth1Ip = status.Eth1;
                    Eth2Ip = status.Eth2;

                    if (Lan1State != status.Lan1State)
                        Lan1State = status.Lan1State;
                    if (Lan2State != status.Lan2State)
                        Lan2State = status.Lan2State;
                    if (Lan3State != status.Lan3State)
                        Lan3State = status.Lan3State;

                    IsConnected = true;
                });
            }
            catch (Exception ex)
            {
                // Debug.WriteLine("failed to read status: " + ex.Message);
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusMessage = $"Error reading status: {ex.Message}";
                    IsConnected = false;
                });
                await DisconnectAsync();
            }

            await GetAllErrorList();

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
            "sleep 1; " + // 1초로 증가하여 더 정확한 측정
            "grep 'cpu ' /proc/stat | awk '{print $2\" \"$3\" \"$4\" \"$5}' > /tmp/cpu2; " +
            "awk 'NR==FNR{a1=$1;a2=$2;a3=$3;a4=$4;next} " +
            "{total=($1+$2+$3+$4)-(a1+a2+a3+a4); idle=$4-a4; used=total-idle; " +
            "if(total>0 && used>=0) printf \"%.1f\", (used/total)*100; else print \"0.0\"}' /tmp/cpu1 /tmp/cpu2; " +
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
        if (!info.ContainsKey("eth0ip")) info["eth0ip"] = "N/A";
        if (!info.ContainsKey("eth1ip")) info["eth1ip"] = "N/A";
        if (!info.ContainsKey("eth2ip")) info["eth2ip"] = "N/A";

        double.TryParse(info["cpuusage"], out double cpuUsage);
        double.TryParse(info["memusage"], out double memUsage);
        double.TryParse(info["diskusage"], out double diskUsage);

        // Carrier status: "1" is connected, "0" is disconnected
        bool isEth0Live = info["eth0carrier"] == "1";
        bool isEth1Live = info["eth1carrier"] == "1";
        bool isEth2Live = info["eth2carrier"] == "1";

        // Simulate some serial activity for demo purposes (randomly active if connected)
        // In real scenario, you would parse /proc/tty/driver/serial to check tx/rx counts


        return new DeviceStatus
        {
            Timestamp = DateTime.Now,
            CpuUsage = cpuUsage,
            MemoryUsage = memUsage,
            DiskUsage = diskUsage,
            Eth0 = info.GetValueOrDefault("eth0ip", "N/A"),
            Eth1 = info.GetValueOrDefault("eth1ip", "N/A"),
            Eth2 = info.GetValueOrDefault("eth2ip", "N/A"),

            IsEth0Live = isEth0Live,
            IsEth1Live = isEth1Live,
            IsEth2Live = isEth2Live,

            Lan1State = isEth0Live ? PortState.Connected : PortState.Idle,
            Lan2State = isEth1Live ? PortState.Connected : PortState.Idle,
            Lan3State = isEth2Live ? PortState.Connected : PortState.Idle,
        };
    }

    private async Task<List<string>?> GetErrorList()
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(Host, SocketServerPort);

            await using var stream = client.GetStream();

            var requestBytes = Encoding.UTF8.GetBytes("errorList\n");
            await stream.WriteAsync(requestBytes.AsMemory(0, requestBytes.Length));
            await stream.FlushAsync();

            using var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true);
            var response = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(response))
            {
                Debug.WriteLine("서버 응답이 비어있습니다.");
                return null;
            }

            Debug.WriteLine($"Received response: {response}");

            var deviceList = JsonConvert.DeserializeObject<List<string>>(response);

            if (deviceList == null || deviceList.Count == 0)
            {
                Debug.WriteLine("연결 불가 디바이스가 없습니다.");
                return null;
            }

            foreach (var device in deviceList)
            {
                Debug.WriteLine($"- {device}");
            }

            return deviceList;
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"JsonException: {ex.Message}");
        }
        catch (SocketException ex)
        {
            Debug.WriteLine($"SocketException: {ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"IOException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");
        }

        return null;
    }
    
    private async Task GetAllErrorList()
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(Host, SocketServerPort);

            await using var stream = client.GetStream();

            var requestBytes = Encoding.UTF8.GetBytes("allErrors\n");
            await stream.WriteAsync(requestBytes.AsMemory(0, requestBytes.Length));
            await stream.FlushAsync();

            using var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true);
            var response = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(response))
            {
                Debug.WriteLine("서버 응답이 비어있습니다.");
                _allErrorsResponse = null;
                return ;
            }

            Debug.WriteLine($"Received response: {response}");

            var result = JsonConvert.DeserializeObject<AllErrorsResponse>(response);
            _allErrorsResponse = result;

            await ShowErrorMark(result);

            foreach (var ip in result?.CannotConnectDevices!)
                Debug.WriteLine($"연결불가: {ip}");

            foreach (var err in result.TcpConfigErrors)
                Debug.WriteLine($"TCP설정오류: {err.Ip} [{err.GwStart}~{err.GwEnd}]");

            foreach (var err in result.SerialErrors)
                Debug.WriteLine($"시리얼오류: Port{err.PortNumber} [{err.GwStart}~{err.GwEnd}]");
            
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"JsonException: {ex.Message}");
        }
        catch (SocketException ex)
        {
            Debug.WriteLine($"SocketException: {ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"IOException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");
        }
        
    }
    
    private async Task ShowErrorMark(AllErrorsResponse? errorResponse)
    {
       
        if (errorResponse != null && (errorResponse.CannotConnectDevices.Count > 0 || errorResponse.CannotConnectDevices.Count > 0))
        {
            if (Lan1State == PortState.Connected)
                Lan1HasError = true;
            if (Lan2State == PortState.Connected)
                Lan2HasError = true;
            if (Lan3State == PortState.Connected)
                Lan3HasError = true;
        }
        else
        {
            if(Lan1HasError == true)
                Lan1HasError = false;
            if(Lan2HasError == true)
                Lan2HasError = false;
            if(Lan3HasError == true)
                Lan3HasError = false;
        }


        for (int i = 0; i < 8; i++)
        {
            this[i] = false;
        }
        
        if(errorResponse != null && errorResponse.SerialErrors.Count > 0)
        {
            foreach (var v in errorResponse.SerialErrors)
            {
                Debug.WriteLine("afasfafada" + v.PortNumber);
                
                this[v.PortNumber - 1] = true;
            }
        }
        
    }

}