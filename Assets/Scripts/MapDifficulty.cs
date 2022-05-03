using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDifficulty : MonoBehaviour
{
    private Game_Controller m_GameController;
    public Text Difficultytext;
    public GameObject Background;
    private int Generation;

    // Start is called before the first frame update
    void Start()
    {
        m_GameController = FindObjectOfType<Game_Controller>();
    }

    // Update is called once per frame
    void Update()
    {

        if (m_GameController.m_Complete)
        {
            Background.SetActive(true);
            Difficultytext.enabled = true;
            Generation = m_GameController.GetGeneration();
            if (Generation <= 5)
            {
                Difficultytext.text = "This track is easy";
            }
            else if (Generation <= 20)
            {
                Difficultytext.text = "This track is of medium difficulty";
            }
            else
            {
                Difficultytext.text = "This track is of hard difficulty";
            }
        }
    }
}
