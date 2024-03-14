using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
            {
                return go as T;
            }
        }

        return Resources.Load<T>(path);
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@EnablePool_Root");
            if (root == null)
            {
                root = new GameObject { name = "@EnablePool_Root" };
            }
            return root;
        }
    }

    //public GameObject Instantiate(string path, Vector3 position, Quaternion rotation, Transform parent = null)
    //{
    //    GameObject go = Instantiate(path, parent);
    //    go.SetActive(false);
    //    go.transform.SetPositionAndRotation(position, rotation);
    //    go.SetActive(true);

    //    return go;
    //}

    public GameObject Instantiate(string path, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        // 1. original 이미 들고 있으면 바로 사용
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // 2. 혹시 풀링된 애가 있을까?
        if (original.GetComponent<Poolable>() != null)
        {
            return Managers.Pool.Pop(original, position, rotation, Root.transform).gameObject;
        }

        GameObject go = Object.Instantiate(original, position, rotation, parent);
        go.name = original.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        // 만약에 풀링이 필요한 아이라면 -> 풀링 매니저한테 위탁
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
