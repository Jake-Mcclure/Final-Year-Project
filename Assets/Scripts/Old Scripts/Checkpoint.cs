using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    CheckpointController checkpointcontroller;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            checkpointcontroller.PlayerThroughCheckpoint(this);
        }
    }
    public void SetCheckpoints(CheckpointController controller)
    {
        this.checkpointcontroller = controller;
    }
}
