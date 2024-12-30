using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class GameController : MonoBehaviour

{
    public static GameController instance;
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
    public AudioSource wrongtSound;
    [SerializeField]
    public AudioClip Right, Wrong;
 



    private bool firstGuess = true;
    private bool secondGuess = true;
    private int countGuesses;
    private int countCorrectGuesses;
    public int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;
    public Text bestscoretxt;

    int bestscore = 0;

    private float timer = 40f; // Timer duration (60 seconds)
    private bool gameOver = false;
    void Awake()
    {
        Puzzles = Resources.LoadAll<Sprite>("assets/96dpi");
        instance = this;
    }

    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzels();
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
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        foreach (var obj in objects)
        {
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                btns.Add(button);
                if (bgImage != null)
                {
                    Debug.Log($"Setting bgImage for button: {obj.name}");
                    button.image.sprite = bgImage;
                }
                else
                {
                    Debug.LogWarning($"bgImage is null when setting button: {obj.name}");
                }
            }
        }
    }


    void AddGamePuzzels()
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

        int selectedIndex = int.Parse(selectedObject.name);

        if (firstGuess)
        {
            firstGuess = false;
            firstGuessIndex = selectedIndex;
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (secondGuess)
        {
            if (selectedIndex == firstGuessIndex) return; // Avoid selecting the same button
            secondGuess = false;
            secondGuessIndex = selectedIndex;
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

            StartCoroutine(CheckIfThePuzzlesMatch());
        }
    }

    IEnumerator CheckIfThePuzzlesMatch()
    {
        //yield return new WaitForSeconds(1f);
        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(.2f);
            // Disable interaction with the matched puzzle pieces
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
            wrongtSound.clip = Wrong;
            wrongtSound.Play();
            yield return new WaitForSeconds(.2f);
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }

        yield return new WaitForSeconds(.2f);
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

        SceneManager.LoadScene("into");
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
            //store the sprite at index i in a temporary variable
            Sprite temp = list[i];
            // Generate a random index between i (inclusive) and the count of the list (exclusive)
            int randomIndex = UnityEngine.Random.Range(i, list.Count);

            // Swap the sprite at index i with the sprite at the random index
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}