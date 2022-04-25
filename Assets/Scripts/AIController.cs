using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public TheNetwork m_Newtwork;
    private Game_Controller m_Gamecontroller;

    Rigidbody m_RigidBody;
    Collider m_Collider;
    public float Turnamount;
    public float[] Inputs;
    public float m_Ability;
    private float[] initial;
    public string m_Information;

    int m_Speed = 8;
    bool IsActive = true;

    // Start is called before the first frame update
    void Start()
    {
        m_Gamecontroller = Camera.main.GetComponent<Game_Controller>();

        m_Newtwork = new TheNetwork(5,4);

        //Get Collider
        m_Collider = GetComponent<BoxCollider>();

        //Get RigidBody
        m_RigidBody = GetComponent<Rigidbody>();

        //set constraints for collider
        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;

        tag = "Alive";
    }

    // Update is called once per frame
    void Update()
    {
        m_Information = m_Newtwork.ReadBrain();

        //still running
        if(IsActive)
        {
            m_RigidBody.MovePosition(transform.position+transform.forward * (Time.deltaTime * m_Speed));
        }

        //rotate left or right depending on the output
        transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, Turnamount * 2.0f, 0));

        //this sets up the rays which will later find the distance from the car to the edge of the track
        //in each direction from left to right left, front left, forward, front right, right
        Vector3[] inputrays = new Vector3[]
        {
            transform.TransformDirection(Vector3.left),
            transform.TransformDirection(Vector3.left + Vector3.forward),
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(Vector3.right+Vector3.forward),
            transform.TransformDirection(Vector3.right)
        };

        //this ceates an array of the 5 inputs and sets the values of each index to the correspnding distance from the car to the edge of the track
        //it checks if the raycast hits itsself and if it hits nothing, essentialy checkign it hits the wall
        Inputs = new float[inputrays.Length];

        RaycastHit Hit;

        for(int i = 0;i < inputrays.Length;i++)
        {
            Debug.DrawRay(transform.position,inputrays[i] * 10,Color.blue);
            if(Physics.Raycast(transform.position,inputrays[i],out Hit))
            {
                if(Hit.collider != m_Collider && Hit.collider != null)
                {
                    Inputs[i] = Hit.distance;
                }
            }
            
        }

        m_Ability += CalculateAbility(Inputs);
    }
    //this calculates the ability of the AI, which will constantly increase until it is dead
    //
    float CalculateAbility(float[] inputarray)
    {
        float basevalue = 0;

        for (int i = 0; i < inputarray.Length; i++)
        {
            basevalue += inputarray[i];
        }

        return (basevalue/50);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall")
        {
            Stop();
        }
    }

    public void Stop()
    {
        IsActive = false;
        tag = "Dead";
        m_Newtwork.SetAbility(m_Ability);
        m_Gamecontroller.m_AllAICont.Remove(this);
        CheckIfLast();

    }

    public void Reset()
    {
        m_Ability = 0;
        m_Newtwork.SetAbility(m_Ability);
        tag = "Alive";
        IsActive = true;

    }

    void CheckIfLast()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Alive");

        if (temp.Length == 0)
        {
            m_Gamecontroller.NewGeneration();
        }
    }

    public void SetInformation(float[] inf)
    {
        initial = inf;
        m_Newtwork.InitialiseWeights(initial);
    }
}
