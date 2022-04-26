using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public TheNetwork m_Newtwork = new TheNetwork(5,4);
    private Game_Controller m_Gamecontroller;

    Rigidbody m_RigidBody;
    Collider m_Collider;
    public float Turnamount;
    public float[] Inputs;
    public float m_Ability;
    private float[] initial;
    public string m_Information;

    int m_Speed = 20;
    bool IsActive = true;

    // Start is called before the first frame update
    void Start()
    {
        if(!(m_Gamecontroller = Camera.main.GetComponent<Game_Controller>()))
        {
            Debug.Log("Cant find controller");
        }

        //Get Collider
        m_Collider = GetComponent<BoxCollider>();

        //Get RigidBody
        m_RigidBody = GetComponent<Rigidbody>();

        //set constraints for collider
        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;

        m_Newtwork.InitialiseWeights(new float[] { 3.472079f, 1.762525f, -2.266208f, 0.8920379f, -3.915989f, -1.762377f, -2.844904f, 3.381477f, 1.12464f, -3.086241f, 3.320154f, 0.1941123f, 0.1791953f, -3.122393f, 0.8971314f, 0.1158746f, 3.512217f, 1.440832f, 3.3429f, -3.377463f, -2.171291f, 1.523072f, -2.242229f, -2.650826f, 3.01321f, -3.341551f, 3.746894f, -1.755286f, -0.3875917f });

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
        transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * Turnamount * 2.5f);

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

        if(IsActive)
        {
            Turnamount = m_Newtwork.CalculateOutput(Inputs);
        }

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
        Debug.Log("car information is " + inf);
        initial = inf;
        m_Newtwork.InitialiseWeights(initial);
    }
}
