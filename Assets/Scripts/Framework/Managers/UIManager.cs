using System.Collections.Generic;
using Framework.Behavoir;
using UnityEngine;

namespace Framework.Managers
{
    public class UIManager : MonoBehaviour
    {
        // 缓存 UI TODO 临时使用
        private readonly Dictionary<string, GameObject> m_UI = new();

        private Transform m_UIParent;

        private void Awake()
        {
            m_UIParent = transform.parent.Find("UI");
        }

        public void OpenUI(string uiName, string luaName)
        {
            GameObject ui = null;

            if (m_UI.TryGetValue(uiName, out ui))
            {
                var uiLogic = ui.GetComponent<UILogic>();
                uiLogic.OnOpen();
                return;
            }

            Manager.Resource.LoadUI(uiName, obj =>
            {
                ui = Instantiate(obj) as GameObject;
                m_UI.Add(uiName, ui);

                var uiLogic = ui.AddComponent<UILogic>();

                uiLogic.Init(luaName); // 相当于 Awake
                uiLogic.OnOpen(); // 相当于 Start
            });
        }
    }
}