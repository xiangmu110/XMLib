﻿using System.Collections.Generic;
using UnityEngine;

namespace XM.Services.UI
{
    /// <summary>
    /// UI设置
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(menuName = "XMLib/UI Setting")]
    public class UISetting : SimpleSetting
    {
        #region 设置

        /// <summary>
        /// UI根节点
        /// </summary>
        [SerializeField]
        protected GameObject _root;

        /// <summary>
        /// 面板
        /// </summary>
        [SerializeField]
        protected List<GameObject> _panels;

        #endregion 设置

        #region 公开

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns></returns>
        public GameObject GetRoot()
        {
            return _root;
        }

        /// <summary>
        /// 获取面板预制
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <returns></returns>
        public GameObject GetPanel(string panelName)
        {
            UpdatePanelDict();

            GameObject panelObj;
            if (_panelDict.TryGetValue(panelName, out panelObj))
            {
            }

            return panelObj;
        }

        #endregion 公开

        #region 不公开

        protected Dictionary<string, GameObject> _panelDict;

        /// <summary>
        /// 获取面板字典
        /// </summary>
        /// <param name="isForce">强制更新</param>
        protected void UpdatePanelDict(bool isForce = false)
        {
            if (!isForce && null != _panelDict)
            {
                return;
            }

            //转换成字典
            int length = _panels.Count;
            _panelDict = new Dictionary<string, GameObject>(length);

            GameObject obj;
            IUIPanel panel;
            for (int i = 0; i < length; i++)
            {
                obj = _panels[i];
                panel = obj.GetComponent<IUIPanel>();
                _panelDict.Add(panel.PanelName, obj);
            }
        }

        #endregion 不公开
    }
}