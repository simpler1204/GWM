namespace GWM.Models;

/// <summary>
/// 시리얼 포트 상태 DTO (8개 포트)
/// </summary>
public class TcpAddressInfo
{ private string _ip;
    private int _gwStart;
    private int _gwEnd;
    private int _deviceStart;
    private int _swap;

    public string Ip => _ip;
    public int GwStart => _gwStart;
    public int GwEnd => _gwEnd;
    public int DeviceStart => _deviceStart;
    public int Swap => _swap;

    public TcpAddressInfo(string ip, int gwStart, int gwEnd, int deviceStart, int swap)
    {
        _ip = ip;
        _gwStart = gwStart;
        _gwEnd = gwEnd;
        _deviceStart = deviceStart;
        _swap = swap;
    }


    public override string ToString()
    {
        return $"Ip: {_ip}, GwStart: {_gwStart}, GwEnd: {_gwEnd}, DeviceStart: {_deviceStart}, Swap: {_swap}";
    }
}

