using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    /// <summary>
    /// 表示回调上下文。
    /// </summary>
    public class CallbackContext
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly CallbackContext Empty = new CallbackContext((Action)null);

        Action _action;
        Delegate _delegate;
        object[] _args;
        CallbackContext _callback;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public CallbackContext(Action callback)
            : this(null, callback, null, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public CallbackContext(CallbackContext callback)
            : this(callback, null, null, null)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public CallbackContext(Delegate callback, params object[] args)
            : this(null, null, callback, args)
        {
        }

        private CallbackContext(CallbackContext callback, Action action, Delegate @delegate, object[] args)
        {
            _delegate = @delegate;
            _action = action;
            _args = args;
            _callback = callback;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Callback()
        {
            try
            {
                if (_delegate != null)
                    _delegate.DynamicInvoke(_args);
                else if (_action != null)
                    _action();
                else if (_callback != null)
                    _callback.Callback();
            }
            catch { }
        }
    }
}
