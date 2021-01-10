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

    private Dictionary<Tile, Vector2Int> tileToCoord;
    private Dictionary<Vector2Int, Tile> coordToTile;

    //public Piece piece;

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
        tileToCoord = new Dictionary<Tile, Vector2Int>();
        coordToTile = new Dictionary<Vector2Int, Tile>();

        if(sprite != null)
        {
            GenerateBoard(sprite.texture);
        } else
        {
            GenerateBoard();
        }
        MapBoard();
        Tile center = GetCenterTile();
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(center.transform.position.x, center.transform.position.y, cam.transform.position.z);
    }

    public void PlacePiece(Piece p, Tile t)
    {
        if (board == null) return;
        Vector2Int placementPos = tileToCoord[t];

        Tile[,] newBoard = new Tile[height, width];
        Debug.Log(p.boundingBox);
        for (int y = placementPos.y; y < p.boundingBox.GetLength(0) + placementPos.y; y++)
        {
            for (int x = placementPos.x; x < p.boundingBox.GetLength(1) + placementPos.x; x++)
            {
                Tile currTile = coordToTile[new Vector2Int(x, y)];
                //if (p.boundingBox[y - placementPos.y, x - placementPos.x])
                //{
                    currTile.SetTileType(TileType.Piece);
                //}
            }
        }
    }

    private void MapBoard()
    {
        for (int y = 0; y < board.GetLength(0); y++)
        for (int x = 0; x < board.GetLength(1); x++)
        {
            Vector2Int coord = new Vector2Int(x, y);
            tileToCoord.Add(board[y, x], coord);
            coordToTile.Add(coord, board[y, x]);
        }
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
