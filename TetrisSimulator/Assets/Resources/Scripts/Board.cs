using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private Tile[,] board;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] Sprite map;

    //[SerializeField] private Dictionary<Piece, Tile[,]> pieceDict;
    [SerializeField] private (Piece, Tile[,], ISet<Tile>) pieceTuple;
    public int width;
    public int height;

    public float discoTime;
    public bool isDisco;

    public bool startGame;

    private Dictionary<Tile, Vector2Int> tileToCoord;
    private Dictionary<Vector2Int, Tile> coordToTile;

    ISet<Tile> previewedTiles;

    public MoveDirection moveDirection = MoveDirection.Down;

    //public Piece piece;
    [SerializeField] private float ticTime;
    [SerializeField] private float elapsedTicTime;

    Player player;

    
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
        //pieceDict = new Dictionary<Piece, Tile[,]>();

        previewedTiles = new HashSet<Tile>();

        if(map != null)
        {
            GenerateBoard(map.texture);
        } else
        {
            GenerateBoard();
        }
        player = FindObjectOfType<Player>();
        MapBoard();
        Tile center = GetCenterTile();
        //Camera cam = Camera.main;
        //cam.transform.position = new Vector3(center.transform.position.x, center.transform.position.y, cam.transform.position.z);

        player.cam.camera.transform.position = new Vector3(center.transform.position.x,
                                                           Board.Instance.GetBoardTop() - 0.5f * player.cam.height,
                                                           player.cam.camera.transform.position.z);
    }

    public float GetBoardBottom()
    {
        return board == null ? 0 : board[board.GetLength(0) - 1, 0].transform.position.y;
    }

    public float GetBoardTop()
    {
        return board == null ? 0 : board[0, 0].transform.position.y;
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
                if (!coordToTile.ContainsKey(coord) ||
                    coordToTile[coord].GetTileType() != TileType.Preview)
                {
                    ClearPreviews();
                    return;
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

        bool canPlace = true;

        ISet<Tile> newTilesSet = new HashSet<Tile>();
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
                    //(currTile.GetTileType() != TileType.Normal && currTile.GetTileType() != TileType.Transparent))
                    (currTile.GetTileType() != TileType.Placeable))
                {
                    canPlace = false;
                    break;
                }
                newTilesSet.Add(currTile);
            }
        }

        // check if any of the tile in new piece is neighboring an existing piece
        ISet<Tile> pieceSet = pieceTuple.Item3;
        bool isNeighbor = pieceSet == null;

        foreach(Tile tile in newTilesSet)
        {
            if (isNeighbor) break; // all ready known that something is neighboring
            Vector2Int currentCoord = tileToCoord[tile];

            List<Vector2Int> neighborCoords = new List<Vector2Int>();
            if (coordToTile.ContainsKey(currentCoord + Vector2Int.up)) neighborCoords.Add(currentCoord + Vector2Int.up);
            if (coordToTile.ContainsKey(currentCoord + Vector2Int.down)) neighborCoords.Add(currentCoord + Vector2Int.down);
            if (coordToTile.ContainsKey(currentCoord + Vector2Int.left)) neighborCoords.Add(currentCoord + Vector2Int.left);
            if (coordToTile.ContainsKey(currentCoord + Vector2Int.right)) neighborCoords.Add(currentCoord + Vector2Int.right);

            foreach(Vector2Int vec in neighborCoords)
            {
                Tile neighborT = coordToTile[vec];
                if(neighborT.GetTileType() == TileType.Piece && pieceSet.Contains(neighborT))
                {
                    isNeighbor = true;
                    break;
                }
            }
        }

        
        if (!canPlace || !isNeighbor)
        {
            Destroy(p.gameObject);
            return;
        }

        Tile[,] newTilesPiece = new Tile[p.boundingBox.GetLength(0), p.boundingBox.GetLength(1)];
        ISet<Tile> newSetPiece = new HashSet<Tile>();
        //pieceDict[p] = tilesPiece;
        //pieceDict.Add(p, tilesPiece);
        (Piece, Tile[,], ISet<Tile>) newTuple = (p, newTilesPiece, newSetPiece);

        for (int y = placementPos.y, y1 = 0 ; y < p.boundingBox.GetLength(0) + placementPos.y; y++, y1++)
        {
            for (int x = placementPos.x, x1 = 0; x < p.boundingBox.GetLength(1) + placementPos.x; x++, x1++)
            {
                Tile currTile = coordToTile[new Vector2Int(x, y)];
                newTilesPiece[y1, x1] = currTile;
                if (p.boundingBox[y - placementPos.y, x - placementPos.x])
                {
                    currTile.SetTileType(TileType.Piece);
                    newSetPiece.Add(currTile);
                    //tilesPiece[y1, x1] = currTile;
                    currTile.SetCorrespondingPiece(p);
                }
            }
        }

        if (pieceTuple.Item1 == null) pieceTuple = newTuple;
        else
        {
            List<(Piece, Tile[,], ISet<Tile>)> list = new List<(Piece, Tile[,], ISet<Tile>)>();
            list.Add(pieceTuple); list.Add(newTuple);
            TryCombinePiece(list);
        }
        //Debug.Log("size " + tilesPiece.GetLength(0) + " " + tilesPiece.GetLength(1));
        //for(int i = 0; i < tilesPiece.GetLength(0); i++)
        //{
        //    for(int j = 0; j < tilesPiece.GetLength(1); j++)
        //    {
        //        Debug.Log(tilesPiece[i, j]);
        //    }
        //}
    }

    private void TryCombinePiece(List<(Piece, Tile[,], ISet<Tile>)> pieces)
    {
        int minX = width - 1, maxX = 0, minY = height - 1, maxY = 0;
        Piece initPiece = null;

        foreach((Piece, Tile[,], ISet<Tile>) pair in pieces)
        {
            Piece piece = pair.Item1;
            if (initPiece == null) initPiece = piece;
            Tile[,] tiles = pair.Item2;
            Vector2Int topLeft = tileToCoord[tiles[0, 0]];
            //Tile ti = tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1];
            //Debug.Log(ti);
            Vector2Int bottomRight = tileToCoord[tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1]];
            minX = Mathf.Min(minX, topLeft.x);
            maxX = Mathf.Max(maxX, bottomRight.x);
            minY = Mathf.Min(minY, topLeft.y);
            maxY = Mathf.Max(maxY, bottomRight.y);
        }

        int lengthX = maxX - minX + 1;
        int lengthY = maxY - minY + 1;

        Tile[,] newPieceTiles = new Tile[lengthY, lengthX];
        Piece newPiece = initPiece;
        ISet<Tile> tileSet = new HashSet<Tile>();
        newPiece.boundingBox = new bool[lengthY, lengthX];
        for (int y = minY, y1 = 0; y <= maxY; y++, y1++)
        {
            for(int x = minX, x1 = 0; x <= maxX; x++, x1++)
            {
                Tile t = board[y, x];
                newPieceTiles[y1, x1] = t;
                newPiece.boundingBox[y1, x1] = t.GetTileType() == TileType.Piece;
                tileSet.Add(t);
            }
        }

        pieceTuple = (newPiece, newPieceTiles, tileSet);

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
                tile.coord = new Vector2Int(x, (height - 1) - y);
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
        for (int i = 0; i < height; i++)
        {
            ClearRow(i);
        }
        
        if (startGame)
        {
            RemovePlaceable();
            MovePieceDown();
        }

    }

    public void RemovePlaceable()
    {
        foreach (Tile t in tileToCoord.Keys)
        {
            if (t.GetTileType() == TileType.Placeable)
            {
                t.SetTileType(TileType.Transparent);
            }
        }
    }

    private bool CanMove()
    {
        return pieceTuple.Item1 != null;
    }
    public void MovePieceDown()
    {
        if (!CanMove()) return;
        bool canMove = true;
        Tile[,] tiles = pieceTuple.Item2;
        int pieceHeight = tiles.GetLength(0);
        int pieceWidth = tiles.GetLength(1);

        // first pass to simulate
        for (int y = pieceHeight - 1; y >= 0 && canMove; y--)
        {
            //string st = "";
            for (int x = 0; x < pieceWidth && canMove; x++)
            {
                Tile tile = tiles[y, x];
                //st += coord.ToString() + ", ";
                if (tile == null) continue;

                Vector2Int coord = tileToCoord[tile];
                if (tile.GetTileType() == TileType.Piece)
                {
                    if (coord.y + 1 < height - 1)
                    {
                        Tile belowTile = coordToTile[new Vector2Int(coord.x, coord.y + 1)];

                        //Debug.Log(tile.coord + " " + belowTile.coord);
                        if (belowTile.GetTileType() == TileType.Wall || belowTile.GetTileType() == TileType.LevelPiece)
                        {
                            canMove = false;
                            break;
                        }
                        if (belowTile.GetTileType() != TileType.Normal &&
                            belowTile.GetTileType() != TileType.Transparent)
                        {
                            continue;
                        }

                    }
                }
            }
            //Debug.Log(st);
        }
        if (!canMove) return;

        for (int y = pieceHeight - 1; y >= 0 && canMove; y--)
        {
            //string st = "";
            for (int x = 0; x < pieceWidth && canMove; x++)
            {
                Tile tile = tiles[y, x];
                //st += coord.ToString() + ", ";
                if (tile == null) continue;

                Vector2Int coord = tileToCoord[tile];
                if (tile.GetTileType() == TileType.Piece)
                {
                    //IsContain(tiles, tile)
                    if (coord.y + 1 < height - 1)
                    {
                        Tile belowTile = coordToTile[new Vector2Int(coord.x, coord.y + 1)];
                        //Debug.Log(tile.coord + " " + belowTile.coord);
                        if (belowTile.GetTileType() != TileType.Normal &&
                            belowTile.GetTileType() != TileType.Transparent)
                        {
                            continue;
                        }
                        belowTile.SetTileType(tile.GetTileType());
                        belowTile.SetCorrespondingPiece(tile.GetCorrespondingPiece());
                        tiles[y, x] = belowTile;
                        tile.SetTileType(TileType.Normal);

                    }
                }
            }
        }



        //for(int y = height - 1; y >=0; y--)
        //{
        //    for(int x = 0; x < width; x++)
        //    {
        //        Tile tile = board[y, x];
        //        if(tile.GetTileType() == TileType.Piece)
        //        {
        //            if(y + 1 < height - 1)
        //            {
        //                Tile bellowTile = board[y + 1, x];
        //                if (bellowTile.GetTileType() != TileType.Normal &&
        //                    bellowTile.GetTileType() != TileType.Transparent)
        //                {
        //                    continue;
        //                }
        //                bellowTile.SetTileType(tile.GetTileType());
        //                bellowTile.SetCorrespondingPiece(tile.GetCorrespondingPiece());
        //                tile.SetTileType(TileType.Normal);


        //                //board[y + 1, x] = tile;
        //                //tile.SetTileType(TileType.Normal);
        //            }

        //        }
        //    }
        //}
        for (int i = 0; i < height; i++)
        {
            ClearRow(i);
        }
    }

    private bool IsValidMove(Vector2Int dir)
    {
        if (!CanMove()) return false;
        Tile[,] tiles = pieceTuple.Item2;

        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                Tile t = tiles[y, x];
                Vector2Int coords = tileToCoord[t];

                Vector2Int neighborCoord = coords + dir;
                if (!coordToTile.ContainsKey(neighborCoord))  //checking if coordinate is valid in board
                {
                    return false;
                }
                Tile neighborT = coordToTile[neighborCoord];
                TileType neighborType = neighborT.GetTileType();

                if (neighborType == TileType.Wall || 
                    neighborType == TileType.LevelPiece)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //public void MovePieceLeft()
    //{
    //    Vector2Int dir = Vector2Int.left;
    //    if (!IsValidMove(dir))
    //    {
    //        Debug.Log("can't move");
    //        return;
    //    }
    //    Debug.Log("can move");
    //    Tile[,] tiles = pieceTuple.Item2;

    //    // by column
    //    for (int x = 0; x < tiles.GetLength(1); x++)
    //    {
    //        for (int y = 0; y < tiles.GetLength(0); y++)
    //        {
    //            // curr tile
    //            Tile t = tiles[y, x];
    //            Vector2Int coords = tileToCoord[t];
    //            Vector2Int localCoords = new Vector2Int(x, y);
    //            TileType currType = t.GetTileType();

    //            // left tile
    //            Vector2Int leftCoords = coords + dir;
    //            Vector2Int leftLocalCoords = localCoords + dir;
    //            // This should never happen
    //            if (!coordToTile.ContainsKey(leftCoords))
    //            {
    //                Debug.LogError("TRIED TO MOVE BUT COULDN'T ABORT ABORT ABORT");
    //                return;
    //            }
    //            Tile leftT = coordToTile[leftCoords];
    //            TileType leftType = leftT.GetTileType();

    //            // reassign
    //            //leftT.SetTileType(currType);
    //            //leftT.SetCorrespondingPiece(t.GetCorrespondingPiece());

    //            //if (x == tiles.GetLength(1) - 1)
    //            //{
    //            //    t.SetTileType(TileType.Transparent);
    //            //    t.SetCorrespondingPiece(null);
    //            //}
    //            leftT.SetTileType(t.GetTileType());
    //            leftT.SetCorrespondingPiece(t.GetCorrespondingPiece());
    //            tiles[y, x] = leftT;
    //            t.SetTileType(TileType.Transparent);
    //        }
    //    }
    //}

    public void MovePieceLeft()
    {
        if (!CanMove()) return;
        bool canMove = true;
        Tile[,] tiles = pieceTuple.Item2;
        int pieceHeight = tiles.GetLength(0);
        int pieceWidth = tiles.GetLength(1);

        // first pass to simulate
        //for (int x = pieceWidth - 1; x >= 0 && canMove; x--)
        //{
        //    //string st = "";
        //    for (int y = 0; y < pieceHeight && canMove; y++)
        //    {
        //        Tile tile = tiles[y, x];
        //        //st += coord.ToString() + ", ";
        //        if (tile == null) continue;

        //        Vector2Int coord = tileToCoord[tile];
        //        if (tile.GetTileType() == TileType.Piece)
        //        {
        //            if (coord.x - 1 >= 0)
        //            {
        //                Tile rightTile = coordToTile[new Vector2Int(coord.x - 1, coord.y)];

        //                //Debug.Log(tile.coord + " " + belowTile.coord);
        //                if (rightTile.GetTileType() == TileType.Wall || rightTile.GetTileType() == TileType.LevelPiece)
        //                {
        //                    canMove = false;
        //                    break;
        //                }
        //                if (rightTile.GetTileType() != TileType.Normal &&
        //                    rightTile.GetTileType() != TileType.Transparent)
        //                {
        //                    continue;
        //                }

        //            }
        //        }
        //    }
        //    //Debug.Log(st);
        //}
        //if (!canMove) return;
        if (!IsValidMove(Vector2Int.left)) return;

        for (int x = 0; x < pieceWidth && canMove; x++)
        {
            //string st = "";
            for (int y = 0; y < pieceHeight && canMove; y++)
            {
                Tile tile = tiles[y, x];
                //st += coord.ToString() + ", ";
                if (tile == null) continue;

                Vector2Int coord = tileToCoord[tile];
                if (tile.GetTileType() == TileType.Piece)
                {
                    if (coord.x - 1 >= 0)
                    {
                        Tile leftTile = coordToTile[new Vector2Int(coord.x - 1, coord.y)];

                        //Debug.Log(tile.coord + " " + belowTile.coord);
                        if (leftTile.GetTileType() == TileType.Wall || leftTile.GetTileType() == TileType.LevelPiece)
                        {
                            canMove = false;
                            break;
                        }
                        if (leftTile.GetTileType() != TileType.Normal &&
                            leftTile.GetTileType() != TileType.Transparent)
                        {
                            continue;
                        }
                        leftTile.SetTileType(tile.GetTileType());
                        leftTile.SetCorrespondingPiece(tile.GetCorrespondingPiece());
                        tiles[y, x] = leftTile;
                        tile.SetTileType(TileType.Normal);
                    }
                }
            }
            //Debug.Log(st);
        }
        for (int i = 0; i < height; i++)
        {
            ClearRow(i);
        }
    }

    public void MovePieceRight()
    {
        if (!CanMove()) return;
        bool canMove = true;
        Tile[,] tiles = pieceTuple.Item2;
        int pieceHeight = tiles.GetLength(0);
        int pieceWidth = tiles.GetLength(1);

        // first pass to simulate
        for (int x = pieceWidth - 1; x >= 0 && canMove; x--)
        {
            //string st = "";
            for (int y = 0; y < pieceHeight && canMove; y++)
            {
                Tile tile = tiles[y, x];
                //st += coord.ToString() + ", ";
                if (tile == null) continue;

                Vector2Int coord = tileToCoord[tile];
                if (tile.GetTileType() == TileType.Piece)
                {
                    if (coord.x - 1 >= 0)
                    {
                        Tile rightTile = coordToTile[new Vector2Int(coord.x + 1, coord.y)];

                        //Debug.Log(tile.coord + " " + belowTile.coord);
                        if (rightTile.GetTileType() == TileType.Wall || rightTile.GetTileType() == TileType.LevelPiece)
                        {
                            canMove = false;
                            break;
                        }
                        if (rightTile.GetTileType() != TileType.Normal &&
                            rightTile.GetTileType() != TileType.Transparent)
                        {
                            continue;
                        }

                    }
                }
            }
            //Debug.Log(st);
        }
        if (!canMove) return;

        for (int x = pieceWidth - 1; x >= 0 && canMove; x--)
        {
            //string st = "";
            for (int y = 0; y < pieceHeight && canMove; y++)
            {
                Tile tile = tiles[y, x];
                //st += coord.ToString() + ", ";
                if (tile == null) continue;

                Vector2Int coord = tileToCoord[tile];
                if (tile.GetTileType() == TileType.Piece)
                {
                    if (coord.x - 1 >= 0)
                    {
                        Tile rightTile = coordToTile[new Vector2Int(coord.x + 1, coord.y)];

                        //Debug.Log(tile.coord + " " + belowTile.coord);
                        if (rightTile.GetTileType() == TileType.Wall || rightTile.GetTileType() == TileType.LevelPiece)
                        {
                            canMove = false;
                            break;
                        }
                        if (rightTile.GetTileType() != TileType.Normal &&
                            rightTile.GetTileType() != TileType.Transparent)
                        {
                            continue;
                        }
                        rightTile.SetTileType(tile.GetTileType());
                        rightTile.SetCorrespondingPiece(tile.GetCorrespondingPiece());
                        tiles[y, x] = rightTile;
                        tile.SetTileType(TileType.Normal);
                    }
                }
            }
            //Debug.Log(st);
        }
        for (int i = 0; i < height; i++)
        {
            ClearRow(i);
        }
    }

    private bool IsContain(Tile[,] tiles, Tile tile)
    {
        for(int i = 0; i < tiles.GetLength(0); i++)
        {
            for(int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tile == tiles[i, j]) return true;
            }
        }
        return false;
    }

    private void ClearRow(int row)
    {
        // Wall, piece, piece, ... , piece, Wall//
        TileType[] match = { TileType.Wall, TileType.Piece, TileType.LevelPiece, TileType.Wall };
        int i = 0;
        for (;i < width; i++)
        {
            Tile tile = board[row, i];
            //Debug.LogFormat("{0}, {1}",i, tile.GetTileType());
            if (i == 0)// first tile
            {
                if (tile.GetTileType() != match[0]) break;
            }
            
            if (i == width - 1)
            {
                if (tile.GetTileType() != match[3]) break;
            }
            
            if (i > 0 && i < width - 1)
            {
                if (tile.GetTileType() != match[1] && tile.GetTileType() != match[2]) break;
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
            Piece p = pieceTuple.Item1;
            Tile[,] tiles = pieceTuple.Item2;
            ISet<Tile> set = pieceTuple.Item3;
            if (tiles.GetLength(0) - 1 == 0)
            {
                pieceTuple = (null, null, null);
                return;
            }
            Tile[,] newTiles = new Tile[tiles.GetLength(0) - 1, tiles.GetLength(1)];
            ISet<Tile> newSet = new HashSet<Tile>();
            for (int x = 0; x < newTiles.GetLength(0); x++)
            {
                for(int y = 0; y < newTiles.GetLength(1); y++)
                {
                    newTiles[x, y] = tiles[x, y];
                    newSet.Add(tiles[x, y]);
                }
            }
            pieceTuple = (p, newTiles, newSet);
        }

    }

    public Tile[,] GetBoard()
    {
        return board;
    }
}
