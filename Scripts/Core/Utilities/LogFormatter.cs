using System.Text;

namespace Core.Utilities
{
    /// <summary>
    /// ログイベントをフォーマットするユーティリティ
    /// </summary>
    public class LogFormatter
    {
        public string Format(LogEvent logEvent)
        {
            var sb = new StringBuilder();
            sb.Append($"[{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss}] ");
            sb.Append($"{logEvent.Level}: {logEvent.Message}");

            if (logEvent.Exception != null)
            {
                sb.AppendLine();
                sb.Append($"Exception: {logEvent.Exception}");
            }

            return sb.ToString();
        }
    }
}
