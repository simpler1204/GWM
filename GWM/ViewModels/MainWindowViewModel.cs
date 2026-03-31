using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GWM.Models;
using GWM.Utilities;
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

    [ObservableProperty] private bool _serial1State;
    [ObservableProperty] private bool _serial2State;
    [ObservableProperty] private bool _serial3State;
    [ObservableProperty] private bool _serial4State;
    [ObservableProperty] private bool _serial5State;
    [ObservableProperty] private bool _serial6State;
    [ObservableProperty] private bool _serial7State;
    [ObservableProperty] private bool _serial8State;

    private SshClient? _sshClient;
    private bool _isMonitoring;

    [RelayCommand]
    public async Task Port1Clicked()
    {
        await GetSerialErrorList();
    }


    [RelayCommand]
    public async Task ToggleConnectionAsync()
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
                // _ = GetErrorContent();
                 
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"{ex.Message}";
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
                   // Debug.WriteLine($"{status.CpuUsage:F1}%");
                    MemoryUsageText = $"{status.MemoryUsage:F1}%";
                    DiskUsageText = $"{status.DiskUsage:F1}%";

                    // Update Port States 
                    Eth0Ip = status.Eth0;
                    Eth1Ip = status.Eth1;
                    Eth2Ip = status.Eth2;
                    
                    if(Lan1State != status.Lan1State)
                        Lan1State = status.Lan1State;
                    if(Lan2State != status.Lan2State)
                        Lan2State = status.Lan2State;
                    if(Lan3State != status.Lan3State)
                        Lan3State = status.Lan3State;


                    // Serial states (mocked or from status)
                    // Serial1State = status.Serial1State;
                    // Serial2State = status.Serial2State;
                    // Serial3State = status.Serial3State;
                    // Serial4State = status.Serial4State;
                    // Serial5State = status.Serial5State;
                    // Serial6State = status.Serial6State;
                    // Serial7State = status.Serial7State;
                    // Serial8State = status.Serial8State;
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
            await Task.Delay(1000);
        }
    }
    
    private async Task GetErrorContent()
    {
        while (_isMonitoring && _sshClient != null && _sshClient.IsConnected)
        {
            try
            {
                await GetCannotConnectDevice();
            }
            catch (Exception ex)
            {
               
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

    [RelayCommand]
    private async Task<List<string>?> GetCannotConnectDevice()
    {
        try
        {
            //const string message = "tcpAddress";
            const string message = "cannotConnectDevice";
            //const string message = "serialErrorList";

            using var client = new TcpClient();

            // 비동기로 연결 시도
            try
            {
                await Task.Run(() => client.Connect(Host, SocketServerPort));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Client not connected");
            }

            // 연결 상태 확인
            if (!client.Connected)
            {
                throw new InvalidOperationException("Client not connected");
            }

            using var stream = client.GetStream();

            // 메시지 전송
            var sendBuffer = System.Text.Encoding.UTF8.GetBytes(message + "\n");

            try
            {
                await stream.WriteAsync(sendBuffer.AsMemory(0, sendBuffer.Length));
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                throw new SocketException((int)SocketError.ConnectionReset);
            }

            var response = ReadJsonArrayResponse(stream, message);
            var cannotConnectList = ResponseParser.ParseCannotConnectDeviceList(response);
            
            if (cannotConnectList is not null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if(Lan1State == PortState.Connected)
                        Lan1State = PortState.Error;
                    if(Lan2State == PortState.Connected)
                        Lan2State = PortState.Error;
                    if(Lan3State == PortState.Connected)
                        Lan3State = PortState.Error;
                });
                
                return cannotConnectList;
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if(Lan1State == PortState.Error)
                        Lan1State = PortState.Connected;
                    if(Lan2State == PortState.Error)
                        Lan2State = PortState.Connected;
                    if(Lan3State == PortState.Error)
                        Lan3State = PortState.Connected;
                });
            }
        }
        catch (Exception ex)
        {
           // await Dispatcher.UIThread.InvokeAsync(() => { StatusMessage = $"❌ 오류: {ex.Message}"; });
        }

        return null;
    }
    
     private async Task<List<string>?> GetSerialErrorList()
    {
        try
        {
            //const string message = "tcpAddress";
            //const string message = "cannotConnectDevice";
            const string message = "serialErrorList";

            using var client = new TcpClient();

            // 비동기로 연결 시도
            try
            {
                await Task.Run(() => client.Connect(Host, SocketServerPort));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Client not connected");
            }

            // 연결 상태 확인
            if (!client.Connected)
            {
                throw new InvalidOperationException("Client not connected");
            }

            using var stream = client.GetStream();

            // 메시지 전송
            var sendBuffer = System.Text.Encoding.UTF8.GetBytes(message + "\n");

            try
            {
                await stream.WriteAsync(sendBuffer.AsMemory(0, sendBuffer.Length));
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                throw new SocketException((int)SocketError.ConnectionReset);
            }
            var response = ReadJsonArrayResponse(stream, message);
            var serialErrorList = ResponseParser.ParseSerialDeviceAddressInfoList(response);

            if (serialErrorList != null)
                foreach (var v in serialErrorList)
                {
                    Debug.WriteLine(v.ToString());
                }
        }
        catch (Exception ex)
        {
           // await Dispatcher.UIThread.InvokeAsync(() => { StatusMessage = $"❌ 오류: {ex.Message}"; });
        }

        return null;
    }

    private static string ReadJsonArrayResponse(NetworkStream stream, string requestToken)
    {
        stream.ReadTimeout = 5000; // 5초로 증가

        var receiveBuffer = new byte[4096];
        var aggregate = new System.Text.StringBuilder();


        for (var attempt = 1; attempt <= 10; attempt++)
        {
            try
            {
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);


                if (bytesRead == 0)
                {
                    break;
                }

                var chunk = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                var trimmedChunk = chunk.Trim();


                // Some servers echo the request token before sending the actual JSON payload.
                if (string.Equals(trimmedChunk, requestToken, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                aggregate.Append(chunk);

                if (TryExtractJsonArray(aggregate.ToString(), out var json))
                {
                    return json;
                }
            }
            catch (TimeoutException tex)
            {
                if (attempt >= 10)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        throw new InvalidDataException("JSON array payload was not received from server.");
    }

    private static bool TryExtractJsonArray(string text, out string json)
    {
        var trimmed = text.Trim();
        if (LooksLikeJsonArray(trimmed))
        {
            json = trimmed;
            return true;
        }

        var start = text.IndexOf('[');
        var end = text.LastIndexOf(']');
        if (start >= 0 && end > start)
        {
            var candidate = text.Substring(start, end - start + 1).Trim();
            if (LooksLikeJsonArray(candidate))
            {
                json = candidate;
                return true;
            }
        }

        json = string.Empty;
        return false;
    }

    private static bool LooksLikeJsonArray(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var trimmed = text.Trim();
        return trimmed.StartsWith("[") && trimmed.EndsWith("]");
    }

    // 텍스트 기반 TCP 요청/응답(UTF-8) 유틸리티
    private async Task<string> SendSocketMessageAsync(string message, int timeoutMs = 3000, bool appendNewLine = true)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("message must not be empty", nameof(message));
        }

        using var client = new TcpClient();
        using var timeoutCts = new CancellationTokenSource(timeoutMs);

        try
        {
            
            await client.ConnectAsync(Host, SocketServerPort, timeoutCts.Token);
            await using var stream = client.GetStream();

            var payload = appendNewLine ? $"{message}\n" : message;
            var requestBytes = Encoding.UTF8.GetBytes(payload);

            await stream.WriteAsync(requestBytes.AsMemory(0, requestBytes.Length), timeoutCts.Token);
            await stream.FlushAsync(timeoutCts.Token);

            var responseBuffer = new byte[1024];
            var responseBuilder = new StringBuilder();

            while (true)
            {
                var read = await stream.ReadAsync(responseBuffer.AsMemory(0, responseBuffer.Length), timeoutCts.Token);
                if (read == 0)
                {
                    break;
                }

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, read));

                // 프레이밍 정보가 없으므로 현재 수신 가능한 데이터까지 읽고 반환
                if (!stream.DataAvailable)
                {
                    break;
                }
            }

            return responseBuilder.ToString().Trim();
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException(
                $"Socket request timed out after {timeoutMs}ms ({Host}:{SocketServerPort}).");
        }
        catch (SocketException ex)
        {
            throw new InvalidOperationException($"Socket communication failed ({Host}:{SocketServerPort}).",
                ex);
        }
    }
}