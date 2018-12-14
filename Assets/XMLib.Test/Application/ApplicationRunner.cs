/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 12/14/2018 10:35:30 AM
 */

using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace XMLib.Test
{
    public class ApplicationRunner
    {
        [UnityTest]
        public IEnumerator Run()
        {
            GameObject obj = new GameObject();
            obj.AddComponent<BootstrapTest1>();
            obj.AddComponent<BootstrapTest2>();
            obj.AddComponent<FrameworkTest>();

            bool waitFlag = false;
            App.On(ApplicationEvents.OnStartCompleted, () =>
            {
                waitFlag = true;
            });

            yield return new WaitUntil(() => waitFlag);
            GameObject.Destroy(obj);
        }
    }
}