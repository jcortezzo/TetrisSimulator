using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private Tile[,] board;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] Sprite map;

    [SerializeField] public List<Tile> pieces;

    public int width;
    public int height;

    public float discoTime;
    public bool isDisco;

    public bool startGame;

    private Dictionary<Tile, Vector2Int> tileToCoord;
    private Dictionary<Vector2Int, Tile> coordToTile;

    ISet<Tile> previewedTiles;

    //public Piece piece;
    [SerializeField] private float ticTime;
    [SerializeField] private float elapsedTicTime;

    
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


        previewedTiles = new HashSet<Tile>();

        if(map != null)
        {
            GenerateBoard(map.texture);
        } else
        {
            GenerateBoard();
        }
        MapBoard();
        Tile center = GetCenterTile();
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(center.transform.position.x, center.transform.position.y, cam.transform.position.z);
    }

    public void ClearPreviews()
    {
        foreach (Tile t in previewedTiles)
        {
            t.Unpreview();
        }
        previewedTiles.Clear();
    }

    public void PreviewPiece(Piece p, Tile t)
    {
        Vector2Int placementPos = tileToCoord[t];

        for (int y = placementPos.y; y < p.boundingBox.GetLength(0) + placementPos.y; y++)
        {
            for (int x = placementPos.x; x < p.boundingBox.GetLength(1) + placementPos.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                if (!coordToTile.ContainsKey(coord))
                {
                    continue;
                }
                Tile currTile = coordToTile[coord];
                if (p.boundingBox[y - placementPos.y, x - placementPos.x])
                {
                    currTile.Preview();
                    previewedTiles.Add(currTile);
                }
            }
        }
    }

    public void PlacePiece(Piece p, Tile t)
    {
        if (board == null) return;
        Vector2Int placementPos = tileToCoord[t];
        Tile[,] copyBoard = board.Clone() as Tile[,];

        bool canPlace = true;

        for (int y = placementPos.y; y < p.boundingBox.GetLength(0) + placementPos.y; y++)
        {
            for (int x = placementPos.x; x < p.boundingBox.GetLength(1) + placementPos.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                if (!coordToTile.ContainsKey(coord))
                {
                    canPlace = false;
                    break;
                }
                Tile currTile = coordToTile[coord];
                if (p.boundingBox[y - placementPos.y, x - placementPos.x] &&
                    (currTile.GetTileType() != TileType.Normal && currTile.GetTileType() != TileType.Transparent))
                {
                    canPlace = false;
                    break;
                }
            }
        }

        if (!canPlace) return;

        for (int y = placementPos.y; y < p.boundingBox.GetLength(0) + placementPos.y; y++)
        {
            for (int x = placementPos.x; x < p.boundingBox.GetLength(1) + placementPos.x; x++)
            {
                Tile currTile = coordToTile[new Vector2Int(x, y)];
                if (p.boundingBox[y - placementPos.y, x - placementPos.x])
                {
                    currTile.SetTileType(TileType.Piece);
                    pieces.Add(currTile);
                }
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
        width = levelTexture.width;
        height = levelTexture.height;
        board = new Tile[height, width];
        Tile[,] newBoard = new Tile[height, width];

        // 0,0 of the texture is at bottom left corner
        for(int y = height - 1; y >= 0; y--)
        {
            Vector2 tilePosition = this.transform.position - new Vector3(0, height - y, 0);
            for (int x = 0; x < width ; x++)
            {
                Tile tile = GenerateTileFromTexture2D(levelTexture, x, y, tilePosition);
                tilePosition += Vector2.right;
                board[(height - 1) - y, x] = tile;
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
        if(elapsedTicTime > ticTime)
        {
            elapsedTicTime = 0;
            Tic();
        }
        elapsedTicTime += Time.deltaTime;

    }

    private void Tic()
    {
        ClearRow(3);

        if(startGame)
        {
            MovePieceDown();
        }

    }

    private void MovePieceDown()
    {
        for(int y = height - 1; y >=0; y--)
        {
            for(int x = 0; x < width; x++)
            {
                Tile tile = board[y, x];
                if(tile.GetTileType() == TileType.Piece)
                {
                    if(y + 1 < height)
                    {
                        Tile bellowTile = board[y + 1, x];
                        tile.SetTileType(TileType.Normal);
                        bellowTile.SetTileType(tile.GetTileType());
                    }
                    
                }
            }
        }
    }

    private void ClearRow(int row)
    {
        // Wall, piece, piece, ... , piece, Wall//
        TileType[] match = { TileType.Wall, TileType.Piece, TileType.Wall };
        int i = 0;
        for (;i < width; i++)
        {
            Tile tile = board[row, i];
            if (i == 0)// first tile
            {
                if (tile.GetTileType() != match[0]) break;
            }
            
            if (i == width - 1)
            {
                if (tile.GetTileType() != match[2]) break;
            }
            
            if (i > 0 && i < width - 1)
            {
                if (tile.GetTileType() != match[1]) break;
            }
        }
        if(i == width)
        {
            i = 1;
            for (; i < width - 1; i++)
            {
                Tile tile = board[row, i];
                tile.SetTileType(TileType.Normal);
            }
        }

    }

    public Tile[,] GetBoard()
    {
        return board;
    }
}
