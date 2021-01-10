using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
    private static int counter = 0;

    [SerializeField] private TileTexture[] tileTexture;
    [SerializeField] private TileType type;
    public int Width { get; private set; }
    public int Height { get; private set; }

    float timeElapsed;

    private void Awake()
    {
        sr = this.GetComponent<SpriteRenderer>();
        //Width = (int)sr.sprite.rect.width;
        //Height = (int)sr.sprite.rect.width;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Board.Instance.isDisco)
        {
            if (timeElapsed > Board.Instance.discoTime)
            {
                timeElapsed = 0;
                sr.color = colors[Random.Range(0, colors.Length)];
            }
            timeElapsed += Time.deltaTime;
        }
    }

    public void SetTileType(TileType type)
    {
        this.type = type;
    }

    public void SetTileTypeFromColor(Color color)
    {
        if (color.a == 0)
        {
            type = TileType.Transparent;
            sr.color = Color.white;
            SetTexture(type);
            return;
        } else if (color == Color.white)
        {
            type = TileType.Normal;
        } else if (color == Color.black)
        {
            type = TileType.Wall;
        } else
        {
            type = TileType.Piece;
            //Debug.LogError("Color unknown");
        }
        sr.color = color;
        SetTexture(type);
    }

    private void SetTexture(TileType type)
    {
        foreach (TileTexture t in tileTexture)
        {
            if(t.type == type)
            {
                sr.sprite = t.sprite;
            }
        }
    }
}

[System.Serializable]
public struct TileTexture
{
    public TileType type;
    public Sprite sprite;
}
