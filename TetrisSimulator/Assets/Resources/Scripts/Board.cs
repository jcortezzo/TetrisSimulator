using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private Tile[,] board;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] Sprite sprite;

    [SerializeField] public Dictionary<Piece, Vector2Int> piece;

    public int width;
    public int height;

    public float discoTime;
    public bool isDisco;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(sprite != null)
        {
            GenerateBoard(sprite.texture);
        } else
        {
            GenerateBoard();
        }
        Tile center = GetCenterTile();
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(center.transform.position.x, center.transform.position.y, cam.transform.position.z);
    }

    public void GenerateBoard()
    {
        board = new Tile[height, width];
        for (int i = 0; i < height; i++)
        {
            Vector2 tilePosition = this.transform.position - new Vector3(0, i, 0);
            for (int j = 0; j < width; j++)
            {
                Tile tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                tilePosition += Vector2.right;
                board[i, j] = tile;
            }
        }
    }

    public Tile[,] GenerateBoard(Texture2D levelTexture)
    {
        height = levelTexture.width;
        width = levelTexture.height;
        board = new Tile[height, width];
        Tile[,] newBoard = new Tile[levelTexture.width, levelTexture.height];
        
        // 0,0 of the texture is at bottom left corner
        for(int x = 0; x < levelTexture.width; x++)
        {
            Vector2 tilePosition = this.transform.position + new Vector3(x, 0, 0);
            for (int y = levelTexture.height - 1; y >= 0 ; y--)
            {
                Tile tile = GenerateTileFromTexture2D(levelTexture, x, y, tilePosition);
                tilePosition += Vector2.down;
                board[x, y] = tile;
            }
        }
        return newBoard;
    }

    private Tile GetCenterTile()
    {
        return board[height / 2, width / 2];
    }

    private Tile GenerateTileFromTexture2D(Texture2D texture2D, int x, int y, Vector2 position) 
    {
        Tile tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tile.SetTileTypeFromColor(texture2D.GetPixel(x, y));
        return tile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Tile[,] GetBoard()
    {
        return board;
    }
}
