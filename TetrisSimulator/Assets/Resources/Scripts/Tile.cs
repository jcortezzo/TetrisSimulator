using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
    private static int counter = 0;
    
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    float timeElapsed;

    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        Width = (int)sr.sprite.rect.width;
        Height = (int)sr.sprite.rect.width;
        sr.color = colors[(counter++) % colors.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if(timeElapsed > Board.Instance.discoTime)
        {
            timeElapsed = 0;
            sr.color = colors[Random.Range(0, colors.Length)];
        }
        timeElapsed += Time.deltaTime;
    }


}
