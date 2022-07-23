using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/*Player Controller:
 * Performs user movement, attacks, & monitors overall health of main character.
 * Can freely traverse between different scenes.
 * Also handles interactions with other objects/npcs/enemies.
 */

public class Player : MonoBehaviour
{
    public float walkSpeed;
    public float deadZone;
    private Vector2 currentMove;
    public GameObject Stick;
    public ChangeScene changeSceneScript;

    private float attackCounter = 0;
    private bool attack = false;
    private float abilityCounter = 0;
    private bool ability = false;
    private float buttonCount = .25f;   //Buffer between button presses to avoid attack/ability spam
    public bool action = false; //Controlled through animator

    public bool talking = false; //Controlled through DialogController
    private float dialogPostCount = 0;
    private float dialogPostLim = .6f;

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
    private bool textZone = false;
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

    void OnEnable()     //Sets function for every scene change to grab new location's camera & GUI for easier scene building
    {
        SceneManager.sceneLoaded += LoadGUI;
    }

    void LoadGUI(Scene scene, LoadSceneMode mode)
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

        facing = Dir.south;     //Always start game with player facing towards camera
    }

    void FixedUpdate(){
        if (!talking && Time.timeScale == 1)    //Prevent attacks if in dialog or paused
        {
            if (attack && !action && !hurting)
            {
                if (!textZone){
                    Attacking();
                }
                attackCounter = buttonCount;
                attack = false;
            }

            if (ability && !action && !hurting)
            {
                MagicMeter.UseSpell(50.0f, (int) facing, transform.position);
                abilityCounter = buttonCount;
                ability = false;
            }

            if (currentMove != Vector2.zero && !action && !hurting && !cameraScroll.panning)
            {
                Move();
            }

            if (dialogPostCount > 0)    //Prevent accidental NPC double talk if player is mashing button
                dialogPostCount -= Time.deltaTime;
        }
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

        if (attackCounter > 0){
            attackCounter -= Time.deltaTime;
        }

        if (abilityCounter > 0){
            abilityCounter -= Time.deltaTime;
        }
    }

    void Move()     //Handle player movement
    {
        rigi.MovePosition(transform.position + new Vector3(currentMove.x, currentMove.y).normalized * walkSpeed * Time.deltaTime);
    }

    void Attacking()
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

    //Input Manager Functions
    public void SetAttack(InputAction.CallbackContext context){
        if (context.phase == InputActionPhase.Started && attackCounter <= 0 && !attack){
            attack = true;
        }
    }

    public void SetAbility(InputAction.CallbackContext context){
        if (context.phase == InputActionPhase.Started && abilityCounter <= 0 && !ability){
            ability = true;
        }
    }

    public void SetMove(InputAction.CallbackContext context){
        Vector2 newMove = context.ReadValue<Vector2>();
        if (newMove.x > deadZone){
            currentMove.x = 1;
        } else if (newMove.x < -deadZone){
            currentMove.x = -1;
        } else {
            currentMove.x = 0;
        }

        if (newMove.y > deadZone){
            currentMove.y = 1;
        } else if (newMove.y < -deadZone){
            currentMove.y = -1;
        } else {
            currentMove.y = 0;
        }

        //Need to compare greater move value in order for controller stick facing to feel natural
        if (currentMove.y > 0 && currentMove.y > Mathf.Abs(currentMove.x))
        {
            facing = Dir.north;
        }
        else if (currentMove.y < 0 && currentMove.y < -Mathf.Abs(currentMove.x))
        {
            facing = Dir.south;
        }
        else if (currentMove.x > 0)
        {
            facing = Dir.east;
        }
        else if (currentMove.x < 0)
        {
            facing = Dir.west;
        }
    }

    //Triggers & Collisions
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Warp") && !action)
        {
            WarpInfo warpInfo = other.GetComponent<WarpInfo>();
            string type = warpInfo.type;
            if (type == "Door")
            {
                //Start scene change animation & pass proper info
                changeSceneScript.SetVariables(warpInfo.location, warpInfo.coordinates, warpInfo.cameraCoordinates, warpInfo.facing, warpInfo.color);
            }
            else if (type == "Pan")     //Hit camera's edge and prepares to pan the camera in the moving direction
            {
                string param = other.GetComponent<WarpInfo>().location;
                cameraScroll.ScreenTransition(param);
            } 
        } else if (other.CompareTag("Coin"))
        {
            money += other.GetComponent<Coin>().value;
            Destroy(other.gameObject);
        } else if (other.CompareTag("Key"))
        {
            keys++;
            AudioSource.PlayClipAtPoint(keySFX, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .75f);
            Destroy(other.gameObject);
        } else if (other.CompareTag("Text") || other.CompareTag("Chest")){
            textZone = true;
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (!talking && !action)
        {
            if (other.CompareTag("Text") && attackCounter > 0 && dialogPostCount <= 0) //Start dialog with sign or NPC
            {
                talking = true;
                dialogPostCount = dialogPostLim;
                Dialog otherDialog = other.GetComponent<Dialog>();
                DialogController.GetComponent<DialogController>().BeginDialog(otherDialog.personName, otherDialog.text, otherDialog.sfx, otherDialog.changePitch);
            }
            else if (other.CompareTag("Chest") && attackCounter > 0)   //Open Chest
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

    void OnTriggerExit2D(Collider2D other)  //Used to start/stop attack prevention region
    {
        if (other.CompareTag("Text") || other.CompareTag("Chest")){
            textZone = false;
        }
    }    
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))    //Add knockback force to player and get enemy's strength for hurt calculation
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
        } else if (other.gameObject.CompareTag("Lock"))
        {
            if (keys > 0)   //Use collected key to open lock
            {
                keys--;
                AudioSource.PlayClipAtPoint(lockSFX, 0.9f * Camera.main.transform.position + 0.1f * transform.position, .5f);
                Destroy(other.gameObject);
            }
        }
    }
}
