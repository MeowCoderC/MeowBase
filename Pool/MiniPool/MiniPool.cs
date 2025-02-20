using System.Collections.Generic;
using UnityEngine;

public class MiniPool<T> where T : Component
{
    private List<T> pool = new List<T>();
    private T prefab;
    private Transform parent;

    public void Initialize(T prefab, int initialCount, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialCount; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public T Spawn(Vector3 position, Quaternion rotation)
    {
        T obj = Spawn();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public T Spawn()
    {
        T obj = pool.Find(x => !x.gameObject.activeSelf);
        if (obj == null)
        {
            obj = GameObject.Instantiate(prefab, parent);
            pool.Add(obj);
        }
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Despawn(T obj)
    {
        if (obj.gameObject.activeSelf)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public void DespawnAll()
    {
        foreach (var obj in pool)
        {
            if (obj.gameObject.activeSelf)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    public void ReleasePool()
    {
        DespawnAll();
        foreach (T obj in pool)
        {
            GameObject.Destroy(obj.gameObject);
        }
        pool.Clear();
    }
}
