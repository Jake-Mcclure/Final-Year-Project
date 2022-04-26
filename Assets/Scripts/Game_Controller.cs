using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Method {Random, Slice}

public class Game_Controller : MonoBehaviour
{

    private List<AIController> m_CurrentAICont = new List<AIController>();
    public List<AIController> m_AllAICont = new List<AIController>();
    private AIController m_AICont;

    public Method m_method= Method.Slice;

    public GameObject m_Car,m_StartPos;
    private float[][] m_InitialData;
    private string[] m_InitialString;

    public List<string> LoadedInformation;

    private float m_MutationRate = 1.0f;
    private float m_MutationProbability = 0.75f;

    private List<float> m_Abilities;
    private TheNetwork m_BestAbility = new TheNetwork();

    private int Generation = 0;
    public int Attempts = 12;

    private void Start()
    {
        Debug.Log("Started Game Controller");
        
        m_InitialData = new float[Attempts][];
        m_InitialString = new string[Attempts];
        for (int i = 0; i < m_InitialData.Length; i++)
        {
            m_InitialData[i] = new float[29];
        }
        Debug.Log(Attempts + " " + m_InitialData.Length);
        RandomDay();
        NewGeneration();
    }

    private void Update()
    {

        bool IsActive = m_AllAICont.Count > 0;

        if (IsActive)
        {

        }

    }

    void RandomDay()
    {
        Debug.Log("RandomDay called");
        for (int i = 0; i < m_InitialData.Length; i++)
        {
            for (int x = 0; x < m_InitialData[i].Length; x++)
            {
                m_InitialData[i][x] = Random.Range(-4, 4);
            }
        }
    }

    public void SpawnAi(int amount)
    {
        Debug.Log("Spawn AI called");
        for (int i = 0; i < amount; i++)
        {
            AIController car = Instantiate(m_Car).GetComponent<AIController>();

            car.transform.position = new Vector3(m_StartPos.transform.position.x, 1, m_StartPos.transform.position.z);

            car.transform.eulerAngles = new Vector3(0, 0, 0);

            car.SetInformation(m_InitialData[i]);

            m_AllAICont.Add(car);
        }

        m_CurrentAICont.Clear();

        for (int i = 0; i < m_AllAICont.Count; i++)
        {
            m_CurrentAICont.Add(m_AllAICont[i]);
        }
    }
    
    public void RestartAI(int amount)
    {
        Debug.Log("Restart AI called");
        GameObject[] deadAI = GameObject.FindGameObjectsWithTag("Dead");

        for (int i = 0; i < amount; i++)
        {
            AIController car = deadAI[i].GetComponent<AIController>();

            car.transform.position = new Vector3(m_StartPos.transform.position.x, 1, m_StartPos.transform.position.z);

            car.transform.eulerAngles = new Vector3(0, 0, 0);

            car.SetInformation(m_InitialData[i]);

            m_AllAICont.Add(car);
        }

        m_CurrentAICont.Clear();

        m_CurrentAICont = m_AllAICont;
    }

    private void Learn()
    {
        GameObject[] deadcars = GameObject.FindGameObjectsWithTag("Dead");

        List<TheNetwork> allnetworks = new List<TheNetwork>();

        for (int i = 0; i < deadcars.Length; i++)
        {
            allnetworks.Add(deadcars[i].GetComponent<AIController>().m_Newtwork);
        }


        switch (m_method)
        {
            case Method.Random:
                RandomInteraction(allnetworks);
                break;
            case Method.Slice:
                SlicedInteraction(allnetworks);
                break;
            default:
                break;
        }
    }    

    private void RandomInteraction(List<TheNetwork> allnetworks)
    {
        TheNetwork[] parents = new TheNetwork[2];

        parents = GetParents(allnetworks);

        for (int i = 0; i < m_InitialData.Length; i++)
        {

            if (i > m_InitialData.Length - 3)
            {
                RandomizeParentWeights(parents);
            }

            for (int x = 0; x < m_InitialData[i].Length; x++)
            {
                int fifty = Random.Range(0, parents.Length);

                float mutation = Random.value;

                if (mutation < m_MutationProbability)
                {
                    m_InitialData[i][x] = parents[fifty].GetBrain()[x] + Random.Range(-m_MutationRate,m_MutationRate);
                }
                else
                {
                    m_InitialData[i][x] = parents[fifty].GetBrain()[x];
                }

            }
        }
    }

    private void SlicedInteraction(List<TheNetwork> allnewtworks)
    {
        TheNetwork[] parents = new TheNetwork[2];

        parents = GetParents(allnewtworks);

        for (int i = 0; i < m_InitialString.Length; i++)
        {
            if(i > m_InitialString.Length -3)
            {
                RandomizeParentWeights(parents);
            }

            m_InitialString[i] = MakeChildInfo(parents);
        }

        for (int i = 0; i < m_InitialData.Length; i++)
        {
            for (int x = 0; x < m_InitialData[i].Length; x++)
            {
                m_InitialData[i][x] = float.Parse(m_InitialString[i].Split(',')[x]);
            }
        }

    }

    private string MakeChildInfo(TheNetwork[] parents)
    {
        int start = Random.Range(0, 29);
        int stop = Random.Range(start, 29);

        string childinfo = "";

        for (int i = 0; i < start; i++)
        {
            childinfo += parents[0].ReadBrain().Split(',')[i] + ",";
        }

        for (int i = start; i < stop; i++)
        {
            childinfo += parents[1].ReadBrain().Split(',')[i] + ",";
        }

        for (int i = stop; i < 29; i++)
        {
            if (i != 28)
            {
                childinfo += parents[0].ReadBrain().Split(',')[i] + ",";
            }
            else
            {
                childinfo += parents[0].ReadBrain().Split(',')[i] + string.Empty;
            }
        }

        string newchildinfo = "";

        for (int i = 0; i < childinfo.Split(',').Length; i++)
        {
            float mutation = Random.value;

            if (mutation < m_MutationProbability && i != childinfo.Split(',').Length -1)
            {
                newchildinfo += (float.Parse(childinfo.Split(',')[i])) + Random.Range(-m_MutationRate, m_MutationRate) + ',';
            }
        }

        return newchildinfo;
    }

    private TheNetwork[] GetParents(List<TheNetwork> allnetworks)
    {
        TheNetwork[] parents = new TheNetwork[2];

        parents[0] = allnetworks[GetBest(allnetworks)];

        if (parents[0].m_Ability > m_BestAbility.m_Ability)
        {
            m_BestAbility = parents[0];
        }

        if (allnetworks.Count > 1)
        {
            allnetworks.Remove(allnetworks[GetBest(allnetworks)]);
        }

        parents[1] = allnetworks[GetBest(allnetworks)];
        return parents;
    }

    private int GetBest(List<TheNetwork> allnetworks)
    {
        Debug.Log("Get Best called");
        double best = 0;
        int id = 0;

        for (int i = 0; i < allnetworks.Count; i++)
        {
            if (allnetworks[i].m_Ability > best)
            {
                id = i;
                best = allnetworks[i].m_Ability;
            }
        }

        return id;
    }

    private int GetBest(List<AIController> allnetworks)
    {
        Debug.Log("Get Best called " + allnetworks.Count);
        float best = 0;
        int id = 0;

        for (int i = 0; i < allnetworks.Count; i++)
        {
            if (allnetworks[i].m_Ability > best)
            {
                id = i;
                best = allnetworks[i].m_Ability;
            }
        }

        Debug.Log("getbest ended id returned is " + id);
        return id;
    }

    private void RandomizeParentWeights(TheNetwork[] parents)
    {
        Debug.Log("randomizing weights");
        float[] temp = new float[29];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = Random.Range(-4.0f, 4.0f);
        }

        int fifty = Random.Range(0, 1);

        parents[fifty].InitialiseWeights(temp);
    }

    public void NewGeneration()
    {
        Debug.Log("NewGen called");
        if (Generation > 0)
        {
            SetBest();
            Learn();
            RestartAI(Attempts);
        }
        else
        {
            SpawnAi(Attempts);
        }

        Generation++;
    }

    private void SetBest()
    {
        Debug.Log("Set Best called");
        AIController temp = m_CurrentAICont[GetBest(m_CurrentAICont)];

        if(temp.m_Ability > m_BestAbility.m_Ability)
        {

            m_BestAbility = new TheNetwork(temp.m_Newtwork.m_Inputs,temp.m_Newtwork.m_Hiddenlayer.Length);

            m_BestAbility.SetAbility(temp.m_Ability);

            m_BestAbility.InitialiseWeights(temp.m_Newtwork.GetBrain());

        }

    }

    /*float[][] SetBrain(float[][] size)
    {
        float[][] infomation = size;

        for (int i = 0; i < infomation.Length; i++)
        {
            int temp = i;
        }
    }*/

}

