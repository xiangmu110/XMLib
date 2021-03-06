﻿/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/11/2018 2:43:54 PM
 */

using System;
using System.Collections.Generic;

namespace XMLib
{
    /// <summary>
    /// SortList 接口
    /// </summary>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <typeparam name="TWeight">权重类型</typeparam>
    public interface ISortList<TValue, TWeight> : IEnumerable<TValue>
        where TWeight : IComparable<TWeight>
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>值</returns>
        TValue this[int index] { get; }

        /// <summary>
        /// 获取权重
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>权重</returns>
        TWeight this[TValue value] { get; }

        /// <summary>
        /// 数量
        /// </summary>
        int count { get; }

        /// <summary>
        /// 迭代器迭代方向
        /// </summary>
        bool forward { get; set; }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="weight">权重</param>
        void Add(TValue value, TWeight weight);

        /// <summary>
        /// 添加一组
        /// </summary>
        /// <param name="sortList">一组数据</param>
        void AddRange(ISortList<TValue, TWeight> sortList);

        /// <summary>
        /// 清理
        /// </summary>
        void Clear();

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否包含</returns>
        bool Contains(TValue value);

        /// <summary>
        /// 获取排名
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>从 0 开始，未找到为-1</returns>
        int GetIndex(TValue value);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>值</returns>
        TValue GetValue(int index);

        /// <summary>
        /// 获取权重
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>权重</returns>
        TWeight GetWeight(TValue value);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否存在</returns>
        bool Remove(TValue value);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="index">序号</param>
        void RemoveIndex(int index);

        /// <summary>
        /// 排序
        /// </summary>
        void Sort();

        /// <summary>
        /// 转换为列表
        /// </summary>
        /// <returns>列表</returns>
        List<TValue> ToList();

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns>列表</returns>
        TValue[] ToArray();
    }
}