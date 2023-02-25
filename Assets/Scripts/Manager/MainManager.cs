using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    private PlayerControls playerControls;
    private GameObject startMenu;
    private GameObject characterSelection;
    private DiceController diceController;
    private AbilityController abilityController;
    private Ludwig ludwig;
    private bool gameStarted = false;
    private string player;
    private GameObject playerGa;
    private GameObject lose;
    private GameObject win;
    private string screen = "start";
    private int currentSelection = 0;
    private List<Button> buttons = new List<Button>();
    private GameObject tutorial;

    private void Awake()
    {
        if(null != Instance) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        /* Pause at beginning */
        // Time.timeScale = 0;

        GameObject controls = GameObject.FindGameObjectWithTag("Controls");
        playerControls = controls.GetComponent<PlayerControls>();

        startMenu = GameObject.FindGameObjectWithTag("StartMenu");

        characterSelection = GameObject.FindGameObjectWithTag("CharacterSelection");
        characterSelection.SetActive(false);

        GameObject dice = GameObject.FindGameObjectWithTag("DiceContainer");
        diceController = dice.GetComponent<DiceController>();

        ludwig = GameObject.FindGameObjectWithTag("Ludwig").GetComponent<Ludwig>();

        /* Select play button */
        GameObject.Find("PlayButton").GetComponent<Button>().Select();

        tutorial = GameObject.Find("Tutorial");
        tutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowCharacterSelection()
    {
        screen = "selection";

        /* Hide start menu */
        startMenu.SetActive(false);

        /* Show player selection */
        characterSelection.SetActive(true);

        /* Select dog button */
        GameObject.Find("DogButton").GetComponent<Button>().Select();
    }


    public void StartGame(string player)
    {
        screen = "game";

        /* Spawn player */
        GameObject playerPrefab = Resources.Load("Players/" + player) as GameObject;
        playerGa = Instantiate(playerPrefab, new Vector3(1.78f, 0.48f, 0), Quaternion.identity);
        playerGa.SetActive(true);

        /* Init dice */
        diceController.Init();

        /* Init controls */
        playerControls.Init();

        /* Init player abilities */
        abilityController = playerGa.GetComponent<AbilityController>();
        abilityController.Init();

        /* Init Ludwig */
        ludwig.Init();

        /* Init enemy spawner */
        EnemySpawner.Instance.Init();

        /* Init camera */
        SmoothCamera smoothCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SmoothCamera>();
        smoothCamera.Init();

        /* Hide player selection */
        characterSelection.SetActive(false);

        gameStarted = true;
    }

    public void GameOver()
    {
        AudioController.Instance.Lose();

        /* Pause Game */
        Time.timeScale = 0;

        /* Show lose screen */
        GameObject losePrefab = Resources.Load("UI/Lose") as GameObject;
        lose = Instantiate(losePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        /* Reset game */
        ResetGame();

        /* Setup try again button */
        Button tryAgainButton = lose.GetComponentInChildren<Button>();
        tryAgainButton.Select();
        tryAgainButton.onClick.AddListener(() => TryAgain());
    }

    public void Win()
    {
        AudioController.Instance.Win();

        /* Pause Game */
        Time.timeScale = 0;

        /* Show win screen */
        GameObject winPrefab = Resources.Load("UI/Win") as GameObject;
        win = Instantiate(winPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        /* Reset game */
        ResetGame();

        /* Setup play again button */
        Button playAgainButton = win.GetComponentInChildren<Button>();
        playAgainButton.Select();
        playAgainButton.onClick.AddListener(() => TryAgain());
    }

    private void ResetGame()
    {
        gameStarted = false;

        if(playerGa) {
            /* Set player position to reset camera on coots */
            playerGa.transform.position = new Vector3(0.51f, 0, 0);
            
            /* Hide player */
            playerGa.SetActive(false);
        }

        /* Remove enemies */
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            Destroy(enemy);
        }

        /* Add 3 Schleimis */
        GameObject schleimiPrefab1 = Resources.Load("Enemies/Schleimi") as GameObject;
        GameObject schleimiPrefab2 = Resources.Load("Enemies/Schleimiblau") as GameObject;
        GameObject schleimiPrefab3 = Resources.Load("Enemies/Schleimirot") as GameObject;
        GameObject schleimi1 = Instantiate(schleimiPrefab1, new Vector3(4.95f, 4.05f, 0), Quaternion.identity) as GameObject;
        GameObject schleimi2 = Instantiate(schleimiPrefab2, new Vector3(7.71f, -3.9f, 0), Quaternion.identity) as GameObject;
        GameObject schleimi3 = Instantiate(schleimiPrefab3, new Vector3(-6.72f, 1.36f, 0), Quaternion.identity) as GameObject;

        /* Reset Ludwig by respawning */
        GameObject ludwigGa = GameObject.FindGameObjectWithTag("Ludwig");
        Destroy(ludwigGa);
        GameObject ludwigPrefab = Resources.Load("Enemies/Ludwig") as GameObject;
        ludwigGa = Instantiate(ludwigPrefab, new Vector3(-7.01f, -5.85f, 0), Quaternion.identity) as GameObject;
        ludwig = ludwigGa.GetComponent<Ludwig>();

        /* Reset Dice + Abilities */
        if(abilityController) {
            abilityController.ResetAbilities();
        }
        diceController.Reset();

        /* Reset coots lives */
        GameObject coots = GameObject.FindGameObjectWithTag("Coots");
        coots.GetComponent<CootsHealth>().Reset();

        /* Remove Turds */
        GameObject[] turds = GameObject.FindGameObjectsWithTag("Turd");
        foreach(GameObject turd in turds) {
            Destroy(turd);
        }

        playerControls.Reset();

        EnemySpawner.Instance.Reset();
    }

    private void TryAgain()
    {
        Destroy(lose);
        Destroy(win);

        /* Unpause game */
        Time.timeScale = 1;

        ShowCharacterSelection();
    }

    public bool HasGameStarted()
    {
        return gameStarted;
    }

    public void Quit()
    {
        if("start" == screen) {
            Debug.Log("QUIT GAME");
            Application.Quit();
        }
        else {
            ResetGame();

            Destroy(lose);
            Destroy(win);

            characterSelection.SetActive(false);
            startMenu.SetActive(true);
            GameObject.Find("PlayButton").GetComponent<Button>().Select();

            screen = "start";

            /* Unpause game */
            Time.timeScale = 1;
        }
    }

    public void ShowTutorial()
    {
        tutorial.SetActive(true);
        GameObject.Find("CloseTutorialButton").GetComponent<Button>().Select();
    }

    public void HideTutorial()
    {
        tutorial.SetActive(false);
        GameObject.Find("PlayButton").GetComponent<Button>().Select();
    }
}
