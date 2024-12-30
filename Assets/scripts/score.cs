using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score Instance;
    public Text scoretxt;

    int score = 0; // Corrected variable name to match the property being used

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //scoretxt.text = "Score: " + score.ToString() +"/"+gameGuesses; // Use 'score', not 'Score'
        

        scoretxt.text = "Score: " + score.ToString();  // Use 'score', not 'Score'
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateScoreUI()
    {
        score += 1;

        scoretxt.text = "Score: " + score.ToString(); // Use 'score', not 'Score'

    }
}
