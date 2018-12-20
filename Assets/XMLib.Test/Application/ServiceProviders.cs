/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/14/2018 12:09:23 PM
 */

using System;
using System.Collections;
using UnityEngine;

namespace XMLib.Test.ApplicationTest
{
    public class Service1 : IDisposable, IUpdate, IOnDestroy, IFixedUpdate, ILateUpdate, IOnGUI
    {
        public void Dispose()
        {
            Debug.LogFormat("{0} 被释放了，呜呜呜...", this);
        }

        public override string ToString()
        {
            return "我是" + GetType().Name;
        }

        public void Update()
        {
            Debug.Log("Service1 Update");
        }

        public void OnDestroy()
        {
            Debug.Log("Service1 OnDestroy");
        }

        public void FixedUpdate()
        {
            Debug.Log("Service1 FixedUpdate");
        }

        public void LateUpdate()
        {
            Debug.Log("Service1 LateUpdate");
        }

        public void OnGUI()
        {
            Debug.Log("Service1 OnGUI");
        }
    }

    public class ServiceProvider1 : IServiceProvider, ICoroutineInit
    {
        [Priority(3)]
        public void Init()
        {
            App.Log("ServiceProviders1 Init");
            App.Make<Service1>();
        }

        public void Register()
        {
            App.Log("ServiceProviders1 Register");

            App.Singleton<Service1>();
        }

        public IEnumerator CoroutineInit()
        {
            int index = 3;
            while (index > 0)
            {
                App.Log("ServiceProviders1 init> {0}", index);
                yield return new WaitForSeconds(1);
                index--;
            }
        }
    }

    public class ServiceProvider2 : IServiceProvider, ICoroutineInit
    {
        [Priority(2)]
        public void Init()
        {
            App.Log("ServiceProviders2 Init");
        }

        public void Register()
        {
            App.Log("ServiceProviders2 Register");
        }

        public IEnumerator CoroutineInit()
        {
            int index = 4;
            while (index > 0)
            {
                App.Log("ServiceProviders2 init> {0}", index);
                yield return new WaitForSeconds(1);
                index--;
            }
        }
    }
}