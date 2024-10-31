public enum PacketID : ushort
{
    None = 0,
    S_MyPlayerID = 1,
    C_ChangeObjInfo = 2,
    S_ChangeObjInfo = 3,
    C_SaveScenario = 4,
    S_SaveScenarioResult = 5,
    C_LeaveGame = 6,
    S_BroadcastNewOwner = 7,
    Response_ScenrioList = 8,
    Request_ScenarioList = 9,
    Response_ScenrioData = 10,
    Request_ScenarioData = 11,
    ChangeMapMove = 12
}

/// <summary>
/// 시나리오 선택 모드 로컬, 클라우드, 위경도
/// </summary>
public enum SelectScenrioMode : int
{
    Local = 0,          // 로컬 모드
    Cloud,              // 클라우드 모드
    Location            // 위경도 모드
}

/// <summary>
/// 시나리오 오브젝트 선택 모드
/// </summary>
public enum ObjectSelectMode : int
{
    None = 0,           // 선택 X
    UI,                 // UI에서 선택
    VirtualMap,         // 가상지도에서 선택
}

/// <summary>
/// 시나리오 오브젝트 객체 ID
/// </summary>
public enum ObjectID : int
{
    None = 0,
    InfantryUnit,       // 보병부대
    TankUnit,           // 전차대대
    FixWingUnit,        // 고정익대대
    RotateWingUnit,     // 회전익대대
    Tank,               // 전차
    FixWing,            // 고정익
    RoateWing,          // 회전익
    //적군보병부대,
    //적군전차대대,
    //적군고정익대대,
    //적군회전익대대,
    //적군전차,
    //적군고정익,
    //적군회전익
}

/// <summary>
/// 부대 규모 타입
/// </summary>
public enum ForceSizeType : int
{
    Platoon = 0,        // 소대
    Company,            // 중대
    Battalion,          // 대대
    FixWing,            // 고정익
    RoateWing,          // 회전익
}

/// <summary>
/// 가상지도 이동 관련
/// </summary>
public enum QuntizedDirection
{
    None= 0,
    Up,             // 위
    Down,               // 아래
    Left,               // 왼쪽
    Right              // 오른쪽,
}

public enum ScenarioObjectInteractionType
{
    None = 0,
    Add = 1,
    Revision,
    Remove
}