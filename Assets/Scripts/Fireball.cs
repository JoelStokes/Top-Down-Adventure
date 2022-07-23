using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = .25f;
    public float strength = 1;
    public Sprite burnt;

    private Rigidbody2D rigi;

    void Start(){
        rigi = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rigi.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")){
            if (other.CompareTag("Enemy")){
                other.gameObject.BroadcastMessage("Hurt", strength);    //Hurt any enemy regardless of script name
                addForce(other.gameObject);
            } else if (other.CompareTag("Stump")){
                other.gameObject.GetComponent<SpriteRenderer>().sprite = burnt;
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }

            Destroy(gameObject);    //Any collision that's not the player, fireball should be destroyed
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
