using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TestAssets",menuName ="CreatAssets",order =1)]
public class AssetsSerilize : ScriptableObject
{
    public int id;
    public string name;
    public List<int> list;
  
}
