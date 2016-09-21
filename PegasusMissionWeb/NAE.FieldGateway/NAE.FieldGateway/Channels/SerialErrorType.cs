
namespace NAE.FieldGateway.Channels
{
    public enum SerialErrorType
    {
        Unknown,
        Open,
        Close,
        CutDownImmediate,
        CutDownAtAltitude,
        CutDownInMinutes,
        ParachuteDeployImmediate,
        ParachuteDeployAtAltitude,
        ParachuteDeployFalling,
        Telemetry
    }
}
