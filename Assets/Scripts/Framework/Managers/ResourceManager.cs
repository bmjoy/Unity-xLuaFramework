using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Framework.Managers
{
    public class ResourceManager : MonoBehaviour
    {
        // 存放 Bundle 信息的集合
        private readonly Dictionary<string, BundleInfo> _bundle_infos = new();

        // TODO 卸载

        /// <summary>
        ///     解析版本文件
        /// </summary>
        public void ParseVersionFile()
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

                if (info[0].IndexOf("LuaScripts") > 0)
                    Manager.Lua.LuaNames.Add(info[0]);
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

            action?.Invoke(bundle_request?.asset);
        }

#if UNITY_EDITOR
        /// <summary>
        ///     编辑器环境加载资源
        /// </summary>
        /// <param name="asset_name"></param>
        /// <param name="action"></param>
        private void EditorLoadAsset(string asset_name, Action<UObject> action = null)
        {
            Debug.Log("Loading Assets in editor mode.");

            var obj = AssetDatabase.LoadAssetAtPath(asset_name, typeof(UObject));
            if (obj == null) Debug.LogErrorFormat("assets name does not exist: {0}", asset_name);

            action?.Invoke(obj);
        }
#endif

        private void LoadAsset(string asset_name, Action<UObject> action)
        {
            if (AppConst.GameMode == GameMode.EditorMode)
            {
#if UNITY_EDITOR
                EditorLoadAsset(asset_name, action);
#endif
            }

            else
            {
                StartCoroutine(LoadBundleAsync(asset_name, action));
            }
        }

        public void LoadUI(string name, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetUIPath(name), action);
        }

        public void LoadMusic(string name, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetMusicPath(name), action);
        }

        public void LoadSound(string name, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetSoundPath(name), action);
        }

        public void LoadEffect(string name, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetEffectPath(name), action);
        }

        public void LoadScene(string name, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetScenePath(name), action);
        }

        public void LoadLua(string assetName, Action<UObject> action = null)
        {
            LoadAsset(assetName, action);
        }

        internal class BundleInfo
        {
            public string AssetsName;
            public string BundleName;
            public List<string> Dependencies;
        }
    }
}