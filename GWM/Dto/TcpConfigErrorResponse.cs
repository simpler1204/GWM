using Newtonsoft.Json;

namespace GWM.Dto;

public class TcpConfigErrorResponse
{
    [JsonProperty("ip")]
    public string Ip { get; set; }

    [JsonProperty("gwStart")]
    public int GwStart { get; set; }

    [JsonProperty("gwEnd")]
    public int GwEnd { get; set; }

    [JsonProperty("deviceStart")]
    public int DeviceStart { get; set; }

    [JsonProperty("swap")]
    public int Swap { get; set; }
}