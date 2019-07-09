/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2019/4/21 2:34:57
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XMLib
{
    /// <summary>
    /// GUIStyleViewer
    /// </summary>
    public class GUIStyleViewer : EditorWindow
    {
        private Vector2 scrollVector2 = Vector2.zero;
        private string search = "";

        [MenuItem("XMLib/GUIStyle查看器")]
        public static void _ShowWindow()
        {
            var win = GetWindow<GUIStyleViewer>();
            win.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Space(30);
            search = EditorGUILayout.TextField("", search, "SearchTextField", GUILayout.MaxWidth(position.x / 3));
            GUILayout.Label("", "SearchCancelButtonEmpty");
            GUILayout.EndHorizontal();
            scrollVector2 = GUILayout.BeginScrollView(scrollVector2);
            foreach (GUIStyle style in GUI.skin.customStyles)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    DrawStyleItem(style);
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawStyleItem(GUIStyle style)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Space(40);
            EditorGUILayout.SelectableLabel(style.name);
            GUILayout.FlexibleSpace();
            EditorGUILayout.SelectableLabel(style.name, style);
            GUILayout.Space(40);
            EditorGUILayout.SelectableLabel("", style, GUILayout.Height(40), GUILayout.Width(40));
            GUILayout.Space(50);
            if (GUILayout.Button("复制到剪贴板"))
            {
                TextEditor textEditor = new TextEditor();
                textEditor.text = style.name;
                textEditor.OnFocus();
                textEditor.Copy();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
    }
}