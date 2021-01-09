using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private Tile[,] board;
    [SerializeField] private Tile tilePrefab;

    public int width;
    public int height;

    public float discoTime;

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
        board = new Tile[height, width];
        GenerateBoard();
    }

    public void GenerateBoard()
    {
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

    public void GenerateBoard(Texture2D levelTexture)
    {

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
