using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    /// <summary>
    /// 开关状态类。无法继承此类
    /// </summary>
    public sealed class SwitchStatus : IDisposeAware
    {
        private bool _isOn;
        private int _instances;
        private Action<bool> _action;

        /// <summary>
        /// 构造 SwitchStatus 的新实例
        /// </summary>
        public SwitchStatus()
            : this(null)
        {
        }

        /// <summary>
        /// 构造指定打开时执行的操作 SwitchStatus 的新实例
        /// </summary>
        /// <param name="action">打开时执行的操作委托</param>
        public SwitchStatus(Action<bool> action)
        {
            _action = action;
        }

        /// <summary>
        /// 获取状态是否以打开
        /// </summary>
        public bool IsOn
        {
            get { return _isOn; }
            private set
            {
                _isOn = value;
                if (_action != null) _action(value);
            }
        }

        /// <summary>
        /// 获取状态是否关闭
        /// </summary>
        public bool IsOff
        {
            get { return !_isOn; }
        }

        /// <summary>
        /// 打开状态。
        /// <para>用using指令使用</para>
        /// </summary>
        /// <returns></returns>
        public IDisposable Open()
        {
            return Open(null);
        }

        IList<IDisposable> _disposables;
        /// <summary>
        /// 打开状态。
        /// <para>用using指令使用</para>
        /// </summary>
        /// <returns></returns>
        public IDisposable Open(params IDisposable[] disposables)
        {
            if (disposables == null)
                _disposables = null;
            else
                _disposables = disposables.ToList();

            IsOn = true;
            _instances++;
            return new Disposer(this);
        }

        /// <summary>
        /// 开关复位
        /// </summary>
        public void Reset()
        {
            _instances = 0;
            IsOn = false;
            if (_disposables != null)
            {
                foreach (var item in _disposables)
                {
                    item.Dispose();
                }
            }
            foreach (var item in _callbacks)
            {
                item.Callback();
            }
            _callbacks.Clear();
        }

        List<CallbackContext> _callbacks = new List<CallbackContext>();
        /// <summary>
        /// 增加回调处理。在复位时将调用。
        /// </summary>
        /// <param name="callback"></param>
        public void AddCallback(CallbackContext callback)
        {
            _callbacks.Add(callback);
        }

        #region IDisposeAware 成员

        void IDisposeAware.OnDispose(object key)
        {
            _instances--;
            if (_instances <= 0) Reset();
        }

        #endregion
    }
}
