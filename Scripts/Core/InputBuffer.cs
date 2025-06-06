using Godot;
using System.Collections.Generic;

#nullable enable

[GlobalClass]
public partial class InputBuffer : Node
{
    // 受け取った入力イベントを保持するキュー
    private readonly Queue<InputEvent> input_history = new();

    // 入力イベントをキューに追加する
    public void Enqueue(InputEvent input_event)
    {
        input_history.Enqueue(input_event);
    }

    // 入力イベントをキューから取得する。無ければ null を返す
    public InputEvent? Dequeue()
    {
        return input_history.Count > 0 ? input_history.Dequeue() : null;
    }

    // 入力履歴をすべて消去する
    public void Clear()
    {
        input_history.Clear();
    }
}

