using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject _menuPanel = null;
    [SerializeField] GameObject _controlsPanel = null;
    [SerializeField] GameObject _creditsPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        ShowMenu();
    }

    // Update is called once per frame
    void Update()
    {
        //exit level
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitGame();
    }

    public void ShowMenu()
    {
        _menuPanel.SetActive(true);
        HideControls();
        HideCredits();
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("Level01");
    }

    public void ShowControls()
    {
        HideMenu();
        _controlsPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        HideMenu();
        _creditsPanel.SetActive(true);
    }

    public void HideMenu()
    {
        _menuPanel.SetActive(false);
    }

    public void HideControls()
    {
        _controlsPanel.SetActive(false);
    }

    public void HideCredits()
    {
        _creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
