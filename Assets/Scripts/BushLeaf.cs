using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushLeaf : MonoBehaviour
{
    private float timer = 0;
    private float timeLim;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        timeLim = (Random.Range(40, 60)/10);
        sr = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        Color col = sr.color;
        float lerp = Mathf.PingPong(Time.time, timeLim) / timeLim;
        col = new Vector4(col.r, col.g, col.b, Mathf.Lerp(0.0f, 1.0f, lerp));
        sr.color = col;

        if (timer > timeLim)
        {
            Destroy(gameObject);
        }
    }
}
