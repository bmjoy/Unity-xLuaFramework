using Framework;
using UnityEngine;

public class PathUtil
{
    // 根目录
    public static readonly string AssetsPath = Application.dataPath;

    // 需要打 Bundle 的目录
    public static readonly string BuildResourcesPath = $"{AssetsPath}/BuildResources";

    // Bundle 输出目录
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    // 只读目录
    public static readonly string ReadPath = Application.streamingAssetsPath;

    // 可读写目录
    public static readonly string ReadWritePath = Application.persistentDataPath;

    // Lua 路径
    public static readonly string LuaPath = "Assets/BuildResources/LuaScripts";

    // Bundle 资源路径
    public static string BundleResourcePath
    {
        get
        {
            if (AppConst.GameMode == GameMode.UpdateMode) return ReadWritePath;

            return ReadPath;
        }
    }

    // 获取 Unity 的相对路径
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        return path.Substring(path.IndexOf("Assets"));
    }

    // 获取标准路径：把反斜杠改为斜杠
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        return path.Trim().Replace("\\", "/");
    }

    public static string GetLuaPath(string name)
    {
        return $"Assets/BuildResources/LuaScripts/{name}.bytes";
    }

    public static string GetUIPath(string name)
    {
        return $"Assets/BuildResources/UI/Prefabs/{name}.prefab";
    }

    public static string GetMusicPath(string name)
    {
        return $"Assets/BuildResources/Audio/Music/{name}";
    }

    public static string GetSoundPath(string name)
    {
        return $"Assets/BuildResources/Audio/Sound/{name}";
    }

    public static string GetEffectPath(string name)
    {
        return $"Assets/BuildResources/Effect/Prefabs/{name}.prefab";
    }

    public static string GetSpritePath(string name)
    {
        return $"Assets/BuildResources/Sprites/{name}";
    }

    public static string GetScenePath(string name)
    {
        return $"Assets/BuildResources/Scenes/{name}.unity";
    }
}