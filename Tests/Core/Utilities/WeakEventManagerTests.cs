using NUnit.Framework;
using Core.Utilities;
using System;

namespace Tests.Core
{
    public class WeakEventManagerTests
    {
        // WeakEventManager の private フィールド名
        private const string HANDLERS_FIELD_NAME = "_handlers";
        [Test]
        public void AddRaiseRemove_Works()
        {
            var mgr = new WeakEventManager();
            int called = 0;
            EventHandler handler = (_, _) => called++;
            mgr.AddHandler("test", handler);
            mgr.RaiseEvent("test", this, EventArgs.Empty);
            Assert.AreEqual(1, called);
            mgr.RemoveHandler("test", handler);
            mgr.RaiseEvent("test", this, EventArgs.Empty);
            Assert.AreEqual(1, called);
        }

        private class Dummy
        {
            private readonly Action _onCall;
            public Dummy(Action onCall) => _onCall = onCall;
            public void Handler(object? s, EventArgs e) => _onCall();
        }

        [Test]
        public void DeadHandlers_AreCleanedUp()
        {
            var mgr = new WeakEventManager();
            int called = 0;
            EventHandler handler = (_, _) => called++;
            mgr.AddHandler("dead", handler);

            // テスト目的で内部フィールドにアクセスする
            var dict = GetHandlers(mgr);
            dict["dead"].Add(new WeakReference(null));
            Assert.AreEqual(2, dict["dead"].Count);

            handler = null;
            mgr.RaiseEvent("dead", this, EventArgs.Empty);
            Assert.AreEqual(1, called);
            Assert.AreEqual(1, dict["dead"].Count);
        }

        [Test]
        public void MultipleEvents_Independent()
        {
            var mgr = new WeakEventManager();
            int a = 0;
            int b = 0;
            EventHandler ha = (_, _) => a++;
            EventHandler hb = (_, _) => b++;
            mgr.AddHandler("a", ha);
            mgr.AddHandler("b", hb);
            mgr.RaiseEvent("a", this, EventArgs.Empty);
            mgr.RaiseEvent("b", this, EventArgs.Empty);
            Assert.AreEqual(1, a);
            Assert.AreEqual(1, b);
        }

        /// <summary>
        /// 登録されていないイベント名を指定しても例外が発生しないことを確認
        /// </summary>
        [Test]
        public void RemoveHandler_NoSuchEvent_DoesNotThrow()
        {
            var mgr = new WeakEventManager();
            Assert.DoesNotThrow(() => mgr.RemoveHandler("none", (_, _) => { }));
        }

        /// <summary>
        /// GC 発生後に無効なハンドラ参照が削除されることを検証
        /// </summary>
        [Test]
        public void GarbageCollectedHandlers_AreRemoved()
        {
            var mgr = new WeakEventManager();
            // テストのためプライベートフィールドを取得
            var dict = GetHandlers(mgr);

            void SubscribeTemp()
            {
                var dummy = new Dummy(() => { });
                mgr.AddHandler("temp", dummy.Handler);
            }

            SubscribeTemp();
            Assert.AreEqual(1, dict["temp"].Count);
            System.GC.Collect();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            mgr.RaiseEvent("temp", this, EventArgs.Empty);
            Assert.AreEqual(0, dict["temp"].Count);
        }

        /// <summary>
        /// リフレクションで WeakEventManager の内部ハンドラ辞書を取得する
        /// </summary>
        private static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<WeakReference>> GetHandlers(WeakEventManager mgr)
        {
            // テストから安全にアクセスできるよう、今後は InternalsVisibleTo による公開を検討する
            var field = typeof(WeakEventManager).GetField(HANDLERS_FIELD_NAME, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            return (System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<WeakReference>>)field.GetValue(mgr)!;
        }

    }
}
