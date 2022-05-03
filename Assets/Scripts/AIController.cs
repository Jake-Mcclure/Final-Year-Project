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
    public string m_Information;

    public int m_Lap;

    bool IsActive = true;

    // Start is called before the first frame update
    void Start()
    {
        m_Lap = 1;
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

        m_Newtwork.InitialiseWeights(new float[] {3.392318f,1.837881f,-1.975526f,1.753359f,-4.294175f,-2.293187f,-3.118063f,3.982157f,1.886362f,-3.332502f,2.589881f,-0.5650082f,-0.7998129f,-2.323318f,0.3580253f,0.820689f,2.878132f,2.043788f,3.468808f,-3.762825f,-2.515255f,0.999253f,-2.823804f,-3.577962f,3.483124f,-3.816596f,4.26746f,-1.312109f,-0.04019344f});
        //m_Newtwork.InitialiseWeights(new float[] {2.680655f, 1.45462f, -2.919403f, 0.9514958f, -4.532198f, -2.111656f, -2.388326f, 3.16306f, 0.9624192f, -3.405635f, 3.510265f, -1.041404f, 0.1047662f, -2.852448f, -0.06707132f, 1.326346f, 3.550567f, 2.365993f, 2.645675f, -3.523063f, -2.591178f, 0.3205468f, -2.986468f, -3.395661f, 3.26246f, -4.475611f, 3.515121f, -0.5581498f, -0.78376f });
        tag = "Alive";
    }

    // Update is called once per frame
    void Update()
    {
        m_Information = m_Newtwork.ReadBrain();
        if (!m_Gamecontroller.m_Complete)
        {
            //still running
            if (IsActive)
            {
                m_RigidBody.MovePosition(transform.position + transform.forward * (Time.deltaTime * 40));
            }
        }
        //rotate left or right depending on the output
        transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * Turnamount * 3f);

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

        if(IsActive)
        {
            Turnamount = m_Newtwork.CalculateOutput(Inputs);
            m_Ability += CalculateAbility(Inputs);
        }
        else
        {
            Turnamount = 0;
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

        return (basevalue/1000) * m_Lap;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall")
        {
            Stop();
        }
        else if (other.tag == "LapEnd")
        {
            m_Lap++;
            if(m_Lap > m_Gamecontroller.MaxLaps)
            {
                m_Gamecontroller.m_Complete = true;
            }
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
        Debug.Log("check if last called");
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Alive");

        if (temp.Length == 0)
        {
            Debug.Log("calling new generation from check if last");
            m_Gamecontroller.NewGeneration();
        }
    }

    public void SetInformation(float[] inf)
    {
        //float[] initial;
    //Debug.Log("car information is " + inf);
        float[] initial = inf;
        m_Newtwork.InitialiseWeights(initial);
    }
}
