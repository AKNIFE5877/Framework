using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{

    void Awake()
    {
        AssetBundleManager.Instance.LoadAssetBundleConfig();
    }


    void Update()
    {
        
    }
}
