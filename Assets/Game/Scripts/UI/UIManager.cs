using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject PausedGamePanel;
    public GameObject PauseMenu;
    public GameObject ItemsMenu;
    public GameObject StatusMenu;
    public GameObject GameOverMenu;
    public GameObject LevelCompleteMenu;
    public GameObject InteractText;
    public GameObject BossInterfaces;
    public GameObject PotionDetails;

    //public GameObject BleedText;

    //public GameObject PoisonText;

    //public GameObject BurningText;

    //public GameObject FreezeText;

    public static UIManager instance;

    public bool GameIsPaused;
    public bool CanUnpauseGame = true;

    private GameObject ActiveMenu;
    StatusMenu statusMenu;
    ItemsMenu itemsMenu;

    void Awake()
    {
        instance = this;
        statusMenu = GameObject.Find("Canvas").GetComponent<StatusMenu>();
        itemsMenu = GameObject.Find("Canvas").GetComponent<ItemsMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CanUnpauseGame)
        {
            if (!GameIsPaused)
            {
                PauseGame(PauseMenu);
            }
            else
            {
                UnPauseGame(ActiveMenu);
            }
        }
        else if (Input.GetKeyDown(KeyCode.I) && CanUnpauseGame)
        {
            if (!GameIsPaused)
            {
                itemsMenu.UpdatePotionsAmount();
                PauseGame(ItemsMenu);
            }
            else
            {
                UnPauseGame(ActiveMenu);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) && CanUnpauseGame)
        {
            if (!GameIsPaused)
            {
                statusMenu.UpdateStatus();
                PauseGame(StatusMenu);
            }
            else
            {
                UnPauseGame(ActiveMenu);
            }
        }
    }

    //public void SetBleedText(bool state)
    //{
    //    BleedText.SetActive(state);
    //}


    //public void SetPoisonText(bool state)
    //{
    //    PoisonText.SetActive(state);
    //}

    //public void SetBurningText(bool state)
    //{
    //    BurningText.SetActive(state);
    //}


    //public void SetFreezeText(bool state)
    //{
    //    FreezeText.SetActive(state);
    //}

    public void LoadMenu(GameObject ui)
    {
        ActiveMenu = ui;
        ui.SetActive(true);
    }

    public void UnLoadMenu(GameObject ui)
    {
        if (ui.name.Equals("ItemsMenu"))
        {
            PotionDetails.SetActive(false);
        }

        ui.SetActive(false);
    }

    public void PauseGame(GameObject ui)
    {
        PausedGamePanel.SetActive(true);
        ActiveMenu = ui;

        ui.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void UnPauseGame(GameObject ui)
    {
        if (ui.name.Equals("ItemsMenu"))
        {
            PotionDetails.SetActive(false);
        }

        PausedGamePanel.SetActive(false);
        ui.SetActive(false);

        ActiveMenu = null;
        GameIsPaused = false;
        CanUnpauseGame = true;
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        PausedGamePanel.SetActive(true);
        GameOverMenu.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
        CanUnpauseGame = false;
    }

    public void LevelComplete()
    {
        PausedGamePanel.SetActive(true);
        LevelCompleteMenu.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
        CanUnpauseGame = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public IEnumerator StartGameOver()
    {
        yield return new WaitForSeconds(1f);
        GameOver();
    }
}
