using System;
using UnityEngine;

public class SaveDataHolder : MonoBehaviour
{
    public static SaveDataHolder instance;
    public Guid saveGuid;
    public bool isFirstLaunch = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}