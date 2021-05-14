using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject ChestOpened;
    public string[] text;
    public Sprite itemImage;
    public string itemType;
    public int itemCount;   //For multiple of the same item
    public AudioClip openSFX;

    public void Open()
    {
        GameObject OpenChest = Instantiate(ChestOpened, transform.position, Quaternion.identity);  //Starts animation of opening
        OpenChest = OpenChest.transform.Find("CombinedEffect").gameObject;
        OpenChest.transform.Find("Item").gameObject.GetComponent<SpriteRenderer>().sprite = itemImage;

        if (itemType == "money")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().money += itemCount;   //Can be used to subtract money with negative itemCount
        }
        else if (itemType == "key")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().keys += 1;    //Never have more than one key in a chest
        }

        GameObject.Find("DialogController").GetComponent<DialogController>().BeginDialog("", text, null, false, true);
        AudioSource.PlayClipAtPoint(openSFX, transform.position, 1.0f);

        Destroy(gameObject);
    }
}