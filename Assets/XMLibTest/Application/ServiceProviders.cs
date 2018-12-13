/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/12/2018 3:50:09 PM
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace XMLib.Test
{
    public class ServiceProviders1 : IServiceProvider, ICoroutineInit
    {
        [Priority(3)]
        public void Init()
        {
            App.Log("ServiceProviders1 Init");
        }

        public void Register()
        {
            App.Log("ServiceProviders1 Register");
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

    public class ServiceProviders2 : IServiceProvider, ICoroutineInit
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