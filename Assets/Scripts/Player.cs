using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Player Controller:
 * Performs user movement, attacks, & monitors overall health of main character.
 * Can freely traverse between different scenes.
 * Also controls interactions with other objects/npcs/enemies.
 * 
 * To do:
 * dialogPostCount is count down based on frames

    change inputs from keys to Unity Input

    In move, use Delta for diagonal (See Pro notes)
 */

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

    enum Dir    //Used to set current direction for animations / sword swings
    {
        north,
        east,
        south,
        west
    }
    private Dir facing;

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

    public void Awake()     //Allow Player to move from scene to scene
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()     //Sets function for every scene change to grab new location's camera & GUI
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cameraScroll = GameObject.Find("Main Camera").GetComponent<CameraScroll>();
        DialogController = GameObject.Find("DialogController");
        MagicMeter = GameObject.Find("MagicMeter").GetComponent<MagicMeter>();
    }

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        facing = Dir.south;
    }

    void Update()
    {
        if (hurting)    //Invincibility grace period to prevent player being hurt multiple times
        {
            hurtTimer += Time.deltaTime;
            if (hurtTimer > hurtLim)
            {
                hurtTimer = 0;
                hurting = false;
            }
        }

        if (!talking && Time.timeScale == 1)    //Prevent attacks if in dialog or paused
        {
            if (Input.GetKeyDown(KeyCode.Space) && !action && !hurting)
            {
                Attack();
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

            if (dialogPostCount > 0)    //Prevent accidental NPC double talk if player is mashing button
                dialogPostCount--;
        }
    }

    void Attack()
    {
        switch (facing) //Set Rotation & Depth of stick swing based on current direction, then play attack
        {
            case Dir.north:
                Stick.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, .5f);
                break;
            case Dir.east:
                Stick.transform.localRotation = Quaternion.Euler(0, 0, 270);
                Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, -.5f);
                break;
            case Dir.south:
                Stick.transform.localRotation = Quaternion.Euler(0, 0, 180);
                Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, -.5f);
                break;
            case Dir.west:
                Stick.transform.localRotation = Quaternion.Euler(0, 0, 90);
                Stick.transform.position = new Vector3(Stick.transform.position.x, Stick.transform.position.y, .5f);
                break;
            default:
                Debug.Log("Invalid Direction Detected!");
                break;
        }
        anim.Play("StickSwing");
    }

    void Move()     //Handle player movement
    {
        rigi.MovePosition(transform.position + change * walkSpeed * Time.deltaTime);
        if (change.y > 0)
        {
            facing = Dir.north;
        }
        else if (change.y < 0)
        {
            facing = Dir.south;
        }
        else if (change.x > 0)
        {
            facing = Dir.east;
        }
        else if (change.x < 0)
        {
            facing = Dir.west;
        }
    }

    void Hurt(int damage)   //Handle player injury
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
                //Start Zoom effect & pass warp info along to it
                GameObject newZoom = Instantiate(ZoomPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z-1.5f), Quaternion.identity);
                newZoom.GetComponent<ZoomLoad>().SetVariables(warpInfo.location, warpInfo.coordinates, warpInfo.cameraCoordinates);
            }
            else if (type == "Pan")     //Hit camera's edge and prepares to pan the camera in the moving direction
            {
                string param = other.GetComponent<WarpInfo>().location;
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")    //Add knockback force to player and get enemy's strength for hurt calculation
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
            if (keys > 0)   //Use collected key to open lock
            {
                keys--;
                AudioSource.PlayClipAtPoint(lockSFX, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .5f);
                Destroy(other.gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!talking)
        {
            if (other.tag == "Text" && Input.GetKeyDown(KeyCode.Space) && dialogPostCount <= 0) //Start dialog with sign or NPC
            {
                talking = true;
                dialogPostCount = dialogPostLim;
                Dialog otherDialog = other.GetComponent<Dialog>();
                DialogController.GetComponent<DialogController>().BeginDialog(otherDialog.personName, otherDialog.text, otherDialog.sfx, otherDialog.changePitch);
            }
            else if (other.tag == "Chest" && Input.GetKeyDown(KeyCode.Space))   //Open Chest
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
