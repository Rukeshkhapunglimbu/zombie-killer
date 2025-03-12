using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menuscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("levelmenu");
    }

    public void ShowAdGame()
    {
        SceneManager.LoadSceneAsync("ADS");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ExitGame()
    {
        Debug.Log("Exiting game..."); // Log for debugging
        Application.Quit(); // Exit the game

        // If you're in the editor, you can stop play mode by using this:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

   

}
