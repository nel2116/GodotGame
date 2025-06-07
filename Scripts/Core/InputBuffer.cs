using Godot;
using System.Collections.Generic;

#nullable enable

[GlobalClass]
public partial class InputBuffer : Node
{
    private struct BufferEntry
    {
        public InputEvent Event;
        public double Time;
    }

    // 入力履歴を保持するキュー
    private readonly Queue<BufferEntry> input_history = new();

    // バッファ保持時間（秒）
    [Export]
    public float RetentionTime { get; set; } = 0.5f;

    public override void _Process(double delta)
    {
        var now = Time.GetTicksMsec() / 1000.0;
        while (input_history.Count > 0 && (RetentionTime <= 0.0f || now - input_history.Peek().Time > RetentionTime))
        {
            input_history.Dequeue();
        }
    }

    // 入力イベントをキューに追加する
    public void Enqueue(InputEvent input_event)
    {
        input_history.Enqueue(new BufferEntry { Event = input_event, Time = Time.GetTicksMsec() / 1000.0 });
    }

    // 入力イベントをキューから取得する。無ければ null を返す
    public InputEvent? Dequeue()
    {
        return input_history.Count > 0 ? input_history.Dequeue().Event : null;
    }

    // 入力履歴をすべて消去する
    public void Clear()
    {
        input_history.Clear();
    }
}

