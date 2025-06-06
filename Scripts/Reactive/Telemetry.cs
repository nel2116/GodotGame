using UniRx;
using Godot;

public static class Telemetry
{
    public static readonly Subject<float> FpsStream = new Subject<float>();

    public static void RecordFps()
    {
        FpsStream.OnNext(Engine.GetFramesPerSecond());
    }
}
