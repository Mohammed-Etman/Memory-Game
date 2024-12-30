using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cntrole : MonoBehaviour
{
    public static Cntrole Instance;
    [SerializeField]
    private Sprite bgImage;
    [SerializeField] 
    private Text timerText; // Text element to show the timer

    public Sprite[] Puzzles;
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    [SerializeField]
    public AudioSource rightSound;
    [SerializeField]
    public AudioSource wrongSound;
    [SerializeField]
    private AudioClip Right, Wrong;



    private bool firstGuess = true;
    private bool secondGuess = true;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex;
    private int secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;
    public Text bestscoretxt;

    int bestscore = 0;

    private float timer = 20f; // Timer duration (20 seconds)
    private bool gameOver = false;


    void Awake()
    {
        Puzzles = Resources.LoadAll<Sprite>("assets/96dpi");
        Instance = this;

    }

    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
        bestscore = gameGuesses;
        bestscoretxt.text = "Best Score: " + bestscore.ToString();
        UpdateTimerUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (!gameOver)
        {
            timer -= Time.deltaTime; // Decrease the timer
            UpdateTimerUI();

            if (timer <= 0)
            {
                GameOver();
            }
        }
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Button");
        foreach (var obj in objects)
        {
            Button button = obj.GetComponent<Button>();
            if (!btns.Contains(button))
            {
                btns.Add(button);
                button.image.sprite = bgImage;
            }
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;
        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
            {
                index = 0;
            }
            gamePuzzles.Add(Puzzles[index]);
            index++;
        }
    }

    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickPuzzle());
        }
    }

    public void PickPuzzle()
    {
        var selectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (selectedObject == null) return;

        // Get the currently selected button's index
        int selectedIndex = int.Parse(selectedObject.name);

        // Check if the button is interactable before proceeding
        if (!btns[selectedIndex].interactable) return;

        if (firstGuess)
        {
            firstGuess = false;
            firstGuessIndex = selectedIndex;
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (secondGuess)
        {
            if (selectedIndex == firstGuessIndex) return; // Prevent selecting the same button twice
            secondGuess = false;
            secondGuessIndex = selectedIndex;
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

            StartCoroutine(CheckIfThePuzzlesMatch());
        }
    }

    IEnumerator CheckIfThePuzzlesMatch()
    {
        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(.3f);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;
            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            rightSound.clip = Right;
            rightSound.Play();
            Score.Instance.UpdateScoreUI();

            CheckIfTheGameIsFinished();
        }
        else
        {
            wrongSound.clip = Wrong;
            wrongSound.Play();
            yield return new WaitForSeconds(0.5f);
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }

        yield return new WaitForSeconds(0.5f);
        firstGuess = secondGuess = true;
    }

    void CheckIfTheGameIsFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses == gameGuesses)
        {
            NextLevel();
        }
    }

    void NextLevel()
    {
        gameOver = true; // Stop the timer

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void GameOver()
    {
        gameOver = true;
        SceneManager.LoadScene("GameOver"); // Replace "GameOver" with the actual scene name
    }

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.Ceil(timer).ToString(); // Update UI text
    }



    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
