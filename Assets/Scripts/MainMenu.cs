using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject[] CpuLevel;
    public bool isClicked = false;
    public static int choosedLevel;

    public void PlayBtn()
    {
        if (!isClicked)
        {
            for (int i = 0; i < CpuLevel.Length; i++)
            {
                CpuLevel[i].SetActive(true);
            }
            isClicked = true;
        }
        else
        {
            for (int i = 0; i < CpuLevel.Length; i++)
            {
                CpuLevel[i].SetActive(false);
            }
            isClicked = false;
        }
    }
    public void MultiplayerBtn()
    {
        choosedLevel = 0;
        SceneManager.LoadSceneAsync("GamePlay");
    }
    public void ExitBtn()
    {
        Application.Quit();
    }
    public void EasyBtn()
    {
        choosedLevel = 1;
        SceneManager.LoadSceneAsync("GamePlay");
    }
    public void MediumBtn()
    {
        choosedLevel = 2;
        SceneManager.LoadSceneAsync("GamePlay");
    }
    public void HardBtn()
    {
        choosedLevel = 3;
        SceneManager.LoadSceneAsync("GamePlay");
    }
}
