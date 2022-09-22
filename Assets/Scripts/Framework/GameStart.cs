using Framework;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        AppConst.GameMode = GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        Manager.Lua.Init(() =>
        {
            Manager.Lua.StartLua("main");
            XLua.LuaFunction function = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
            function.Call();
        });
    }
}