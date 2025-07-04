using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameManagerManager gameManageManager;
    
    [Header("Panels")]
    [SerializeField] private GameObject titleScreenPanel;
    [SerializeField] private GameObject mainMenuScreenPanel;
    [SerializeField] private GameObject optionsScreenPanel;
    [SerializeField] private GameObject pauseMenuScreenPanel;
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private GameObject playGamePanel;
    [SerializeField] private GameObject GameScreenPanel;


    [Header("Buttons & sliders")]
    [SerializeField] private Slider volume;
    [SerializeField] private TMP_Text volumeValue;
    [SerializeField] private GameObject pressAnyKey;
    [SerializeField] private GameObject optionsKey;
    [SerializeField] private GameObject backToMainMenu;

    private bool hasPressedAnyKey;

    private void Awake()
    {
        TitleScreen();
    }

   

    private void Update()
    {
        if (!hasPressedAnyKey && Input.anyKeyDown)
        {
            hasPressedAnyKey = true;
            Debug.Log($"A mouse click or a key press was detected Update");
            TitleScreen();
            ShowMainMenu();
        }
            VolumeValue();
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenuScreenPanel.activeInHierarchy && !endScreenPanel.activeInHierarchy)
            {
                PauseGame();
            }
    }

    public void PlayGame()
    {
        playGamePanel.SetActive(true);
        mainMenuScreenPanel.SetActive(false);
        endScreenPanel.SetActive(false);
        pauseMenuScreenPanel.SetActive(false);
        gameManageManager.InitialiseGame();

    }
    public void TitleScreen()
    {
        

        titleScreenPanel.SetActive(true);

        mainMenuScreenPanel.SetActive(false);

        optionsScreenPanel.SetActive(false);

        pauseMenuScreenPanel.SetActive(false);

        endScreenPanel.SetActive(false);

        playGamePanel.SetActive(false);


    }

    public void ShowMainMenu()
    {
        
        titleScreenPanel.SetActive(false);

        mainMenuScreenPanel.SetActive(true);

        optionsScreenPanel.SetActive(false);

        pauseMenuScreenPanel.SetActive(false);

        endScreenPanel.SetActive(false);
    }

    
    public void ShowOptionsMenu()
    {
        titleScreenPanel.SetActive(false);

        mainMenuScreenPanel.SetActive(false);

        optionsScreenPanel.SetActive(false);

        pauseMenuScreenPanel.SetActive(false);

        endScreenPanel.SetActive(true);
    }

    public void VolumeValue()
    {
        volume.wholeNumbers = true;
        volumeValue.text = $"{volume.value}%";
    }
    private void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            pauseMenuScreenPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenuScreenPanel.SetActive(false);
        }

        mainMenuScreenPanel.SetActive(false);
        endScreenPanel.SetActive(false);
    }

    private void LooseScenario()
    {
        titleScreenPanel.SetActive(false);

        mainMenuScreenPanel.SetActive(true);

        optionsScreenPanel.SetActive(false);

        pauseMenuScreenPanel.SetActive(false);

        endScreenPanel.SetActive(true);

    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log($"Application has quit");
    }

    public void HasPressedKeyDetection()
    {
        hasPressedAnyKey = false;
        TitleScreen();
    }
   

}
