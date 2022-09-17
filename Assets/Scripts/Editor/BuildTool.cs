using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildTool : UnityEditor.Editor
    {
        [MenuItem("Tools/Build MacOS Bundle")]
        static void BuildMacOSBundle()
        {
            Build(BuildTarget.StandaloneOSX);
        }

        [MenuItem("Tools/Build Android Bundle")]
        static void BuildAndroidBundle()
        {
            Build(BuildTarget.Android);
        }

        [MenuItem("Tools/Build IOS Bundle")]
        static void BuildIOSBundle()
        {
            Build(BuildTarget.iOS);
        }

        private static void Build(BuildTarget build_target)
        {
            List<AssetBundleBuild> asset_bundle_builds = new();
            List<string> bundle_infos = new List<string>(); // 文件信息列表

            string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*",
                SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".meta")) continue;
                if (files[i].EndsWith(".DS_Store")) continue;

                AssetBundleBuild asset_bundle = new();

                string file_name = PathUtil.GetStandardPath(files[i]);

                Debug.Log("file_name = " + file_name);

                string asset_name = PathUtil.GetUnityPath(files[i]);
                asset_bundle.assetNames = new string[] { asset_name };

                string bundle_name = files[i].Replace($"{PathUtil.BuildResourcesPath}/", "").ToLower();
                asset_bundle.assetBundleName = $"{bundle_name}.ab";

                asset_bundle_builds.Add(asset_bundle);

                // 添加文件和依赖信息
                List<string> dependency_info = GetDependencies(asset_name);
                string bundle_info = asset_name + "|" + bundle_name;
                if (dependency_info.Count > 0)
                {
                    bundle_info = bundle_info + "|" + string.Join("|", dependency_info);
                }

                bundle_infos.Add(bundle_info);
            }

            if (Directory.Exists(PathUtil.BundleOutPath))
            {
                Directory.Delete(PathUtil.BundleOutPath, true);
            }

            Directory.CreateDirectory(PathUtil.BundleOutPath);

            BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, asset_bundle_builds.ToArray(),
                BuildAssetBundleOptions.None, build_target);

            File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundle_infos);
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取依赖文件列表
        /// </summary>
        /// <param name="cur_file"></param>
        /// <returns></returns>
        static List<string> GetDependencies(string cur_file)
        {
            string[] files = AssetDatabase.GetDependencies(cur_file);
            var dependence = files.Where(file => !file.EndsWith(".cs") && !file.Equals(cur_file)).ToList();
            return dependence;
        }
    }
}