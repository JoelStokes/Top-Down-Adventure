using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    private Player playerScript;

    public Text moneyText;
    public Text healthText;
    public Text keyText;

    public void Awake()     //Needed for GUI to move from scene to scene
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        moneyText.text = playerScript.money.ToString();
        healthText.text = playerScript.health.ToString();
        keyText.text = playerScript.keys.ToString();
    }
}
