using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component{

    protected static T m_sInstance;
    protected static bool m_isValid;
    protected bool useDontdestroyOnLoad = true;
    public static bool IsValid()
    {
        return m_isValid;
    }
    protected bool m_hasInit = false;

    public static T Instance
    {
        get
        {
            if (!IsValid())
            {
                Create();
            }
            return m_sInstance;
        }
    }

    private static void Create()
    {
        if (IsValid())
            return;
        if(m_sInstance == null)
        {
            m_sInstance = FindObjectOfType<T>();
            if (m_sInstance == null)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = typeof(T).Name;
                m_sInstance = gameObject.AddComponent<T>();
            }
        }
    }

    protected virtual void OnDestroy()
    {
        m_isValid = false;
    }

    protected void Awake()
    {
        if(m_sInstance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_sInstance =  this.GetComponent<T>();
            if (useDontdestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            m_isValid = true;
            Init();
            m_hasInit = true;
        }
    }

    protected abstract void Init();
}
