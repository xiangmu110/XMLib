/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/28/2018 10:30:14 AM
 */

using System.Collections;
using System.Collections.Generic;
using System;

namespace XMLib
{
    /// <summary>
    /// 输入服务
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// 当前输入方式
        /// </summary>
        ActiveInputMethod method { get; }

        /// <summary>
        /// 切换输入方式
        /// </summary>
        /// <param name="method">输入方式</param>
        /// <param name="deadZoom">死区</param>
        void SwitchInputMethod(ActiveInputMethod method, float deadZoom = 0);

        #region 控制

        /// <summary>
        /// 设置轴值
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="value">值</param>
        void SetAxis(string name, float value);

        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">名字</param>
        void SetButtonDown(string name);

        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">名字</param>
        void SetButtonUp(string name);

        /// <summary>
        /// 获取轴值
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>获取轴值</returns>
        float GetAxis(string name);

        /// <summary>
        /// 获取轴值
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>获取轴值</returns>
        float GetAxisRaw(string name);

        /// <summary>
        /// 获取按钮状态
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>获取按钮状态</returns>
        bool GetButton(string name);

        /// <summary>
        /// 获取按钮是否按下
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>获取按钮是否按下</returns>
        bool GetButtonDown(string name);

        /// <summary>
        /// 获取按钮是否抬起
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>获取按钮是否抬起</returns>
        bool GetButtonUp(string name);

        #endregion 控制
    }
}