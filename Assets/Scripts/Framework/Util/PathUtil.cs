using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    // 根目录
    public static readonly string AssetsPath = Application.dataPath;
    // 需要打 Bundle 的目录
    public static readonly string BuildResourcesPath = $"{AssetsPath}/BuildResources";
    // Bundle 输出目录
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    // 获取 Unity 的相对路径
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        return path.Substring(path.IndexOf("Assets"));
    }

    // 获取标准路径：把反斜杠改为斜杠
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        return path.Trim().Replace("\\", "/");
    }
}
