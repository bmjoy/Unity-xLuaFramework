using System;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    private IEnumerator Start()
    {
        var request1 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/prefabs/testui.prefab.ab");
        yield return request1;
    
        var request2 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/play.jpg.ab");
        yield return request2;
    
        var request3 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/item_icon_posion.png.ab");
        yield return request3;
    
        var bundle_request =
            request1.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefabs/TestUI.prefab");
        yield return bundle_request;
    
        var go = Instantiate(bundle_request.asset, transform) as GameObject;
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}