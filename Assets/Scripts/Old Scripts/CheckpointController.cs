using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    List<Checkpoint> checkpointList;
    private int nextCheckpointindex;

    private void Awake()
    {
        checkpointList = new List<Checkpoint>();
        Transform checkpointsTransform = transform.Find("Check");

        foreach (Transform checkpoint in checkpointsTransform)
        {
            Checkpoint singlecheckpoint = checkpoint.GetComponent<Checkpoint>();
            singlecheckpoint.SetCheckpoints(this);


            checkpointList.Add(singlecheckpoint);
        }

        nextCheckpointindex = 0;
    }

    public void PlayerThroughCheckpoint(Checkpoint checkpointsingle)
    {
        if (checkpointList.IndexOf(checkpointsingle) == nextCheckpointindex)
        {
            Debug.Log("correct point");
            nextCheckpointindex = (nextCheckpointindex + 1) % checkpointList.Count;
        }
        else
        {
            Debug.Log("wrong point");
        }
    }
    
}
