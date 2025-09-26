using UnityEngine;
using System;

[Serializable]
public class TileState
{
    public CropModel crop;

    public string cropModelId;          // link to CropModel.cropId (for saving)
    public Vector3 tilePos;           // tile world position (use integers)
    public int currentStage = 0;      // stage index
    public int daysInStage = 0;       // how many days has been in the current stage
    public bool wateredToday = false;
    public int daysSincePlanted = 0;
    public bool isReadyToHarvest = false;
    public bool isWithered = false;
    public int quality = 0;           // 0-normal,1-silver,2-gold
    public int dryDays = 0;
    // Scene visual (not serialized)
    [NonSerialized] public GameObject cropVisualInstance;
    [NonSerialized] public GameObject soilVisualInstance;
}
