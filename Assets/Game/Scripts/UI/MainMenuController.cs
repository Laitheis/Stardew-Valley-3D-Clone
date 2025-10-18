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
            PlayerData.isPlayerMale = true;
        }
        else
        {
            femaleVisual.SetActive(true);
            maleVisual.SetActive(false);

            _rotate.target = femaleVisual.transform;
            PlayerData.isPlayerMale = false;
        }
    }

    public void StartNewGame()
    {
        SaveDataHolder.instance.saveGuid = System.Guid.NewGuid();
        SaveDataHolder.instance.isFirstLaunch = true;
        SceneManager.LoadScene("Gameplay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
