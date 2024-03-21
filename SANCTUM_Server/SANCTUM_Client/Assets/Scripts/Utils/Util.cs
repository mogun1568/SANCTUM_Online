using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }

    // recursive는 재귀적으로 찾을 것인지 묻는 것 ex) 자기 자식만 찾을 것인지(false), 자기 자식의 자식 쭉 찾을 것인지(true)
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    // recursive는 재귀적으로 찾을 것인지 묻는 것 ex) 자기 자식만 찾을 것인지(false), 자기 자식의 자식 쭉 찾을 것인지(true)
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object 
    {
        if (go == null)
        {
            return null;
        }

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                // 이름이 비어있거나 원하는 이름이면
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        } else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                // 이름이 비어있거나 원하는 이름이면
                if (string.IsNullOrEmpty(name) || component.name == name)
                {
                    return component;
                }
            }
        }

        return null;
    }
}
