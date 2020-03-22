using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class ResourceTest : MonoBehaviour
{

    //void Start()
    //{
    //    AssetBundle assetbudle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/character.model");
    //    GameObject obj = GameObject.Instantiate(assetbudle.LoadAsset<GameObject>("Attack"));
    //    ReadTestAssets();
    //}

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
