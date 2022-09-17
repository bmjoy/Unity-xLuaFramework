using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private IEnumerator Start()
    {
        AssetBundleCreateRequest request1 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/prefabs/testui.prefab.ab");
        yield return request1;
        
        AssetBundleCreateRequest request2 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/play.jpg.ab");
        yield return request2;
        
        AssetBundleCreateRequest request3 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/item_icon_posion.png.ab");
        yield return request3;

        AssetBundleRequest bundle_request =
            request1.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefabs/TestUI.prefab");
        yield return bundle_request;
        
        GameObject go = Instantiate(bundle_request.asset, transform) as GameObject;
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
