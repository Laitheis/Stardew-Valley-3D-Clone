using UnityEngine;
using System;

[Serializable]
public class TileState
{
    public CropModel crop;

    public string defCropId;          // link to CropModel.cropId (for saving)
    public Vector3 tilePos;           // tile world position (use integers)
    public int currentStage = 0;      // индекс стадии
    public int daysInStage = 0;       // сколько дней уже в текущей стадии
    public bool wateredToday = false;
    public int wateredStreak = 0;     // подряд политых дней
    public int daysSincePlanted = 0;
    public bool isReadyToHarvest = false;
    public bool isWithered = false;
    public int quality = 0;           // 0-normal,1-silver,2-gold
    public int dryDays = 0;           // дни без полива (если нужно)
    // ссылка на визуальный объект в сцене (не сериализуется)
    [NonSerialized] public GameObject cropVisualInstance;
    [NonSerialized] public GameObject soilVisualInstance;
}
