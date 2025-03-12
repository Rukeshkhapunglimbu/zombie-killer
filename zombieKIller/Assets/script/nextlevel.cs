using UnityEngine;
using UnityEngine.SceneManagement;

public class nextlevel : MonoBehaviour
{
    private LevelUnlock levelUnlock;

    void Start()
    {
        levelUnlock = FindObjectOfType<LevelUnlock>(); // Automatically find LevelUnlock script
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Unlock the next level before loading it
        if (levelUnlock != null)
        {
            levelUnlock.UnlockNextLevel();
        }

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Time.timeScale = 1f; // Reset time scale if paused
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }
}

