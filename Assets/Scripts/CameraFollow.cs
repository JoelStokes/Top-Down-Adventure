using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector2 BoundsTopRight;
    public Vector2 BoundsBottomLeft;

    private GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");
    }

    void Update()   //Updates the camera position based on if the player is within the boundary regions set
    {
        if (Player.transform.position.x >= BoundsTopRight.x && Player.transform.position.y < BoundsTopRight.y){  
            transform.position = new Vector3(BoundsTopRight.x, Player.transform.position.y, transform.position.z);              //Update Ys 
        } else if (Player.transform.position.x <= BoundsBottomLeft.x && Player.transform.position.y > BoundsBottomLeft.y)
        {
            transform.position = new Vector3(BoundsBottomLeft.x, Player.transform.position.y, transform.position.z);
        }
        else if (Player.transform.position.x < BoundsTopRight.x && Player.transform.position.y >= BoundsTopRight.y)
        {
            transform.position = new Vector3(Player.transform.position.x, BoundsTopRight.y, transform.position.z);              //Update Xs
        } else if (Player.transform.position.x > BoundsBottomLeft.x && Player.transform.position.y <= BoundsBottomLeft.y){
            transform.position = new Vector3(Player.transform.position.x, BoundsBottomLeft.y, transform.position.z);
        }
        else if ((Player.transform.position.x < BoundsTopRight.x && Player.transform.position.y < BoundsTopRight.y) &&
            (Player.transform.position.x > BoundsBottomLeft.x && Player.transform.position.y > BoundsBottomLeft.y))
        {
            transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);       //Update Both X & Y
        }
        else if (Player.transform.position.x >= BoundsTopRight.x && Player.transform.position.y >= BoundsTopRight.y)
        {
            transform.position = new Vector3(BoundsTopRight.x, BoundsTopRight.y, transform.position.z);       //Out of Bounds Top Right
        } else
        {
            transform.position = new Vector3(BoundsBottomLeft.x, BoundsBottomLeft.y, transform.position.z);     //Out of Bounds Bottom Left
        }
    }
}
