using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public bool panning = false;
    Vector2 distance;

    //private int counter = 20;

    private float step = 0;
    private float maxTimeX = 3f;
    private float maxTimeY = 1.5f;
    private float travelLength = 9.85f;
    private Vector3 NewPos;

    private void Update()
    {
        if (panning)
        {
            transform.position = Vector3.MoveTowards(transform.position, NewPos, step);

            if (transform.position == NewPos)   //SHOULD DO MORE TESTING WITH THIS RATHER THAN DEALING WITH EXACTS! could freeze game if not perfectly aligned
            {
                panning = false;
                Time.timeScale = 1;
            }
            /*if ((transform.position.x >= distance.x && distance.x > 0) || (transform.position.x <= distance.x && distance.x < 0) ||
                (transform.position.y >= distance.y && distance.y > 0) || (transform.position.y <= distance.y && distance.y < 0))
            {
                panning = false;
            }*/

            /*
            if (counter > 0)
            {
                timer += Time.deltaTime;
                transform.position = new Vector3(transform.position.x + distance.x, transform.position.y + distance.y, transform.position.z);
                
            } else
                panning = false;*/
        }
    }

    public void ScreenTransition(string dir)
    {
        if (!panning)
        {
            switch (dir)
            {
                case "Down":
                    distance.x = 0; distance.y = -travelLength;
                    step = maxTimeY * Time.deltaTime;
                    break;
                case "Up":
                    distance.x = 0; distance.y = travelLength;
                    step = maxTimeY * Time.deltaTime;
                    break;
                case "Left":
                    distance.x = -travelLength; distance.y = 0;
                    step = maxTimeX * Time.deltaTime;
                    break;
                case "Right":
                    distance.x = travelLength; distance.y = 0;
                    step = maxTimeX * Time.deltaTime;
                    break;
            }

            Time.timeScale = 0;

            NewPos = new Vector3(transform.position.x + distance.x, transform.position.y + distance.y, transform.position.z);
            panning = true;
        }
    }
}
