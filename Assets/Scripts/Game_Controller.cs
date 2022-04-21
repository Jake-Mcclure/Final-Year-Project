using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour
{
    
    public Text BestTime;
    public Text CurrentTime;

    private float m_time = 0.0f;
    private float m_bestTime = 0.0f;
    private string m_txtbesttime;
    private string m_txtcurrenttime;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        m_time += Time.deltaTime;
        CurrentTime.text = m_time.ToString();
    }

    public void LapComplete()
    {

    }

}
