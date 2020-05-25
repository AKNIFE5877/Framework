using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    //资源关系依赖表 crc
    protected Dictionary<string, ResourceItem> m_ResourceItemDic = new Dictionary<string, ResourceItem>();
    //储存已加载的ab包，key=crc
    protected Dictionary<string, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<string, AssetBundleItem>();
    //assetBundleItem类对象池
    protected ClassObjectPool<AssetBundleItem> m_AssetBundleItemPool = ObjectManager.Instance.GetOrCreatClassPool<AssetBundleItem>(500);

    public bool LoadAssetBundleConfig()
    {
        string configPath = Application.streamingAssetsPath + "/assetbundleconfig";
        AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
        TextAsset textAsset = configAB.LoadAsset<TextAsset>("assetbundleconfig");
        if (textAsset == null)
        {
            Debug.Log("AssetBundleConfig is not exist.");
            return false;
        }
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();
        for (int i = 0; i < config.ABList.Count; i++)
        {
            ABBase abBase = config.ABList[i];
            ResourceItem item = new ResourceItem();
            item.m_CRC = abBase.Crc;
            item.m_AssetName = abBase.AssetName;
            item.m_ABName = abBase.ABName;
            item.m_DependAssetBundle = abBase.ABDependce;
            if (m_ResourceItemDic.ContainsKey(item.m_CRC))
            {
                Debug.Log("重复的CRC：" + item.m_AssetName + " ab包名:" + item.m_ABName);
            }
            else
            {
                m_ResourceItemDic.Add(item.m_CRC, item);
            }
        }
        return true;
    }
    public ResourceItem LoadResourceAssetBundle(string crc)
    {
        ResourceItem item = null;
        if (!m_ResourceItemDic.TryGetValue(crc, out item) || item == null)
        {
            Debug.Log(string.Format("LoadResourceAssetBundle error : can not find crc{0} in assetbundleconfig", crc));
            return item;
        }
        if (item.m_AssetBundle != null)
        {
            return item;
        }
        item.m_AssetBundle = LoadAssetBundle(item.m_ABName);
        if (item.m_DependAssetBundle != null)
        {
            for (int i = 0; i < item.m_DependAssetBundle.Count; i++)
            {
                LoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }

        return item;
    }
    private AssetBundle LoadAssetBundle(string name)
    {
        AssetBundleItem item = null;
        string crc = CRC32.GetCRC32(name).ToString();
        if (!m_AssetBundleItemDic.TryGetValue(crc, out item))
        {

            AssetBundle assetBundle = null;
            string fullPath = Application.streamingAssetsPath + "/" + name;
            if (File.Exists(fullPath))
            {
                assetBundle = AssetBundle.LoadFromFile(fullPath);
            }
            if (assetBundle == null)
            {
                Debug.LogError("Load Assetbundle Error:" + fullPath);
            }
            item = m_AssetBundleItemPool.Spawn(true);
            item.assetBundle = assetBundle;
            item.RefCount++;
            m_AssetBundleItemDic.Add(crc, item);
        }
        else
        {
            item.RefCount++;
        }
        return item.assetBundle;
    }
    public  void ReleaseAsset(ResourceItem item)
    {
        if (item == null)
        {
            return;
        }
        if (item.m_DependAssetBundle != null && item.m_DependAssetBundle.Count > 0)
        {
            for (int i = 0; i < item.m_DependAssetBundle.Count; i++)
            {
                UnLoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }
        UnLoadAssetBundle(item.m_AssetName);
    }
    public void UnLoadAssetBundle(string name)
    {
        AssetBundleItem item = null;
        string crc = CRC32.GetCRC32(name).ToString();
        if(m_AssetBundleItemDic.TryGetValue(crc,out item) && item != null)
        {
            item.RefCount--;
            if (item.RefCount <= 0 && item.assetBundle != null)
            {
                item.assetBundle.Unload(true);
                item.Reset();
                m_AssetBundleItemPool.Recycle(item);
                m_AssetBundleItemDic.Remove(crc);
            }
        }
    }
    public ResourceItem FindResourceItem(string crc)
    {
        return m_ResourceItemDic[crc];
    }
}
public class AssetBundleItem
{
    public AssetBundle assetBundle = null;
    public int RefCount;

    public void Reset()
    {
        assetBundle = null;
        RefCount = 0;
    }
}
public class ResourceItem
{
    public string m_CRC;
    public string m_AssetName = string.Empty;
    public string m_ABName = string.Empty;
    public List<string> m_DependAssetBundle = null;
    public AssetBundle m_AssetBundle = null;

}

