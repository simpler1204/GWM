using System.Collections.Generic;
using Newtonsoft.Json;

namespace GWM.Dto;

public class AllErrorsResponse
{
    [JsonProperty("tcpConfigErrors")]
    public List<TcpConfigErrorResponse> TcpConfigErrors { get; set; }

    [JsonProperty("cannotConnectDevices")]
    public List<string> CannotConnectDevices { get; set; }

    [JsonProperty("serialErrors")]
    public List<SerialErrorResponse> SerialErrors { get; set; }
}