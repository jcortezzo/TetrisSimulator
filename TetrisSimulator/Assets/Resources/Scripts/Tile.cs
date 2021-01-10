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

    private bool selected;
    private Color previousColor;

    private bool isPreview;

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
        if(selected)
        {
            sr.color = Color.red;
        } else
        {
            sr.color = previousColor;
        }
        if (isPreview)
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/tile-1.png");
        }
        else
        {
            SetTexture(this.type);
        }
    }

    public void Preview()
    {
        isPreview = true;
    }

    public void Unpreview()
    {
        isPreview = false;
    }

    public void SetTileType(TileType type)
    {
        this.type = type;
        SetTexture(type);
    }

    public void SetTileTypeFromColor(Color color)
    {
        if (color.a == 0)
        {
            type = TileType.Transparent;
            sr.color = Color.white;
            previousColor = color;
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
        }
        sr.color = color;
        previousColor = color;
        SetTexture(type);
    }

    private void SetTexture(TileType type)
    {
        foreach (TileTexture t in tileTexture)
        {
            if(t.type == type)
            {
                sr.sprite = t.sprite;

                // uhhh yeah idk
                if (type == TileType.Piece)
                {
                    sr.color = Color.blue;
                }
            }
        }
        previousColor = sr.color;
    }

    public void Select()
    {
        selected = true;
    }

    public void Unselect()
    {
        selected = false;
    }

    public TileType GetTileType()
    {
        return type;
    }
}

[System.Serializable]
public struct TileTexture
{
    public TileType type;
    public Sprite sprite;
}
