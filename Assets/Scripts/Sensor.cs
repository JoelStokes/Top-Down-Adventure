using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Calls parent if player enters or leaves trigger

public class Sensor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            transform.parent.BroadcastMessage("PlayerEnter", other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            transform.parent.BroadcastMessage("PlayerExit");
        }
    }
}
