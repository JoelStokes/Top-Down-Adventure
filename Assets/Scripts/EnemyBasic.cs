using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public float health = 3;
    public float walkSpeed = 3;
    public AudioClip SFXHurt;
    public AudioClip SFXDie;
    public int strength = 1;

    private float waitCount;
    private float walkCount;
    private int direction = 5;  //Starts on an impossible number to ensure purely random start direction
    private bool walking = false;
    private float timer;

    private float invincibleTimer;  //Prevent double hits from happening
    private float invincibleLim = .2f;
    private bool invincible = false;

    private Rigidbody2D rigi;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Randomize();
        rigi = this.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (invincible)
        {
            invincibleTimer += Time.deltaTime;
            if (invincibleTimer > invincibleLim)
            {
                invincible = false;
                invincibleTimer = 0;
            }
        }

        if (!walking)
        {

            if (timer > waitCount)
            {
                timer = 0;
                walking = true;
            }
        } else
        {
            Move();
            if (timer > walkCount)
            {
                Reset();
            }
        }
    }

    private void Randomize()
    {
        waitCount = Random.Range(0.4f, 1f);
        walkCount = Random.Range(0.6f, 1.4f);

        int lastDirection = direction;
        while (direction == lastDirection)
        {
            direction = Random.Range(0, 4);
        }
    }

    private void Move()
    {
        Vector3 change = Vector3.zero;
        switch (direction)
        {
            case 1:
                change.y = walkSpeed;
                break;
            case 2:
                change.x = walkSpeed;
                break;
            case 3:
                change.y = -walkSpeed;
                break;
            default:
                change.x = -walkSpeed;
                break;
        }
        rigi.MovePosition(transform.position + change * walkSpeed * Time.deltaTime);
    }

    public void Hurt(float damage)
    {
        if (!invincible)
        {
            health -= damage;
            Reset();

            if (health <= 0)
            {
                AudioSource.PlayClipAtPoint(SFXDie, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .5f);
                Die();
            } else
            {
                invincible = true;
                audioSource.PlayOneShot(SFXHurt, .5f);
            }
        }
    }

    public void Reset()
    {
        walking = false;
        timer = 0;
        Randomize();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}