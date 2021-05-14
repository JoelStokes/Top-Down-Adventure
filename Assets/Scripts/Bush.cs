using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public GameObject Leaf;
    public Sprite cut;

    public void Cut()
    {
        Instantiate(Leaf, this.transform.position, Quaternion.identity);

        this.GetComponent<SpriteRenderer>().sprite = cut;
        this.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(this.GetComponent<Bush>());
    }
}
