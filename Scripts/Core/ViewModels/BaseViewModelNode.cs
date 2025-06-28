using Godot;

namespace Core.ViewModels
{
    /// <summary>
    /// ViewModelNodeの基底クラス
    /// 共通のnullチェックロジックを提供
    /// </summary>
    public abstract partial class BaseViewModelNode : Node
    {
        /// <summary>
        /// ViewModelが初期化されていることを確認し、未初期化の場合はエラーメッセージを出力してfalseを返す
        /// </summary>
        /// <typeparam name="T">ViewModelの型</typeparam>
        /// <param name="viewModel">チェック対象のViewModel</param>
        /// <param name="nodeName">ノード名（エラーメッセージ用）</param>
        /// <returns>初期化済みの場合true、未初期化の場合false</returns>
        protected bool EnsureViewModelInitialized<T>(T? viewModel, string nodeName) where T : class
        {
            if (viewModel == null)
            {
                GD.PrintErr($"{nodeName}: ViewModel is not initialized. Call Initialize() first.");
                return false;
            }
            return true;
        }
    }
} 