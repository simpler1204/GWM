using System;
using System.Collections.Generic;
using GWM.Models;
using Newtonsoft.Json;

namespace GWM.Utilities;

/// <summary>
/// JSON 응답을 파싱하는 유틸리티 클래스
/// </summary>
public static class ResponseParser
{
    public static List<TcpAddressInfo>? ParseTcpAddressList(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<TcpAddressInfo>>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static List<SerialDeviceAddressInfo>? ParseSerialDeviceAddressInfoList(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<SerialDeviceAddressInfo>>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static List<string>? ParseCannotConnectDeviceList(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<string>>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }
    

}

