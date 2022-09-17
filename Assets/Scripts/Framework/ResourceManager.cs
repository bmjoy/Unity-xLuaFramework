using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Framework
{
    public class ResourceManager : MonoBehaviour
    {
        // 存放 Bundle 信息的集合
        private readonly Dictionary<string, BundleInfo> _bundle_infos = new();

        /// <summary>
        ///     解析版本文件
        /// </summary>
        private void ParseVersionFile()
        {
            // 版本文件的路径
            var url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
            var data = File.ReadAllLines(url);

            // 解析文件信息
            for (var i = 0; i < data.Length; i++)
            {
                var bundle_info = new BundleInfo();
                var info = data[i].Split('|');
                bundle_info.AssetsName = info[0];
                bundle_info.BundleName = info[1];
                bundle_info.Dependencies = new List<string>(info.Length - 2);
                for (var j = 2; j < info.Length; j++) bundle_info.Dependencies.Add(info[j]);
                _bundle_infos.Add(bundle_info.AssetsName, bundle_info);
            }
        }

        /// <summary>
        ///     异步加载资源
        /// </summary>
        /// <param name="asset_name">资源名</param>
        /// <param name="action">完成回调</param>
        /// <returns></returns>
        private IEnumerator LoadBundleAsync(string asset_name, Action<UObject> action = null)
        {
            var bundle_name = _bundle_infos[asset_name].BundleName;
            var bundle_path = Path.Combine(PathUtil.BundleResourcePath, bundle_name);

            var dependencies = _bundle_infos[asset_name].Dependencies;
            if (dependencies != null && dependencies.Count > 0)
                foreach (var t in dependencies)
                    yield return LoadBundleAsync(t);

            var request = AssetBundle.LoadFromFileAsync(bundle_path);
            yield return request;

            var bundle_request = request.assetBundle.LoadAssetAsync(asset_name);
            yield return bundle_request;
            
            action?.Invoke(bundle_request?.asset); // TODO bundle_request.asset ?
        }

        public void LoadAsset(string asset_name, Action<UObject> action)
        {
            StartCoroutine(LoadBundleAsync(asset_name, action));
        }
        
        private void Start()
        {
            ParseVersionFile();
            LoadAsset("Assets/BuildResources/UI/Prefabs/TestUI.prefab", OnComplete);
        }

        private void OnComplete(UObject obj)
        {
            var go = Instantiate(obj, transform) as GameObject;
            if (go == null) return;
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
        }

        internal class BundleInfo
        {
            public string AssetsName;
            public string BundleName;
            public List<string> Dependencies;
        }
    }
}