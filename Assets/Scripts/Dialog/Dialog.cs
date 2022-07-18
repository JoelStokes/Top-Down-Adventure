using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {

    public bool changePitch;
    public string personName;

    [TextArea(5,5)]
    public string[] text;

     [TextArea(5,5)]
    public string[] repeatText;     //If talked to twice in a row, display a different message
    public AudioClip sfx;

    //Eventually add choice to say Yes / No, choose an option, branching text?
}