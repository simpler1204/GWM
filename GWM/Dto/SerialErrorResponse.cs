using System.Collections.Generic;
using Newtonsoft.Json;

namespace GWM.Dto;

public class SerialErrorResponse
{
    [JsonProperty("portNumber")]
    public int PortNumber { get; set; }

    [JsonProperty("gwStart")]
    public int GwStart { get; set; }

    [JsonProperty("gwEnd")]
    public int GwEnd { get; set; }

    [JsonProperty("deviceStart")]
    public int DeviceStart { get; set; }

    [JsonProperty("swap")]
    public int Swap { get; set; }
}