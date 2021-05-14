using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{

    public string personName;
    public List<string> textList = new List<string>();
    public AudioClip talkSFX;
    
    public GameObject Textbox;
    public GameObject Namebox;
    public GameObject Arrow;
    public GameObject BoxObj;

    public Sprite textboxPicName;
    public Sprite textboxPicNameless;

    public AudioClip defaultSFX;

    private int counter = 0;
    private int stringPos = 0;
    private float highPos;
    private float lowPos;
    private bool changePitch;

    private bool delay = false;
    private float delayTimer = 0.0f;
    private float delayLim = 1.0f;

    private bool started = false;

    /*public void Awake()     //Needed for DialogController to move from scene to scene
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }*/

    public void BeginDialog(string newName, string[] newText, AudioClip sfx, bool pitch, bool startDelay = false)
    {
        personName = newName;
        for (int i=0; i<newText.Length; i++)
        {
            textList.Add(newText[i]);
        }

        if (sfx)
        {
            talkSFX = sfx;
        } else
        {
            talkSFX = defaultSFX;
        }
        changePitch = pitch;

        if (startDelay)
        {
            delay = true;
        }

        Arrow.SetActive(false);

        stringPos = 0;
        counter = 0;
        if (personName == "")
            BoxObj.GetComponent<Image>().sprite = textboxPicNameless;
        else
            BoxObj.GetComponent<Image>().sprite = textboxPicName;

        Namebox.GetComponent<Text>().text = personName;
        Textbox.GetComponent<Text>().text = "";

        for (int i = 0; i < textList.Count; i++)
        {
            textList[i] = ManageText(textList[i]);
        }

        if (!delay)
        {
            BoxObj.SetActive(true);
            started = true;
        }
    }

    private void Start()
    {
        BoxObj.SetActive(false);
        Arrow.SetActive(false);

        Namebox.GetComponent<Text>().text = "";
        Textbox.GetComponent<Text>().text = "";
    }

    private void Update()
    {
        if (!delay)
        {
            if (started)
            {
                Textbox.GetComponent<Text>().text = textList[stringPos].Substring(0, counter);

                if (Input.GetKeyDown(KeyCode.Space) && counter < textList[stringPos].Length && counter > 1)
                {
                    counter = textList[stringPos].Length;   //Immediately jump to the end of the textbox
                }
                else if (Input.GetKeyDown(KeyCode.Space) && stringPos < textList.Count - 1 && counter > 1)
                {
                    Arrow.SetActive(false);     //Change to next string in set
                    stringPos++;
                    counter = 0;
                }
                else if (Input.GetKeyDown(KeyCode.Space) && counter > 1)
                {
                    EndDialog();    //Finished talking
                }

                if (stringPos < textList.Count && counter < textList[stringPos].Length)
                {
                    counter++;

                    char c = textList[stringPos][counter - 1];
                    if (char.IsWhiteSpace(c))
                    {
                        if (changePitch)
                        {
                            GetComponent<AudioSource>().pitch = (Random.Range(0.8f, 1.05f));
                        }
                        else
                        {
                            GetComponent<AudioSource>().pitch = 1f;
                        }
                        GetComponent<AudioSource>().PlayOneShot(talkSFX, 1.1f);
                    }
                }
                else if (textList.Count != 0 && counter == textList[stringPos].Length)
                    Arrow.SetActive(true);
            }
        } else
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > delayLim)
            {
                delay = false;
                delayTimer = 0;
                BoxObj.SetActive(true);
                started = true;
            }
        }
    }

    void EndDialog()
    {
        started = false;
        BoxObj.SetActive(false);
        Arrow.SetActive(false);
        Namebox.GetComponent<Text>().text = "";
        Textbox.GetComponent<Text>().text = "";
        textList.Clear();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().talking = false;
    }

    private string ManageText(string InText)    //Special Characters to space text
    {
        int CIndex = 0;
        while (CIndex != -1)
        {
            CIndex = InText.IndexOf("-NEWLINE"); // Gets the Index of your "\n" Char
            if (CIndex != -1)
            {
                InText = InText.Remove(CIndex, 8); // Removes "\n from original String"
                InText = InText.Insert(CIndex, "\n"); // Adds the actual New Line symbol
            }
        }

        CIndex = 0;
        while (CIndex != -1)
        {
            CIndex = InText.IndexOf("-TAB"); // Gets the Index of your "\t" Char
            if (CIndex != -1)
            {
                InText = InText.Remove(CIndex, 4); // Removes "\t from original String"
                InText = InText.Insert(CIndex, "\t"); // Adds the actual Tab symbol
            }
        }
        return InText;
    }
}