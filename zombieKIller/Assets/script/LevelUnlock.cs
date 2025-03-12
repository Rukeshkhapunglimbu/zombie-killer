using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelUnlock : MonoBehaviour
{
    public Button[] levelButtons; // Assign level buttons in the Inspector

    void Start()
    {
        // Ensure LevelUnlocked exists
        if (!PlayerPrefs.HasKey("LevelUnlocked"))
        {
            PlayerPrefs.SetInt("LevelUnlocked", 1);
            PlayerPrefs.Save();
        }

        int unlockedLevel = PlayerPrefs.GetInt("LevelUnlocked");

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i < unlockedLevel);
        }
    }
    public void UnlockNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int highestUnlockedLevel = PlayerPrefs.GetInt("LevelUnlocked");

        // Unlock only if the next level is not already unlocked
        if (highestUnlockedLevel == currentSceneIndex + 1) 
        {
            PlayerPrefs.SetInt("LevelUnlocked", highestUnlockedLevel + 1);
            PlayerPrefs.Save();
            Debug.Log(highestUnlockedLevel);
        }
    }
}
