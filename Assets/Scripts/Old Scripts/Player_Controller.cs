using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    //Game_Controller m_gameController;

    Rigidbody m_Rigidbody;

    public float m_TopSpeed = 20.0f;
    public float m_Acceleration = 5.0f;
    public int m_TurnSpeed = 100;
    private float m_Speed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //m_gameController = FindObjectOfType<Game_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform trans = transform;
        if (Input.GetKey(KeyCode.W))
        {
            
            if (m_Speed < m_TopSpeed)
            {
                m_Speed += m_Acceleration * Time.deltaTime;
            }
            m_Rigidbody.velocity = transform.forward * m_Speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(0.0f, -m_TurnSpeed * Time.deltaTime, 0.0f, Space.Self);

        }
        if (Input.GetKey(KeyCode.S))
        {
            
            if (m_Speed > -m_TopSpeed)
            {
                m_Speed -= m_Acceleration * Time.deltaTime;
            }
            m_Rigidbody.velocity = -transform.forward * m_Speed;

        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f,m_TurnSpeed * Time.deltaTime,0.0f,Space.Self);

        }

    }


    
}
