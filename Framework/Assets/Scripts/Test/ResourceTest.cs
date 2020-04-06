using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class ResourceTest : MonoBehaviour
{

    void Start()
    {
        TestLoad();
    //    AssetBundle assetbudle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/character.model");
    //    GameObject obj = GameObject.Instantiate(assetbudle.LoadAsset<GameObject>("Attack"));
    //    ReadTestAssets();
    }


    void TestLoad()
    {
        AssetBundle assetBundleconfig = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/assetbundleconfig");
        TextAsset textAsset = assetBundleconfig.LoadAsset<TextAsset>("AssetBundleConfig");
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig testserilize = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();
        string path = "Assets/Prefabs/Attack.prefab";
        string crc = CRC32.GetCRC32(path).ToString();
        ABBase abbase = null;

        for (int i = 0; i < testserilize.ABList.Count; i++)
        {
            if (testserilize.ABList[i].Crc == crc)
            {
                abbase = testserilize.ABList[i];
            }
        }
        for (int i = 0; i < abbase.ABDependce.Count; i++)
        {
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abbase.ABDependce[i]);

        }
        AssetBundle assetbundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/"+abbase.ABName);
        GameObject obj = GameObject.Instantiate(assetbundle.LoadAsset<GameObject>(abbase.AssetName));
    }
    //void ReadTestAssets()
    //{
    //    AssetsSerilize ass= UnityEditor.AssetDatabase.LoadAssetAtPath<AssetsSerilize>("Assets/TestAssets.asset");
    //    Debug.Log(ass.id + " " + ass.name);

    //}
    //void SerilizeTest()
    //{
    //    TestSerilize testserilize = new TestSerilize();
    //    testserilize.id = 1;
    //    testserilize.name = "name";
    //    testserilize.list = new List<int>();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        testserilize.list.Add(i);
    //    }
    //    XmlSerilize(testserilize);
    //}
    //void DeSerilizeTest()
    //{
    //    TestSerilize test = XmlDeserilize();
    //    Debug.Log(test.id + " " + test.name + " " + test.list);
    //}
    //void XmlSerilize(TestSerilize testserilize)
    //{
    //    FileStream fileStream = new FileStream(Application.dataPath + "/test.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
    //    StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
    //    XmlSerializer xml = new XmlSerializer(testserilize.GetType());
    //    xml.Serialize(sw, testserilize);
    //    sw.Close();
    //    fileStream.Close();
    //}
    //TestSerilize XmlDeserilize()
    //{
    //    FileStream fs = new FileStream(Application.dataPath + "/test.xml", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
    //    XmlSerializer xs = new XmlSerializer(typeof(TestSerilize));
    //    TestSerilize testSerilize = (TestSerilize)xs.Deserialize(fs);
    //    fs.Close();
    //    return testSerilize;
    //}
    //void BinarySerTest()
    //{
    //    TestSerilize testserilize = new TestSerilize();
    //    testserilize.id = 1;
    //    testserilize.name = "name";
    //    testserilize.list = new List<int>();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        testserilize.list.Add(i);
    //    }
    //    BinarySerilize(testserilize);
    //}
    //void BinaryDeSerilizeTest()
    //{
    //    TestSerilize test = BinaryDeserilize();
    //    Debug.Log(test.id + " " + test.name + " " + test.list);
    //}
    //void BinarySerilize(TestSerilize testserileze)
    //{
    //    FileStream fs = new FileStream(Application.dataPath + "/test.bytes", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
    //    BinaryFormatter bf = new BinaryFormatter();
    //    bf.Serialize(fs, testserileze);
    //    fs.Close();
    //}
    //TestSerilize BinaryDeserilize()
    //{
    //    TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/test.bytes");
    //    MemoryStream stream = new MemoryStream(textAsset.bytes);
    //    BinaryFormatter bf = new BinaryFormatter();
    //    TestSerilize testserilize = (TestSerilize)bf.Deserialize(stream);
    //    stream.Close();
    //    return testserilize;
    //}
}
