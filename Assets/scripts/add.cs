using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class add : MonoBehaviour

{
    [SerializeField]
    private Transform PuzzleGame;
    [SerializeField]
    private GameObject btn1;
    // Update is called once per frame
        void Awake()
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject button = Instantiate(btn1);
                button.name = "" + i;
                button.transform.SetParent(PuzzleGame, false);

            }
        }
}