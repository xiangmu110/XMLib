/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/18/2018 12:14:02 PM
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace XMLib.Test.ContainerTest
{
    public class Service1 : IDisposable, IService1
    {
        private string _msg = "";

        public Service1([Inject("msg")]string msg)
        {
            _msg = msg;
        }

        public void Dispose()
        {
            Debug.LogFormat("{0} 被释放了，呜呜呜...", this);
        }

        public string Get()
        {
            return "Service 1 Get";
        }

        public override string ToString()
        {
            return "我是" + GetType().Name + ">" + _msg;
        }
    }

    public interface IService1
    {
        string Get();
    }

    public class Service2 : IDisposable
    {
        private IService1 _service1;

        public Service2(IService1 service1)
        {
            _service1 = service1;
        }

        public void Dispose()
        {
            Debug.LogFormat("{0} 被释放了，呜呜呜...", this);
        }

        public override string ToString()
        {
            return "我是" + GetType().Name + ">" + _service1.Get();
        }
    }
}