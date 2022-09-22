using Framework;
using Framework.Managers;
using UnityEngine;
using XLua;

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
            var function = Manager.Lua.LuaEnv.Global.Get<LuaFunction>("Main");
            function.Call();
        });
    }
}