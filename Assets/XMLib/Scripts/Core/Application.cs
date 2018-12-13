/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/11/2018 3:56:05 PM
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace XMLib
{
    /// <summary>
    /// 应用程序类
    /// </summary>
    public class Application : Container, IApplication
    {
        #region 类型定义

        /// <summary>
        /// 启动流程
        /// </summary>
        public enum LaunchProcess
        {
            /// <summary>
            /// 构建阶段
            /// </summary>
            Construct = 0,

            /// <summary>
            /// 引导流程之前
            /// </summary>
            Bootstrap = 1,

            /// <summary>
            /// 引导流程进行中
            /// </summary>
            Bootstrapping = 2,

            /// <summary>
            /// 引导流程结束之后
            /// </summary>
            Bootstraped = 3,

            /// <summary>
            /// 初始化开始之前
            /// </summary>
            Init = 4,

            /// <summary>
            /// 初始化中
            /// </summary>
            Initing = 5,

            /// <summary>
            /// 初始化完成后
            /// </summary>
            Inited = 6,

            /// <summary>
            /// 框架运行中
            /// </summary>
            Running = 7,

            /// <summary>
            /// 框架终止之前
            /// </summary>
            Terminate = 8,

            /// <summary>
            /// 框架终止进行中
            /// </summary>
            Terminating = 9,

            /// <summary>
            /// 框架终止之后
            /// </summary>
            Terminated = 10,
        }

        #endregion 类型定义

        /// <summary>
        /// 是否是主线程
        /// </summary>
        public bool IsMainThread { get { return _mainThreadId == Thread.CurrentThread.ManagedThreadId; } }

        /// <summary>
        /// 主线程id
        /// </summary>
        private readonly int _mainThreadId;

        /// <summary>
        /// 事件系统
        /// </summary>
        private readonly IDispatcher _dispatcher;

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object _syncRoot = new object();

        #region 状态

        /// <summary>
        /// 启动流程
        /// </summary>
        public LaunchProcess Process
        {
            get { return _process; }
            private set { _process = value; }
        }

        /// <summary>
        /// 是否正在释放
        /// </summary>
        public bool IsFlushing { get { return _isFlushing; } }

        /// <summary>
        /// 是否正在注册
        /// </summary>
        public bool IsRegistering { get { return _isRegistering; } }

        /// <summary>
        /// 启动流程
        /// </summary>
        private LaunchProcess _process;

        /// <summary>
        /// 是否引导完成
        /// </summary>
        private bool _isBootstraped;

        /// <summary>
        /// 是否正在释放
        /// </summary>
        private bool _isFlushing;

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInited;

        /// <summary>
        /// 是否正在注册
        /// </summary>
        private bool _isRegistering;

        #endregion 状态

        #region 服务

        /// <summary>
        /// 服务提供者列表
        /// </summary>
        private readonly SortList<IServiceProvider, int> _serviceProviders;

        /// <summary>
        /// 服务提供者类型列表
        /// </summary>
        private readonly HashSet<Type> _serviceProviderTypes;

        #endregion 服务

        /// <summary>
        /// 构造函数
        /// </summary>
        public Application()
        {
            //设置全局句柄
            App.Handler = this;

            //创建对象
            _serviceProviders = new SortList<IServiceProvider, int>();
            _serviceProviderTypes = new HashSet<Type>();

            //初始化参数
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            _dispatcher = new Dispatcher();

            //初始化状态
            _isBootstraped = false;
            _isFlushing = false;
            _isInited = false;
            _isRegistering = false;

            //设置启动进程
            _process = LaunchProcess.Construct;
        }

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <returns>实例</returns>
        public static Application New()
        {
            return new Application();
        }

        /// <summary>
        /// 获取优先级
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="method">函数</param>
        /// <returns>优先级</returns>
        public int GetPriority(Type type, string method = null)
        {
            return AttributeUtil.GetPriority(type, method);
        }

        /// <summary>
        /// 终止
        /// </summary>
        public virtual void Terminate()
        {
            _process = LaunchProcess.Terminate;
            Trigger(ApplicationEvents.OnTerminate, this);
            _process = LaunchProcess.Terminating;

            //释放资源
            Flush();

            App.Handler = null;
            _process = LaunchProcess.Terminated;
            Trigger(ApplicationEvents.OnTerminated, this);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public virtual void Flush()
        {
            lock (_syncRoot)
            {
                try
                {
                    _isFlushing = true;

                    //释放服务

                    //清理
                    _serviceProviders.Clear();
                    _serviceProviderTypes.Clear();
                }
                finally
                {
                    _isFlushing = false;
                }
            }
        }

        /// <summary>
        /// 引导程序
        /// </summary>
        /// <param name="bootstraps">引导程序</param>
        public virtual void Bootstrap(params IBootstrap[] bootstraps)
        {
            Checker.Requires<ArgumentNullException>(bootstraps != null);

            if (_isBootstraped || _process != LaunchProcess.Construct)
            {//已调用或非构造状态
                throw new RuntimeException("Bootstrap() 函数不能重复调用");
            }

            _process = LaunchProcess.Bootstrap;
            Trigger(ApplicationEvents.OnBootstrap, this);
            _process = LaunchProcess.Bootstrapping;

            SortList<IBootstrap, int> sorting = new SortList<IBootstrap, int>(bootstraps.Length);

            //构建引导顺序
            foreach (IBootstrap bootstrap in bootstraps)
            {
                int priority = GetPriority(bootstrap.GetType(), "Bootstrap");
                sorting.Add(bootstrap, priority);
            }

            //调用引导
            foreach (IBootstrap bootstrap in sorting)
            {
                //如果事件没有返回任何结果,则为允许
                bool allowed = TriggerHalt(ApplicationEvents.Bootstrapping, bootstrap) == null;

                if (allowed && null != bootstrap)
                {
                    bootstrap.Bootstrap();
                }
            }

            _process = LaunchProcess.Bootstraped;
            _isBootstraped = true;
            Trigger(ApplicationEvents.OnBootstraped, this);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            StartCoroutine(CoroutineInit());
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceProvider">服务实例</param>
        public virtual void Register(IServiceProvider serviceProvider)
        {
            StartCoroutine(CoroutineRegister(serviceProvider));
        }

        /// <summary>
        /// 服务是否已注册
        /// </summary>
        /// <param name="serviceProvider">服务实例</param>
        /// <returns>是否注册</returns>
        public bool IsRegisted(IServiceProvider serviceProvider)
        {
            Checker.Requires<ArgumentNullException>(serviceProvider != null);
            return _serviceProviderTypes.Contains(serviceProvider.GetType());
        }

        #region Coroutine

        /// <summary>
        /// 默认协程启动器
        /// </summary>
        /// <param name="coroutine">协程</param>
        private void StartCoroutine(IEnumerator coroutine)
        {
            Stack<IEnumerator> stack = new Stack<IEnumerator>();
            stack.Push(coroutine);
            do
            {
                coroutine = stack.Pop();
                while (coroutine.MoveNext())
                {
                    if (coroutine.Current is IEnumerator)
                    {
                        continue;
                    }

                    IEnumerator nextCoroutine = (IEnumerator)coroutine.Current;

                    stack.Push(coroutine);
                    coroutine = nextCoroutine;
                }
            } while (stack.Count > 0);
        }

        /// <summary>
        /// 启动服务初始化携程
        /// </summary>
        /// <returns></returns>
        protected IEnumerator CoroutineInit()
        {
            if (!_isBootstraped)
            {
                throw new RuntimeException("Bootstrap() 引导程序必须在此之前调用");
            }

            if (_isInited || _process != LaunchProcess.Bootstraped)
            {
                throw new RuntimeException("已经初始化或当前启动状态不是引导程序完成");
            }

            _process = LaunchProcess.Init;
            Trigger(ApplicationEvents.OnInit, this);
            _process = LaunchProcess.Initing;

            //初始化服务
            foreach (var serviceProvider in _serviceProviders)
            {
                yield return InitProvider(serviceProvider);
            }

            _isInited = true;
            _process = LaunchProcess.Inited;
            Trigger(ApplicationEvents.OnInited, this);

            _process = LaunchProcess.Running;
            Trigger(ApplicationEvents.OnStartCompleted, this);
        }

        /// <summary>
        /// 初始化服务提供者
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns>迭代器</returns>
        private IEnumerator InitProvider(IServiceProvider serviceProvider)
        {
            //开始事件
            Trigger(ApplicationEvents.OnProviderInit, serviceProvider);

            serviceProvider.Init();
            if (serviceProvider is ICoroutineInit)
            {//存在则调用
                yield return ((ICoroutineInit)serviceProvider).CoroutineInit();
            }

            //结束事件
            Trigger(ApplicationEvents.OnProviderInited, serviceProvider);
        }

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns>迭代器</returns>
        protected IEnumerator CoroutineRegister(IServiceProvider serviceProvider)
        {
            Checker.Requires<ArgumentNullException>(serviceProvider != null);

            if (IsRegisted(serviceProvider))
            {
                throw new RuntimeException("此服务[" + serviceProvider.GetType() + "]已注册");
            }

            if (_process == LaunchProcess.Initing)
            {
                throw new RuntimeException("无法在初始化过程中注册服务");
            }

            if (_process > LaunchProcess.Running)
            {
                throw new RuntimeException("无法在终止流程中注册服务");
            }

            //如果事件没有返回任何结果,则为允许
            bool allowed = TriggerHalt(ApplicationEvents.OnRegisterProvider, serviceProvider) == null;
            if (!allowed)
            {
                yield break;
            }

            try
            {
                _isRegistering = true;
                serviceProvider.Register();
            }
            finally
            {
                _isRegistering = false;
            }

            int priority = GetPriority(serviceProvider.GetType(), "Init");//获取优先级
            _serviceProviders.Add(serviceProvider, priority);//添加到服务列表
            _serviceProviderTypes.Add(serviceProvider.GetType());//添加到服务类型列表

            if (_isInited)
            {//如果程序已初始化,则立即调用服务初始化
                yield return InitProvider(serviceProvider);
            }
        }

        #endregion Coroutine

        #region IDispatcher

        /// <summary>
        /// 是否存在事件的监听
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>是否存在</returns>
        public bool HasListener(string eventName)
        {
            return _dispatcher.HasListener(eventName);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="args">参数</param>
        /// <returns>结果集合</returns>
        public List<object> Trigger(string eventName, params object[] args)
        {
            return _dispatcher.Trigger(eventName, args);
        }

        /// <summary>
        /// 触发一个事件，遇到第一个事件存在处理结果后终止，并获取事件监听的返回结果
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="args">参数</param>
        /// <returns>结果</returns>
        public object TriggerHalt(string eventName, params object[] args)
        {
            return _dispatcher.TriggerHalt(eventName, args);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="target">目标对象</param>
        /// <param name="methodInfo">调用方法</param>
        /// <param name="group">分组</param>
        /// <returns>监听</returns>
        public IEvent On(string eventName, object target, MethodInfo methodInfo, object group = null)
        {
            return _dispatcher.On(eventName, target, methodInfo, group);
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
            _dispatcher.Off(target);
        }

        #endregion IDispatcher
    }
}