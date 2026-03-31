using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GWM.Models;

namespace GWM.Utilities;

/// <summary>
/// TCP 클라이언트에서 JSON 형식의 SerialDeviceAddressInfo 리스트를 수신하는 클래스
/// </summary>
public sealed class SerialDeviceAddressInfoReceiver
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// TCP 클라이언트에서 비동기로 SerialDeviceAddressInfo 리스트를 수신합니다.
    /// </summary>
    /// <param name="client">TCP 클라이언트</param>
    /// <param name="bufferSize">읽기 버퍼 크기 (기본값: 4096 바이트)</param>
    /// <param name="maxPayloadBytes">최대 페이로드 크기 (기본값: 1MB)</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>수신된 SerialDeviceAddressInfo 리스트</returns>
    /// <exception cref="ArgumentNullException">client가 null인 경우</exception>
    /// <exception cref="ArgumentOutOfRangeException">bufferSize 또는 maxPayloadBytes가 0 이하인 경우</exception>
    /// <exception cref="InvalidDataException">유효한 JSON 배열을 파싱하지 못한 경우</exception>
    public async Task<List<SerialDeviceAddressInfo>> ReceiveAsync(
        TcpClient client,
        int bufferSize = 4096,
        int maxPayloadBytes = 1024 * 1024,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (bufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        if (maxPayloadBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPayloadBytes));
        }

        using var memory = new MemoryStream();
        var stream = client.GetStream();
        var buffer = new byte[bufferSize];

        while (memory.Length < maxPayloadBytes)
        {
            var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
            if (bytesRead == 0)
            {
                break;
            }

            await memory.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);

            if (TryDeserialize(memory, out var parsed) && parsed is not null)
            {
                return parsed;
            }
        }

        throw new InvalidDataException("Socket payload does not contain a complete JSON array for SerialDeviceAddressInfo.");
    }

    /// <summary>
    /// MemoryStream의 내용을 SerialDeviceAddressInfo 리스트로 역직렬화하려고 시도합니다.
    /// </summary>
    private bool TryDeserialize(MemoryStream memory, out List<SerialDeviceAddressInfo>? parsed)
    {
        var json = Encoding.UTF8.GetString(memory.GetBuffer(), 0, (int)memory.Length);

        try
        {
            parsed = JsonSerializer.Deserialize<List<SerialDeviceAddressInfo>>(json, _jsonOptions);
            return parsed is not null;
        }
        catch (JsonException)
        {
            parsed = null;
            return false;
        }
    }
}

