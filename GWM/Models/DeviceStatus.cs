using System;

namespace GWM.Models;

public class DeviceStatus
{
    public DateTime Timestamp { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    
    // LAN Info
    public string Eth0 { get; set; } = "N/A";
    public string Eth1 { get; set; } = "N/A";
    public string Eth2 { get; set; } = "N/A";
    public bool IsEth0Live { get; set; }
    public bool IsEth1Live { get; set; }
    public bool IsEth2Live { get; set; }
    
    // Port States for Visualization
    public PortState Lan1State { get; set; }
    public PortState Lan2State { get; set; }
    public PortState Lan3State { get; set; }
    
    public bool Serial1State { get; set; }
    public bool Serial2State { get; set; }
    public bool Serial3State { get; set; }
    public bool Serial4State { get; set; }
    public bool Serial5State { get; set; }
    public bool Serial6State { get; set; }
    public bool Serial7State { get; set; }
    public bool Serial8State { get; set; }
}