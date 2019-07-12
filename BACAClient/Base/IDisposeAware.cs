using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    /// <summary>
    /// 定义 Dispose 通知的行为
    /// </summary>
    public interface IDisposeAware
    {
        /// <summary>
        /// 执行 Dispose
        /// </summary>
        /// <param name="key"></param>
        void OnDispose(object key);
    }

    /// <summary>
    /// 基于<see cref="IDisposeAware"/> 的 <see cref="IDisposable"/> 实现。
    /// </summary>
    public class Disposer : IDisposable
    {
        /// <summary>
        /// 返回一个表示空的 <see cref="Disposer"/> 的实例。
        /// </summary>
        public static readonly Disposer Empty = new Disposer();

        private IDisposeAware _aware;
        private object _key;
        private Disposer()
            : this(null, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aware"></param>
        public Disposer(IDisposeAware aware)
            : this(aware, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aware"></param>
        /// <param name="key"></param>
        public Disposer(IDisposeAware aware, object key)
        {
            _aware = aware;
            _key = key;
        }

        #region IDisposable 成员
        void IDisposable.Dispose()
        {
            if (_aware == null) return;

            _aware.OnDispose(_key);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
