using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelmanager : MonoBehaviour
{
   public GameObject backButton;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


   public void LoadMenuScene()
    {
        SceneManager.LoadScene("menu"); // Change "MenuScene" to your actual menu scene name
    }



}
