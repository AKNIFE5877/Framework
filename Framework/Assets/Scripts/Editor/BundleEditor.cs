using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class BundleEditor
{
    public static string m_BundleTargetPath = Application.streamingAssetsPath;
    public static string ABCONFIGPATH = "Assets/ABConfig.asset";

    //key:ab包名 value:路径
    public static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();

    //过滤的list
    public static List<string> m_AllFileAB = new List<string>();

    //单个prefab的ab包
    public static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

    //储存所有有效路径
    private static List<string> m_ConfigFil = new List<string>();

    [MenuItem("Tools/打包")]
    public static void Build()
    {
        //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        //AssetDatabase.Refresh();
        m_ConfigFil.Clear();
        m_AllFileDir.Clear();
        m_AllFileAB.Clear();
        m_AllPrefabDir.Clear();
        ABConfig abconfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
        foreach (ABConfig.FileDirABName item in abconfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(item.ABName))
            {
                Debug.LogError("AB包配置名字重复，请检查");

            }
            else
            {
                m_AllFileDir.Add(item.ABName, item.Path);
                m_AllFileAB.Add(item.Path);
                m_ConfigFil.Add(item.Path);
            }
        }

        string[] allStr = AssetDatabase.FindAssets("t:Prefab", abconfig.m_AllprefabPath.ToArray());
        for (int i = 0; i < allStr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / allStr.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名字的Prefab:" + obj.name);
                }
                else
                {
                    m_AllPrefabDir.Add(obj.name, allDependPath);

                }
            }
        }


        foreach (string name in m_AllFileDir.Keys)
        {
            SetABName(name, m_AllFileDir[name]);
        }
        foreach (string name in m_AllPrefabDir.Keys)
        {
            SetABName(name, m_AllPrefabDir[name]);
        }

        BunildAssetBundle();

        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < oldABNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[i], i * 1.0f / oldABNames.Length);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    static void SetABName(string name, string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null)
        {
            Debug.Log("不存在此路径：" + path);
        }
        else
        {
            assetImporter.assetBundleName = name;
        }

    }
    static void SetABName(string name, List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            SetABName(name, paths[i]);
        }
    }
    static void BunildAssetBundle()
    {
        string[] allBundle = AssetDatabase.GetAllAssetBundleNames();
        //key：全路径  value：包名
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();
        for (int i = 0; i < allBundle.Length; i++)
        {
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundle[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                {
                    continue;
                }
                Debug.Log("此AB包" + allBundle[i] + "包含文件：" + allBundlePath[j]);
                if (ValidPath(allBundlePath[j]))
                {
                    resPathDic.Add(allBundlePath[j], allBundle[i]);

                }
            }
        }
        DeleteAB();

        //生成配置表
        WriteData(resPathDic);

        BuildPipeline.BuildAssetBundles(m_BundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    static void WriteData(Dictionary<string, string> resPathDic)
    {
        AssetBundleConfig config = new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach (string path in resPathDic.Keys)
        {
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = CRC32.GetCRC32(path).ToString();
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
            abBase.ABDependce = new List<string>();
            string[] resDependce = AssetDatabase.GetDependencies(path);
            for (int i = 0; i < resDependce.Length; i++)
            {
                string tempPath = resDependce[i];
                if (tempPath == path || path.EndsWith(".cs"))
                {
                    continue;
                }
                string abName = "";
                if (resPathDic.TryGetValue(tempPath, out abName))
                {
                    if (abName == resPathDic[path])
                    {
                        continue;
                    }
                    if (!abBase.ABDependce.Contains(abName))
                    {
                        abBase.ABDependce.Add(abName);
                    }
                }
            }
            config.ABList.Add(abBase);

        }

        //写入xml
        string xmlPath = Application.dataPath + "/AssetbundleConfig.xml";
        if (File.Exists(xmlPath))
        {
            File.Delete(xmlPath);
        }
        FileStream fs = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        XmlSerializer xs = new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fs.Close();


        //写入二进制
        foreach (ABBase abBase in config.ABList)
        {
            abBase.Path = "";
        }
        string bytePath = "Assets/GameData/Data/AssetBundleConfig.bytes";
        FileStream fs1 = new FileStream(bytePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs1, config);
        fs1.Close();
    }

    static void DeleteAB()
    {
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo direction = new DirectoryInfo(m_BundleTargetPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (ContainABName(files[i].Name, allBundlesName) || files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            else
            {
                Debug.Log("此AB包已经被删除或改名了：" + files[i].Name);
                if (File.Exists(files[i].FullName))
                {
                    File.Delete(files[i].FullName);
                }
            }
        }
    }

    static bool ContainABName(string name, string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (name == (strs[i]))
            {
                return true;
            }
        }
        return false;
    }
    static bool ContainAllFileAB(string path)
    {
        for (int i = 0; i < m_AllFileAB.Count; i++)
        {
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && (path.Replace(m_AllFileAB[i], "")[0] == '/')))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否是有效路径 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        for (int i = 0; i < m_ConfigFil.Count; i++)
        {
            if (path.Contains(m_ConfigFil[i]))
            {
                return true;
            }
        }
        return false;
    }
}
