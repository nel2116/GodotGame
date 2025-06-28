using System;

namespace Systems.Player.Base
{
    /// <summary>
    /// プレイヤーシステム用例外
    /// </summary>
    public class PlayerSystemException : Exception
    {
        public string SystemName { get; }
        public string Operation { get; }

        public PlayerSystemException(string systemName, string operation, string message)
            : base(message)
        {
            SystemName = systemName;
            Operation = operation;
        }
    }
}
