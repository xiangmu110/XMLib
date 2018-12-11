/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/11/2018 5:18:17 PM
 */

using System;
using System.Collections.Generic;

namespace XMLib
{
    /// <summary>
    /// 事件调度
    /// </summary>
    public class Dispatcher : IDispatcher
    {
        /// <summary>
        /// 分组映射
        /// </summary>
        private readonly Dictionary<object, List<IEvent>> _groupMapping;

        /// <summary>
        /// 事件监听
        /// </summary>
        private readonly Dictionary<string, List<IEvent>> _listeners;

        /// <summary>
        /// 同步锁
        /// </summary>
        private object _syncRoot;

        /// <summary>
        /// 事件跳出标记
        /// </summary>
        protected virtual object BreakFlag { get { return false; } }

        public Dispatcher()
        {
            _groupMapping = new Dictionary<object, List<IEvent>>();
            _listeners = new Dictionary<string, List<IEvent>>();
            _syncRoot = new object();
        }

        /// <summary>
        /// 是否存在事件的监听
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>是否存在</returns>
        public bool HasListener(string eventName)
        {
            Checker.NotEmptyOrNull(eventName, "eventName");
            lock (_syncRoot)
            {
                return _listeners.ContainsKey(eventName);
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="args">参数</param>
        /// <returns>时间结果</returns>
        public List<object> Trigger(string eventName, params object[] args)
        {
            Checker.NotEmptyOrNull(eventName, "eventName");
            return Dispatch(false, eventName, args) as List<object>;
        }

        /// <summary>
        /// 触发一个事件，遇到第一个事件存在处理结果后终止，并获取事件监听的返回结果
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public object TriggerHalt(string eventName, params object[] args)
        {
            Checker.NotEmptyOrNull(eventName, "eventName");
            return Dispatch(true, eventName, args);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="func">事件调用方法</param>
        /// <param name="group">事件分组</param>
        /// <returns></returns>
        public IEvent On(string eventName, Func<string, object[], object> func, object group = null)
        {
            Checker.NotEmptyOrNull(eventName, "eventName");
            Checker.Requires<ArgumentException>(func != null);

            lock (_syncRoot)
            {
                IEvent result = SetupListener(eventName, func, group);

                if (null == group)
                {//无分组则直接返回
                    return result;
                }

                //添加到分组
                List<IEvent> events;
                if (!_groupMapping.TryGetValue(eventName, out events))
                {
                    events = new List<IEvent>();
                    _groupMapping[group] = events;
                }
                events.Add(result);

                return result;
            }
        }

        /// <summary>
        /// 解除注册的事件监听器
        /// </summary>
        /// <param name="target">
        /// 事件解除目标
        /// <para>如果传入的是字符串(<code>string</code>)将会解除对应事件名的所有事件</para>
        /// <para>如果传入的是事件对象(<code>IEvent</code>)那么解除对应事件</para>
        /// <para>如果传入的是分组(<code>object</code>)会解除该分组下的所有事件</para>
        /// </param>
        public void Off(object target)
        {
            Checker.Requires<ArgumentException>(target != null);

            lock (_syncRoot)
            {
                IEvent evt = target as IEvent;
                if (null != evt)
                {//如果是事件
                    ForgetEvent(evt);
                    return;
                }

                if (target is string)
                {//如果是事件名
                    ForgetListener((string)target);
                }

                //移除分组
                ForgetGroup(target);
            }
        }

        /// <summary>
        /// 移除事件对象
        /// </summary>
        /// <param name="evt">事件对象</param>
        private void ForgetEvent(IEvent evt)
        {
            lock (_syncRoot)
            {
                List<IEvent> events = null; ;

                if (null != evt.Group)
                {//移除分组中的事件
                    if (_groupMapping.TryGetValue(evt.Group, out events))
                    {
                        events.Remove(evt);
                        if (events.Count <= 0)
                        {
                            _groupMapping.Remove(evt.Group);
                        }
                    }
                }

                //移除事件
                events = null;
                if (_listeners.TryGetValue(evt.Name, out events))
                {
                    events.Remove(evt);
                    if (events.Count <= 0)
                    {
                        _listeners.Remove(evt.Name);
                    }
                }
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        private void ForgetListener(string eventName)
        {
            List<IEvent> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                return;
            }

            foreach (IEvent evt in events)
            {
                ForgetEvent(evt);
            }
        }

        /// <summary>
        /// 移除分组
        /// </summary>
        /// <param name="group">分组</param>
        private void ForgetGroup(object group)
        {
            List<IEvent> events;
            if (!_groupMapping.TryGetValue(group, out events))
            {
                return;
            }

            foreach (IEvent evt in events)
            {
                ForgetEvent(evt);
            }
        }

        /// <summary>
        /// 设置事件对象
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="func">响应函数</param>
        /// <param name="group">分组</param>
        /// <returns>实例</returns>
        private IEvent SetupListener(string eventName, Func<string, object[], object> func, object group)
        {
            List<IEvent> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = new List<IEvent>();
                _listeners[eventName] = events;
            }

            IEvent output = MakeEvent(eventName, func, group);
            events.Add(output);
            return output;
        }

        /// <summary>
        /// 创建事件对象
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="func">响应函数</param>
        /// <param name="group">分组</param>
        /// <returns>实例</returns>
        protected virtual IEvent MakeEvent(string eventName, Func<string, object[], object> func, object group)
        {
            return new Event(eventName, group, func);
        }

        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="half">遇到第一个事件且有结果</param>
        /// <param name="eventName">事件名</param>
        /// <param name="args">参数</param>
        /// <returns>结果</returns>
        private object Dispatch(bool half, string eventName, object[] args)
        {
            Checker.Requires<ArgumentNullException>(string.IsNullOrEmpty(eventName));

            lock (_syncRoot)
            {
                List<object> outputs = new List<object>();

                foreach (IEvent evt in GetListener(eventName))
                {
                    object result = evt.Call(eventName, args);

                    //只取一个结果时
                    if (half && result != null)
                    {
                        return result;
                    }

                    //遇到事件终止标记
                    if (result != null && result.Equals(BreakFlag))
                    {
                        break;
                    }

                    outputs.Add(result);
                }

                return half ? null : outputs;
            }
        }

        /// <summary>
        /// 获取事件对象
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>事件对象列表</returns>
        private IEnumerable<IEvent> GetListener(string eventName)
        {
            List<IEvent> events = new List<IEvent>();

            List<IEvent> result;
            if (_listeners.TryGetValue(eventName, out result))
            {
                events.AddRange(result);
            }

            return events;
        }
    }
}