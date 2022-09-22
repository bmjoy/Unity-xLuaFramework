using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    // 所有的 Lua 文件名
    public List<string> LuaNames = new();

    // 缓存 Lua 脚本内容
    private Dictionary<string, byte[]> _lua_scripts;

    // Lua Env
    public LuaEnv LuaEnv;

    private Action _init_done;
    public void Init(Action action)
    {
        _init_done += action;
        LuaEnv = new();
        LuaEnv.AddLoader(Loader);
        _lua_scripts = new();
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode) EditorLoadLuaScript();
        else
#endif
            LoadLuaScript();
    }

    public void StartLua(string name)
    {
        // 执行 Lua 文件
        LuaEnv.DoString($"require '{name}'");
    }

    byte[] Loader(ref string script_name)
    {
        return GetLuaScript(script_name);
    }

    public byte[] GetLuaScript(string script_name)
    {
        script_name = script_name.Replace(".", "/"); // 把 lua 脚本中的 require 的路径改为文件路径 
        string file_name = PathUtil.GetLuaPath(script_name);

        if (!_lua_scripts.TryGetValue(file_name, out var lua_script))
        {
            Debug.LogError($"lua script does not exist: {file_name}");
        }

        return lua_script;
    }

    void LoadLuaScript()
    {
        foreach (string lua_name in LuaNames)
        {
            Manager.Resource.LoadLua(lua_name, (UnityEngine.Object obj) =>
            {
                AddLuaScript(lua_name, (obj as TextAsset)?.bytes);
                if (_lua_scripts.Count >= LuaNames.Count) // 所有 lua 加载完成的时候
                {
                    _init_done?.Invoke();
                    LuaNames.Clear();
                    LuaNames = null;
                }
            });
        }
    }

    public void AddLuaScript(string assetsName, byte[] luaScript)
    {
        //m_LuaScripts.Add(assetsName, luaScript); // 这种写法重复添加会报错
        _lua_scripts[assetsName] = luaScript;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 在编辑器模式下加载 Lua 脚本
    /// </summary>
    void EditorLoadLuaScript()
    {
        string[] lua_files = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < lua_files.Length; i++)
        {
            string fileName = PathUtil.GetStandardPath(lua_files[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScript(PathUtil.GetUnityPath(fileName), file);
        }
    }
#endif

    private void Update()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Tick();
        }
    }

    private void OnDestroy()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}