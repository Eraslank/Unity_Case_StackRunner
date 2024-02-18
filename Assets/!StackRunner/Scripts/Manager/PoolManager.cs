using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager<T> where T : Component, IPoolable
{
    public HashSet<T> pool;

    private T prefab;

    private PoolManager() { }

    public PoolManager(T prefab)
    {
        pool = new HashSet<T>();
        this.prefab = prefab;
    }

    public List<T> GetAllActive()
    {
        return pool.ToList().FindAll(p => !p.IsAvailable);
    }

    public T Get(Transform parent = null)
    {
        var obj = pool.FirstOrDefault(p => p.IsAvailable);
        if (obj != null)
            return obj;
        obj = GameObject.Instantiate(prefab,parent);
        pool.Add(obj);
        return obj;
    }

}
