using System;
using UnityEngine;


public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator instance;

    public DefinitionDatabase definitionDatabase;

    internal void Init()
    {
        instance = this;
    }
}