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
    public bool IsEth0live { get; set; }
    public bool IsEth1live { get; set; }
    public bool IsEth2live { get; set; }
    
    // Port States for Visualization
    public PortState Lan1State { get; set; }
    public PortState Lan2State { get; set; }
    public PortState Lan3State { get; set; }
    
    public PortState Serial1State { get; set; }
    public PortState Serial2State { get; set; }
    public PortState Serial3State { get; set; }
    public PortState Serial4State { get; set; }
    public PortState Serial5State { get; set; }
    public PortState Serial6State { get; set; }
    public PortState Serial7State { get; set; }
    public PortState Serial8State { get; set; }
}