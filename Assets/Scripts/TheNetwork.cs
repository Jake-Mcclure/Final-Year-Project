using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheNetwork 
{
    
    public float[] m_Hiddenlayer;
    public float[][] m_HiddenLWeights;
    public float[] m_HiddenLBias;

    public int m_Inputs;

    float[] m_OutputWeights;
    float m_OutputBias;

    public float m_Ability;

    public TheNetwork()
    {

    }

    public TheNetwork(int inputs, int Hiddennodes)
    {
        m_Hiddenlayer = new float[Hiddennodes];
        InitialiseWeights(inputs);
        m_Inputs = inputs;

    }

    public void SetAbility(float ability)
    {
        m_Ability = ability;
    }
    //this makes all weights equal to 0
    public void InitialiseWeights(int inputs)
    {
        m_Inputs = inputs;

        m_HiddenLWeights = new float[m_Hiddenlayer.Length][];
        m_HiddenLBias = new float[m_HiddenLWeights.Length];

        m_OutputWeights = new float[m_Hiddenlayer.Length];

        for (int i = 0; i < m_HiddenLWeights.GetLength(0); i++)
        {
            m_HiddenLWeights[i] = new float[inputs];
            m_HiddenLBias[i] = 0;
            m_OutputWeights[i] = 0;
            for (int x = 0; x < m_HiddenLWeights.Length; x++)
            {
                m_HiddenLWeights[i][x] = 0;
            }

        }
    }

    public void InitialiseWeights(float[] Weights)
    {

        for (int i = 0; i < m_HiddenLWeights.Length; i++)
        {
            for (int x = 0; x < m_HiddenLWeights[i].Length; x++)
            {
                m_HiddenLWeights[i][x] = Weights[x + (6*i)];
            }

            m_HiddenLBias[i] = Weights[5 + (i * 6)];

            m_OutputWeights[i] = Weights[i + 24];

            m_OutputBias = Weights[Weights.Length - 1];
        }


    }

    public float CalculateOutput(float[] inputs)
    {
        Debug.Log("calculating output, HL length is " + m_Hiddenlayer.Length);
        


        for (int i = 0; i < m_Hiddenlayer.Length; i++)
        {
            m_Hiddenlayer[i] = ReLU(Sum(inputs, m_HiddenLWeights[i]) + m_HiddenLBias[i]);
            Debug.Log("hidden l weights are " + m_HiddenLWeights[i]);
        }
        float output = Limit(Sum(m_OutputWeights, m_Hiddenlayer) + m_OutputBias);

        Debug.Log("output is " + output);

        return output;
    }

    public float Sum(float[] a, float[] b)
    {
        float temp = 0;
        for (int i = 0; i < a.Length; i++)
        {
            temp += a[i] * b[i];
        }
        return temp;
    }

    float ReLU(float inp)
    {
        return Mathf.Max(0, inp);
    }

    float Limit(float input)
    {
        return input / (1 + Mathf.Abs(input));
    }

    //this basically gives the information/details about this brain
    public string ReadBrain()
    {
        string information = "";

        for (int i = 0; i < m_HiddenLWeights.Length; i++)
        {
            for (int x = 0; x < m_HiddenLWeights[i].Length; x++)
            {
                information += m_HiddenLWeights[i][x] + ",";
            }
            information += m_HiddenLBias[i] + ",";
        }
        for (int i = 0; i < m_OutputWeights.Length; i++)
        {
            information += m_OutputWeights[i] + ",";
        }
        information += m_OutputBias;

        return information;
    }

    //this transfers the information from string format to an array of float values
    public float[] GetBrain()
    {
        string[] information = ReadBrain().Split(',');
        float[] temp = new float[information.Length];

        for (int i = 0; i < information.Length; i++)
        {
            temp[i] = float.Parse(information[i]);
        }

        return temp;
    }

}
