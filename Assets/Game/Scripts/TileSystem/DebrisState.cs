using System;
using UnityEngine;

[Serializable]
public class DebrisState
{
    [NonSerialized] public DebrisModel model;
    public string debrisModelId;
    public Vector3Int tilePos;

    [NonSerialized] public GameObject debrisVisualInstance;
}
