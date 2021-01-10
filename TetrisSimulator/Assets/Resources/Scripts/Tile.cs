using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private TileTexture[] tileTexture;
    [SerializeField] private TileType type;
    public int Width { get; private set; }
    public int Height { get; private set; }

    private bool selected;
    private Color previousColor;

    private bool isPreview;
    private Piece correspondingPiece;

    public Vector2Int coord;

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
        if (type != TileType.Piece)
        {
            SetCorrespondingPiece(null);
        }
        
        if(selected)
        {
            sr.color = Color.red;
        } else
        {
            sr.color = previousColor;
        }

        if (correspondingPiece != null)
        {
            //previousColor = sr.color;
            sr.color = correspondingPiece.GetColor();
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

    public void SetCorrespondingPiece(Piece p)
    {
        correspondingPiece = p;
    }

    public Piece GetCorrespondingPiece()
    {
        return correspondingPiece;
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
            type = TileType.LevelPiece;
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
                if (type == TileType.Piece)
                {
                    if (correspondingPiece == null) sr.color = Color.white;
                    else sr.color = correspondingPiece.GetColor();
                } else if(type == TileType.Normal || type == TileType.Transparent)
                {
                    sr.color = Color.white;
                } else if(type == TileType.Wall)
                {
                    sr.color = Color.black;
                }
                previousColor = sr.color;
            }
        }
        previousColor = sr.color;
    }

    public TileType GetTileType()
    {
        return type;
    }

    public void Select()
    {
        selected = true;
    }

    public void Unselect()
    {
        selected = false;
    }

}

[System.Serializable]
public struct TileTexture
{
    public TileType type;
    public Sprite sprite;
}
