using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] RotateOnHold _rotate;

    public GameObject maleVisual;
    public GameObject femaleVisual;

    public void SetPlayerName(string name)
    {
        PlayerData.playerName = name;
    }

    public void SetFarmName(string name)
    {
        PlayerData.farmName = name;
    }

    public void TogglePlayerGender()
    {
        PlayerData.isPlayerMale = !PlayerData.isPlayerMale;
        if(PlayerData.isPlayerMale)
        {
            maleVisual.SetActive(true);
            femaleVisual.SetActive(false);

            _rotate.target = maleVisual.transform;
        }
        else
        {
            femaleVisual.SetActive(true);
            maleVisual.SetActive(false);

            _rotate.target = femaleVisual.transform;
        }
    }

    public void StartNewGame()
    {
        SaveDataHolder.instance.saveGuid = System.Guid.NewGuid();
        SaveDataHolder.instance.isFirstLaunch = true;
        SceneManager.LoadScene("Gameplay");
    }

    public void LoadGame()
    {
        //CropManager.SaveData farmSaveData = new();

        //string farmTileData = JsonUtility.ToJson(farmSaveData.cropTiles);
        //List<string> saveContents = new();
        //saveContents.Add();
        //SaveManager.Save();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
