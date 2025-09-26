using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Farming/CropModel")]
public class CropModel : ScriptableObject
{
    public string cropId;                   // уникальный id (для save/load)
    public string displayName;
    public Sprite sprite;
    public Season[] seasons;                // допустимые сезоны (если пусто — все)
    public int[] daysPerStage;              // e.g. [1,3,2] (кол-во дней на стадии)
    public bool regrows = false;
    public int regrowDays = 0;              // через сколько дней после сбора начнёт регроу
    public bool withersIfNotInSeason = true;
    public bool multiHarvest = false;       // даёт много плодов за сбор (если нужно)
    public GameObject[] stagePrefabs;       // префабы/модели для визуализации стадий, должен совпадать по длине с daysPerStage.Length + возможно ещё 1
    public GameObject witheredPrefab;       // визуализация увядания
    public GameObject worldPrefab;

    [Header("Quality rules")]
    public float baseSilverChance = 0.05f;
    public float baseGoldChance = 0.01f;
    public float waterStreakSilverMultiplier = 0.02f;
    public float waterStreakGoldMultiplier = 0.01f;

    public Vector3 objectOffset;
}

public enum Season { Spring, Summer, Fall, Winter }
