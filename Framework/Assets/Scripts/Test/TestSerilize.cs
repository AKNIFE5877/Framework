using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


[System.Serializable]
public class TestSerilize
{

    [XmlAttribute("ID")]
    public int id { get; set; }
    [XmlAttribute("Name")]
    public string name { get; set; }
    [XmlElement("List")]
    public List<int> list { get; set; }
}
