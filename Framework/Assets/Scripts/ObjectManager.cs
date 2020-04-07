using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectManager : Singleton<ObjectManager>
{
    protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();
    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxcount"></param>
    /// <returns></returns>
    public ClassObjectPool<T> GetOrCreatClassPool<T>(int maxcount) where T : class, new()
    {
        Type type = typeof(T);
        object outObj = null;
        if (!m_ClassPoolDic.TryGetValue(type, out outObj) || outObj == null)
        {
            ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxcount);
            m_ClassPoolDic.Add(type, newPool);
            return newPool;
        }
        return outObj as ClassObjectPool<T>;
    }
}
