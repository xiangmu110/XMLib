﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib;
using XMLib.MonoDriver;
using IServiceProvider = XMLib.IServiceProvider;

/// <summary>
/// 游戏管理器
/// </summary>
[DefaultExecutionOrder(-9999)]
public class GameManager : Framework
{
    public override void Bootstrap()
    {
        base.Bootstrap();

        LoadProvider();
    }

    /// <summary>
    /// 加载服务提供者
    /// </summary>
    private void LoadProvider()
    {
        //非mono服务提供者
        foreach (IServiceProvider provider in new IServiceProvider[] {
            new MonoDriverProvider()
        })
        {
            App.Register(provider);
        }

        //mono服务提供者
        Component root = App.Make<Component>();
        if (null != root)
        {
            IServiceProvider[] providers = root.GetComponentsInChildren<IServiceProvider>();
            foreach (IServiceProvider provider in providers)
            {
                if (null == provider)
                {
                    continue;
                }

                App.Register(provider);
            }
        }
    }
}