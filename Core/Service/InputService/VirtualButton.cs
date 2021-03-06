/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/28/2018 10:38:31 AM
 */

using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// 虚拟按钮
    /// </summary>
    public class VirtualButton
    {
        /// <summary>
        /// 按钮名
        /// </summary>
        public string name { get { return _name; } }

        /// <summary>
        /// 按钮名
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 按下的帧
        /// </summary>
        private int _lastPressedFrame;

        /// <summary>
        /// 释放的帧
        /// </summary>
        private int _releasedFrame;

        /// <summary>
        /// 是否按下
        /// </summary>
        private bool _pressed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名字</param>
        public VirtualButton(string name)
        {
            _name = name;

            _lastPressedFrame = 0;
            _releasedFrame = 0;
            _pressed = false;
        }

        /// <summary>
        /// 按下
        /// </summary>
        public void Pressed()
        {
            if (_pressed)
            {
                return;
            }

            _pressed = true;
            _lastPressedFrame = Time.frameCount;
        }

        /// <summary>
        /// 抬起
        /// </summary>
        public void Released()
        {
            _pressed = false;
            _releasedFrame = Time.frameCount;
        }

        /// <summary>
        /// 按住
        /// </summary>
        public bool Holding { get { return (_lastPressedFrame < Time.frameCount) && !(!_pressed && _releasedFrame < Time.frameCount); } }

        /// <summary>
        /// 按下
        /// </summary>
        public bool Down { get { return (_lastPressedFrame - Time.frameCount) == -1; } }

        /// <summary>
        /// 抬起
        /// </summary>
        public bool Up { get { return (_releasedFrame - Time.frameCount) == -1; } }

        /// <summary>
        ///  将要按住
        /// </summary>
        public bool WillHolding { get { return _pressed && (_lastPressedFrame == Time.frameCount); } }

        /// <summary>
        /// 将要按下
        /// </summary>
        public bool WillDown { get { return _lastPressedFrame == Time.frameCount; } }

        /// <summary>
        ///  将要抬起
        /// </summary>
        public bool WillUp { get { return _releasedFrame == Time.frameCount; } }
    }
}