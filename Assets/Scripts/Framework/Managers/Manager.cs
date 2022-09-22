using UnityEngine;

namespace Framework.Managers
{
    public class Manager : MonoBehaviour
    {
        public static ResourceManager Resource { get; private set; }
        public static LuaManager Lua { get; private set; }
        public static UIManager UI { get; private set; }

        private void Awake()
        {
            Resource = gameObject.AddComponent<ResourceManager>();
            Lua = gameObject.AddComponent<LuaManager>();
            UI = gameObject.AddComponent<UIManager>();
        }
    }
}