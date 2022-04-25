using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour
{

    private float[][] m_InitialData;
    private string[] m_InitialString;

    public List<string> LoadedInformation;

    private float m_MutationRate;
    private float m_MutationProbability;
}
