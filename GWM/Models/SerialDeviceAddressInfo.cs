namespace GWM.Models;

/// <summary>
/// 시리얼 디바이스 주소 정보 DTO
/// </summary>
public class SerialDeviceAddressInfo
{
    public string PortName { get; set; } = string.Empty;
    public int GwStart { get; set; }
    public int GwEnd { get; set; }
    public int DeviceStart { get; set; }
    public int Swap { get; set; }

    public override string ToString()
    {
        return $"PortName: {PortName}, GwStart: {GwStart}, GwEnd: {GwEnd}, DeviceStart: {DeviceStart}, Swap: {Swap}";
    }
}

