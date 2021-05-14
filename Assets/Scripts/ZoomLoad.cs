using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoomLoad : MonoBehaviour
{
    Animator anim;
    public string location = "EMPTY";
    public Vector2 playerPos;
    public Vector2 cameraPos;
    public bool endHit = false;

    private bool reversing = false;
    private bool animChange = false;

    public void Awake()     //Needed for load in. Will be deleted when animation finishes
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (reversing && animChange)
        {
            anim.Play("ZoomReverse");
            reversing = false;

            CameraSet();
        }

        if (endHit && !animChange)
        {
            SceneManager.LoadScene(location);
            StartCoroutine("waitForSceneLoad", location);   //Coroutine needed to wait and move ZoomLoad & Player after Scene Change completes.
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Done"))
        {
            Destroy(gameObject);
        }
    }

    public void SetVariables(string newLoc, Vector2 newPlayerPos, Vector2 newCameraPos)
    {
        location = newLoc;
        playerPos = newPlayerPos;
        cameraPos = newCameraPos;
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
            reversing = true;
            animChange = true;

            CameraSet();
        }
    }

    private void CameraSet()    //Camera Set is called in SceneLoad & Update since some loading scenes take too long for the camera
    {
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(cameraPos.x, cameraPos.y, camera.transform.position.z);
    }
}
