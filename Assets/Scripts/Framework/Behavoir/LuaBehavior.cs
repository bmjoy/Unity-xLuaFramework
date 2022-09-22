using System;
using Framework.Managers;
using UnityEngine;
using XLua;

namespace Framework.Behavoir
{
    public class LuaBehaviour : MonoBehaviour
    {
        private readonly LuaEnv m_LuaEnv = Manager.Lua.LuaEnv;
        private Action m_LuaInit;
        private Action m_LuaOnDestroy;
        private Action m_LuaUpdate;
        protected LuaTable m_ScriptEnv;

        // 1. 实例化之后第一个被调用
        // 2. 从实例创建到被销毁只会执行一次
        private void Awake()
        {
            m_ScriptEnv = m_LuaEnv.NewTable();
            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            var meta = m_LuaEnv.NewTable();
            meta.Set("__index", m_LuaEnv.Global);
            m_ScriptEnv.SetMetaTable(meta);
            meta.Dispose();
            m_ScriptEnv.Set("self", this);
        }


        // Update is called once per frame
        private void Update()
        {
            m_LuaUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            m_LuaOnDestroy?.Invoke();
            Clear();
        }

        private void OnApplicationQuit()
        {
            Clear();
        }

        // 用来代替 Awake
        public virtual void Init(string luaName)
        {
            m_LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), luaName, m_ScriptEnv);
            m_ScriptEnv.Get("Update", out m_LuaUpdate);
            m_ScriptEnv.Get("OnInit", out m_LuaInit);
            m_LuaInit?.Invoke();
        }

        protected virtual void Clear()
        {
            m_LuaOnDestroy = null;
            m_ScriptEnv?.Dispose();
            m_ScriptEnv = null;
            m_LuaInit = null;
            m_LuaUpdate = null;
        }
    }
}