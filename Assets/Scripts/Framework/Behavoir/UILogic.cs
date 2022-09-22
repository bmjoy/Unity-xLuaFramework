using System;

namespace Framework.Behavoir
{
    public class UILogic : LuaBehaviour
    {
        public string AssetName;
        private Action m_LuaOnClose;
        private Action m_LuaOnOpen;

        public override void Init(string luaName)
        {
            base.Init(luaName);
            m_ScriptEnv.Get("OnOpen", out m_LuaOnOpen);
            m_ScriptEnv.Get("OnClose", out m_LuaOnClose);
        }

        public void OnOpen()
        {
            m_LuaOnOpen?.Invoke();
        }

        public void Close()
        {
            m_LuaOnClose?.Invoke();
            // Manager.Pool.UnSpawn("UI", AssetName, this.gameObject);
        }

        protected override void Clear()
        {
            base.Clear();
            m_LuaOnOpen = null;
            m_LuaOnClose = null;
        }
    }
}