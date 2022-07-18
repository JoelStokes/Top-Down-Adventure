using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Animator maskAnim;
    public AudioSource music;
    private float musicStartVolume = .1f;

    private Color newColor;
    private string location = "EMPTY";
    private Vector2 playerPos;
    private Vector2 cameraPos;
    private int facing = 0;

    private bool changing = false;
    private Color startColor;
    private float timeElapsed = 0;

    private List<SpriteRenderer> paperRenderers = new List<SpriteRenderer>();

    public void Awake()     //Needed for load in. Will be deleted when animation finishes
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        foreach (Transform child in transform){
            paperRenderers.Add(child.GetComponent<SpriteRenderer>());
        }

        startColor = paperRenderers[0].color;
    }

    void Update()
    {
        if (changing){
            timeElapsed += Time.deltaTime;
            Color lerpedColor = Color.Lerp(startColor, newColor, timeElapsed);

            for (int i=0; i<paperRenderers.Count; i++){
                paperRenderers[i].color = lerpedColor;
            }

            music.volume = Mathf.Lerp(.1f, 0, timeElapsed/1f);

            if (maskAnim.GetCurrentAnimatorStateInfo(0).IsName("Done")){
                SceneManager.LoadScene(location);
                StartCoroutine("waitForSceneLoad", location);   //Coroutine needed to wait and move ZoomLoad & Player after Scene Change completes.
                changing = false;
            }
        }
    }

    public void SetVariables(string newLoc, Vector2 newPlayerPos, Vector2 newCameraPos, int newFacing, Color passedColor)
    {
        location = newLoc;
        playerPos = newPlayerPos;
        cameraPos = newCameraPos;
        facing = newFacing;
        
        startColor = newColor;
        newColor = passedColor;

        maskAnim.SetTrigger("Wipe");

        changing = true;
        timeElapsed = 0;
    }

    IEnumerator waitForSceneLoad(string sceneName)
    {
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }

        // Do anything after proper scene has been loaded
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
            GameObject Player = GameObject.FindGameObjectWithTag("Player");
            Player.transform.position = new Vector3(playerPos.x, playerPos.y, Player.transform.position.z);

            CameraSet();
        }
    }

    private void CameraSet()    //Camera Set is called in SceneLoad & Update since some loading scenes take too long for the camera
    {
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(cameraPos.x, cameraPos.y, camera.transform.position.z);

        music = camera.GetComponent<AudioSource>();
        musicStartVolume = music.volume;

        maskAnim = camera.transform.Find("Cutout Wipe").GetComponent<Animator>();

    }
}
