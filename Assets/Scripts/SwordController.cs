using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float strength = 1;  //Used to calculate damage

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bush")
        {
            other.gameObject.GetComponent<Bush>().Cut();
        } else if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.BroadcastMessage("Hurt", strength);    //Hurt any enemy regardless of script name
            addForce(other.gameObject);
        }
    }

    public void addForce(GameObject other)  //Push enemy / object back when hit
    {
        float magnitude = 5;
        Vector2 dir = new Vector2(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y);
        dir.Normalize();
        other.GetComponent<Rigidbody2D>().AddForce(-dir * magnitude, ForceMode2D.Impulse);
    }
}
