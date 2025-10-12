using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Farming/DebrisModel")]
public class DebrisModel : ScriptableObject
{
    public string debrisId;
    public GameObject worldPrefab;
    public string lootName;
}