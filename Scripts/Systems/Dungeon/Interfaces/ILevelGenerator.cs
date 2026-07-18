using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Interfaces
{
    /// <summary>
    /// レベル生成のインターフェース
    /// ダンジョン 1 フロア分の部屋群を生成・検証する
    /// </summary>
    public interface ILevelGenerator
    {
        /// <summary>
        /// レベルを非同期で生成する
        /// </summary>
        /// <returns>部屋位置をキーとした部屋データの辞書</returns>
        Task<Dictionary<Vector2I, RoomData>> GenerateLevelAsync();

        /// <summary>
        /// 乱数シードを設定する（同一シードで同一レベルを再現可能にする）
        /// </summary>
        /// <param name="seed">乱数シード値</param>
        void SetSeed(int seed);

        /// <summary>
        /// 生成されたレベルが要件を満たしているか検証する
        /// </summary>
        /// <param name="rooms">検証対象の部屋データの辞書</param>
        /// <returns>検証に合格した場合は true</returns>
        bool ValidateLevel(Dictionary<Vector2I, RoomData> rooms);
    }
}
