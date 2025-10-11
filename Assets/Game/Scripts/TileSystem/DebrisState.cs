using UnityEngine;
using System;

[Serializable]
public class DebrisState
{
    public string debrisModelId;
    public Vector3Int tilePos;
    // Scene visual (not serialized)
    [NonSerialized] public GameObject debrisVisualInstance;
}
