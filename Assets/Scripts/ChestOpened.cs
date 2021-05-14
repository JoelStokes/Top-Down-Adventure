using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpened : MonoBehaviour
{
    private GameObject Player;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!Player.GetComponent<Player>().talking)
        {
            GameObject Effect = transform.Find("CombinedEffect").gameObject;    //When finished talking, item vanishes leaving empty chest
            Destroy(Effect);
            Destroy(GetComponent<ChestOpened>());
        }  
    }
}
