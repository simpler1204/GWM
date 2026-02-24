namespace GWM.Models;

public enum PortState
{
    Idle,      // 회색 (대기)
    Connected, // 녹색 (연결됨)
    Active,    // 주황색/깜빡임 (데이터 전송 중)
    Error      // 빨간색 (장애)
}