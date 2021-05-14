using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkSpeed;
    Vector3 change;
    public GameObject Stick;
    public GameObject ZoomPrefab;

    public bool action = false; //Controlled through animator

    public bool talking = false; //Controlled through DialogController
    private int dialogPostCount = 0;
    private int dialogPostLim = 40;

    private int facing = 2;  //0=North, 1=East, 2=South, 3=West

    public int money = 0;   //Only public to be read by GUI
    public int health = 5;
    public int keys = 0;

    private float hurtTimer = 0;    //Prevents movement while initial knockback is happening. Also prevents double damage
    private float hurtLim = .3f;
    private bool hurting = false;
    public AudioClip[] hurtSFX;
    public AudioClip deathSFX;
    public AudioClip keySFX;
    public AudioClip lockSFX;

    //Init in Start
    private Animator anim;
    private Rigidbody2D rigi;
    private AudioSource audioSource;
    private CameraScroll cameraScroll;
    private GameObject DialogController;
    private MagicMeter MagicMeter;

    public void Awake()     //Needed for player to move from scene to scene
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        cameraScroll = GameObject.Find("Main Camera").GetComponent<CameraScroll>();
        DialogController = GameObject.Find("DialogController");
        MagicMeter = GameObject.Find("MagicMeter").GetComponent<MagicMeter>();
    }

    void Update()
    {
        //DontDestroyOnLoad(this.gameObject);

        if (hurting)
        {
            hurtTimer += Time.deltaTime;
            if (hurtTimer > hurtLim)
            {
                hurtTimer = 0;
                hurting = false;
            }
        }

        if (!talking && Time.timeScale == 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !action && !hurting)
            {
                switch (facing)
                {
                    case 0:
                        Stick.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, .5f);
                        break;
                    case 1:
                        Stick.transform.localRotation = Quaternion.Euler(0, 0, 270);
                        Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, -.5f);
                        break;
                    case 2:
                        Stick.transform.localRotation = Quaternion.Euler(0, 0, 180);
                        Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, -.5f);
                        break;
                    default:
                        Stick.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, .5f);
                        break;
                }
                anim.Play("StickSwing");
            }

            if (Input.GetKeyDown(KeyCode.F) && !action && !hurting)
            {
                MagicMeter.UseSpell(20.0f);
            }

            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");

            if (change != Vector3.zero && !action && !hurting && !cameraScroll.panning)
            {
                Move();
            }

            if (dialogPostCount > 0)    //Prevent accidental double talk
                dialogPostCount--;
        }
    }

    void Move()
    {
        rigi.MovePosition(transform.position + change * walkSpeed * Time.deltaTime);
        if (change.y > 0)
        {
            facing = 0;
        }
        else if (change.y < 0)
        {
            facing = 2;
        }
        else if (change.x > 0)
        {
            facing = 1;
        }
        else if (change.x < 0)
        {
            facing = 3;
        }
    }

    void SetPosition(Vector2 newPos)
    {
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    void Hurt(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(deathSFX, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .75f);
            Destroy(gameObject);
        } else
        {
            int rand = Random.Range(0, 3);
            audioSource.PlayOneShot(hurtSFX[rand]);
        }
        hurting = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Warp" && !action)
        {
            WarpInfo warpInfo = other.GetComponent<WarpInfo>();
            string type = warpInfo.type;
            if (type == "Door")
            {
                //Create new Zoom object & pass warp info along to it
                GameObject newZoom = Instantiate(ZoomPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z-1.5f), Quaternion.identity);
                newZoom.GetComponent<ZoomLoad>().SetVariables(warpInfo.location, warpInfo.coordinates, warpInfo.cameraCoordinates);
            }
            else if (type == "Pan")
            {
                string param = other.GetComponent<WarpInfo>().location;
                GameObject.Find("Main Camera").GetComponent<CameraScroll>().ScreenTransition(param);    //Have to find gameObject each time, deleted on screen transitions
            } 
        } else if (other.gameObject.tag == "Coin")
        {
            money += other.GetComponent<Coin>().value;
            Destroy(other.gameObject);
        } else if (other.gameObject.tag == "Key")
        {
            keys++;
            AudioSource.PlayClipAtPoint(keySFX, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .75f);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            float magnitude = 8;
            Vector2 dir = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.y - transform.position.y);
            dir.Normalize();
            GetComponent<Rigidbody2D>().AddForce(-dir * magnitude, ForceMode2D.Impulse);

            if (other.gameObject.GetComponent<EnemyBasic>())
            {
                Hurt(other.gameObject.GetComponent<EnemyBasic>().strength);
            }
            else
            {
                Hurt(other.gameObject.GetComponent<EnemyChaser>().strength);
            }
        } else if (other.gameObject.tag == "Lock")
        {
            if (keys > 0)
            {
                keys--;
                AudioSource.PlayClipAtPoint(lockSFX, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .5f);
                Destroy(other.gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!talking)
        {
            if (other.tag == "Text" && Input.GetKeyDown(KeyCode.Space) && dialogPostCount <= 0)
            {
                talking = true;
                dialogPostCount = dialogPostLim;
                Dialog otherDialog = other.GetComponent<Dialog>();
                DialogController.GetComponent<DialogController>().BeginDialog(otherDialog.personName, otherDialog.text, otherDialog.sfx, otherDialog.changePitch);
            }
            else if (other.tag == "Chest" && Input.GetKeyDown(KeyCode.Space))
            {
                if (other.GetComponent<Chest>() != null)
                {
                    talking = true;
                    dialogPostCount = dialogPostLim;
                    other.GetComponent<Chest>().Open();
                }
            }
        }
    }
}
