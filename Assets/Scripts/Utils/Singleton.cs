using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get{ return GetInstance();}}

    protected static T instance;

    protected virtual void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this as T;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    protected static T GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject newInstance = new GameObject($"{typeof(T)} Instance");
        instance = newInstance.AddComponent<T>();

        return instance;
    }
}
