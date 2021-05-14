using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public float health = 3;
    public float walkSpeed = 1;
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

    private bool chasing = false;
    public float chaseSpeed = 1;
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Randomize();
        rigi = this.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

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

        if (chasing)
        {
            if (Player)     //Helps prevent error on death
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, (chaseSpeed * Time.deltaTime));
            }
        }
        else
        {
            if (!walking)
            {
                if (timer > waitCount)
                {
                    timer = 0;
                    walking = true;
                }
            }
            else
            {
                Move();
                if (timer > walkCount)
                {
                    Reset();
                }
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

    public void PlayerEnter(GameObject newPlayer)
    {
        chasing = true;
        Player = newPlayer;
    }

    public void PlayerExit()
    {
        chasing = false;
        Reset();
    }
}