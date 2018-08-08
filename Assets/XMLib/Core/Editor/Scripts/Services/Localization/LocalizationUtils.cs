﻿using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XM;
using XM.Services.Localization;
using XM.Tools;

namespace XMEditor.Services.Localization
{
    /// <summary>
    /// 语言工具
    /// </summary>
    public static class LocalizationUtils
    {
#if XM_USE_PROTOBUF
        public static string Extension = ".bytes";
#else
        public static string Extension = ".json";
#endif

        /// <summary>
        /// 获取语言配置
        /// </summary>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public static string GetPath(LanguageType languageType)
        {
            string fileName = string.Format("Language/{0}{1}", languageType, Extension);
            string filePath = ToolUtils.GetTemplatePath(fileName);
            return filePath;
        }

        /// <summary>
        /// 获取语言配置文件
        /// </summary>
        /// <returns></returns>
        public static string GetConfigPath()
        {
            string filePath = ToolUtils.GetTemplatePath("Language/Language.xlsx");
            return filePath;
        }

        /// <summary>
        /// 创建配置文件
        /// </summary>
        public static void CreateConfigFile()
        {
            string[] enumNames = Enum.GetNames(typeof(LanguageType));
            int length = enumNames.Length;
            string enumName;

            string filePath = LocalizationUtils.GetConfigPath();

            if (File.Exists(filePath))
            {
                if (!EditorUtility.DisplayDialog("警告", "语言模板已存在，是否覆盖？", "覆盖", "取消"))
                {
                    return;
                }
            }

            using (var excel = new ExcelPackage())
            {
                var sheet = excel.Workbook.Worksheets.Add("Language");

                sheet.SetValue(1, 1, "ID");
                for (int i = 0; i < length; i++)
                {
                    enumName = enumNames[i];
                    sheet.SetValue(1, i + 2, enumName);
                }

                excel.SaveAs(new FileInfo(filePath));
            }

            Debug.Log("Export:" + filePath);
        }

        /// <summary>
        /// 导出配置文件
        /// </summary>
        public static void ExportConfigFile()
        {
            string filePath = GetConfigPath();
            if (!File.Exists(filePath))
            {
                Debug.LogErrorFormat("文件不存在:{0}", filePath);
                return;
            }

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            List<string> indexs = new List<string>();
            string header;
            string indexName;
            List<string> items;
            using (var excel = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheet = excel.Workbook.Worksheets["Language"];

                ExcelRange range;
                string id;
                string text;
                int col = 1;
                int row = 1;

                //header
                do
                {
                    range = sheet.Cells[row, col];
                    header = range.Text;
                    if (string.IsNullOrEmpty(header))
                    {
                        break;
                    }

                    indexs.Add(header);
                    dict[header] = new List<string>();

                    col++;
                } while (true);

                if (!dict.ContainsKey("ID"))
                {
                    throw new Exception(string.Format("配置表中没有ID字段:{0}", filePath));
                }

                //取id
                col = 1;
                row = 2;
                do
                {
                    range = sheet.Cells[row, col];
                    id = range.Text;
                    if (string.IsNullOrEmpty(id))
                    {
                        break;
                    }

                    dict["ID"].Add(id);

                    row++;
                } while (true);

                //取值
                int maxCol = indexs.Count;
                int maxRow = dict["ID"].Count;

                for (col = 2; col <= maxCol; col++)
                {
                    indexName = indexs[col - 1];
                    items = dict[indexName];
                    for (row = 2; row <= maxRow + 1; row++)
                    {
                        range = sheet.Cells[row, col];
                        text = range.Text;
                        if (string.IsNullOrEmpty(text))
                        {
                            Debug.LogWarningFormat("[{0},{1}] 数据为空", row, col);
                        }

                        items.Add(text);
                    }
                }
            }

            //解析
            List<LanguageInfo> infos = new List<LanguageInfo>();
            LanguageInfo info;
            int length = indexs.Count;
            List<string> ids = dict["ID"];
            int itemSize = ids.Count;
            LanguageType type;
            for (int i = 1; i < length; i++)
            {
                header = indexs[i];
                items = dict[header];

                if (!Enum.TryParse<LanguageType>(header, out type))
                {
                    throw new Exception(string.Format("{0} 不是 LanguageType", header));
                }

                info = new LanguageInfo();
                info.Language = type;

                info.Items = new List<LanguageItem>();

                for (int j = 0; j < itemSize; j++)
                {
                    info.Items.Add(new LanguageItem()
                    {
                        ID = ids[j],
                        Text = items[j]
                    });
                }

                infos.Add(info);
            }

            //导出
            length = infos.Count;
            for (int i = 0; i < length; i++)
            {
                info = infos[i];
                WriteFile(info);
            }
        }

        /// <summary>
        /// 写入语言文件
        /// </summary>
        /// <param name="info"></param>
        public static void WriteFile(LanguageInfo info)
        {
            string filePath = GetPath(info.Language);
            Debug.LogFormat("导出语言文件:{0}", filePath);
            byte[] data = SerializationUtils.Serialize(info);
            File.WriteAllBytes(filePath, data);
        }

        /// <summary>
        /// 读取语言文件
        /// </summary>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public static LanguageInfo ReadFile(LanguageType languageType)
        {
            string filePath = GetPath(languageType);
            byte[] data = File.ReadAllBytes(filePath);
            LanguageInfo info = SerializationUtils.Deserialize<LanguageInfo>(data);
            return info;
        }

        /// <summary>
        /// 获取语言
        /// </summary>
        /// <param name="languageType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Get(LanguageType languageType, string id)
        {
            string str = null;

            LanguageInfo info = ReadFile(languageType);
            Checker.NotNull(info, "未读取到 {0} 信息", languageType);

            str = info.Get(id);

            return str;
        }
    }
}