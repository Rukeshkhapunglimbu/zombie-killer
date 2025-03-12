using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public int totalZombies;
    public int bulletsUsed = 0;
    public int maxBullets = 5;
    public bool isGameOver;
    // public int bulletCount = 5;
    public bool isGamePaused { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] public GameObject defeatPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    public int bulletCount;  // This should already exist in GameManager

    [SerializeField] private TMP_Text bulletText;
    private bool hasUsedRewardAd = false; // Track if ad was used
    public GameObject watchAdButton; // Assign this in the Inspector

    [Header("Audio")]
    [SerializeField] private AudioSource bulletFireSound;
    public GoogleAds ads;
    private void Start()
    {
        InitializeGame();
        ads.LoadRewardedAd();
        ads.LoadInterstitialAd();
    }

    private void InitializeGame()
    {
        isGameOver = false;
        isGamePaused = false;
        bulletsUsed = 0;
        Time.timeScale = 1f;

        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);

        UpdateBulletUI();
    }

    public void OnZombieKilled()
    {
        if (isGameOver) return;

        totalZombies--;
        CheckGameStatus();
    }

    public void OnBulletFired()
    {
        if (isGameOver || isGamePaused || bulletsUsed >= maxBullets) return;

        bulletsUsed++;
        UpdateBulletUI();

        if (bulletFireSound != null)
        {
            bulletFireSound.Play();
        }

        CheckGameStatus();
    }


    public void UpdateBulletUI()
    {
        if (bulletText != null)
        {
            // bulletText.text = $"Bullets: {bulletCount - bulletsUsed}";
            bulletText.text = $"Bullets: {Mathf.Max(0, maxBullets - bulletsUsed)}";

        }
    }

    private void CheckGameStatus()
    {
        if (totalZombies <= 0)
        {
            ShowVictoryPanel();
        }
        else if (bulletsUsed >= maxBullets)
        {
            StopAllCoroutines();
            // Only start the coroutine to show the defeat panel after all bullets are gone
            StartCoroutine(WaitForBulletDestructionAndShowDefeatPanel());
        }
    }
    public IEnumerator ShowDefeatPanelAfterBulletsGone()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("bullet").Length == 0);

        if (totalZombies > 0)
        {
            ShowDefeatPanel();
        }
    }
    // public IEnumerator WaitForBulletDestructionAndShowDefeatPanel()
    // {
    //     // Wait until all bullets are destroyed (i.e., no bullets left in the scene)
    //     yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("bullet").Length == 0);

    //     // After all bullets are destroyed, show the defeat panel
    //     ShowDefeatPanel();
    // }
    private IEnumerator WaitForBulletDestructionAndShowDefeatPanel()
{
    yield return new WaitForSeconds(0.5f); // Short delay to ensure bullets are properly detected

    // Wait until all bullets are destroyed
    while (GameObject.FindGameObjectsWithTag("bullet").Length > 0)
    {
        yield return null; // Wait for next frame and check again
    }

    ShowDefeatPanel();
}

    public void ShowVictoryPanel()
    {
        if (bulletFireSound) bulletFireSound.Stop();
        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
        }
        // if (victoryPanel) victoryPanel.SetActive(true);
        // // Show Interstitial Ad after victory
        ads.LoadInterstitialAd();
        ads.ShowInterstitialAd();
        DisablePauseButton(); 

        EndGame();
        UnlockNextLevel();
    }

    public void ShowDefeatPanel()
    {
        if (bulletFireSound) bulletFireSound.Stop();
        if (defeatPanel) defeatPanel.SetActive(true);

        EndGame();
    }

    public void HideDefeatPanel()
    {
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false); // Hides the panel
        }
    }

    private void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        DisablePauseButton();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("levelmenu");
    }

    public void PauseGame()
    {
        if (isGameOver) return;

        if (pausePanel) pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (defeatPanel) defeatPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        isGamePaused = false;
        StartCoroutine(ClearInput());
    }

    private IEnumerator ClearInput()
    {
        yield return new WaitForEndOfFrame();
        Input.ResetInputAxes();
        yield return new WaitForSeconds(0.1f);
    }

    private void DisablePauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
    }

    private void UnlockNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int unlockedLevel = PlayerPrefs.GetInt("LevelUnlocked", 1);
        Debug.Log(currentLevel);

        if (currentLevel + 1 > unlockedLevel && currentLevel <= 19)
        {
            PlayerPrefs.SetInt("LevelUnlocked", currentLevel);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void AddExtraBullet()
    {
        Debug.Log("Adding Extra Bullet!");

        bulletCount++;  // ✅ Increase bullet count
        bulletsUsed--;  // ✅ Reduce used bullets (to allow firing)

        UpdateBulletUI();  // ✅ Update UI
        ResumeGame();      // ✅ Resume game if paused
                           // If bullets finish again, show defeat panel but no Watch Ad button
        if (bulletCount <= 0)
        {
            ShowDefeatPanel();
        }
    }

    public void OnWatchAdButtonPressed()
    {
        // If ad is not used, show the ad
        if (!hasUsedRewardAd)
        {
            ads.ShowRewardedAd(); // Show the rewarded ad
            hasUsedRewardAd = true; // Mark that the ad has been used
            // watchAdButton.SetActive(false); // Disable the button after use
        }
    }
    public int GetBulletCount()
    {
        return bulletCount;  // Make sure 'bulletCount' is a variable in GameManager
    }

}


