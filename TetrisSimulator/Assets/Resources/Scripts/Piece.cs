using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private bool[,] boundingBox;
    [SerializeField] int boundingBoxHeight;
    [SerializeField] int boundingBoxWidth;

    void Awake()
    {
        boundingBox = new bool[boundingBoxHeight, boundingBoxWidth];
        //boundingBox[0, 0] = true;
        //boundingBox[0, 1] = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateRight();
        }
    }

    public Vector2Int GetCenter()
    {
        return new Vector2Int(this.boundingBox.GetLength(0) / 2,
                              this.boundingBox.GetLength(1) / 2);
    }

    private void RotateRight()
    {
        Transpose();
        for (int y = 0; y < this.boundingBox.GetLength(0); y++)
        {
            int i = 0;
            int j = this.boundingBox.GetLength(1) - 1;
            
            while (i <= j)
            {
                bool temp = this.boundingBox[y, i];
                this.boundingBox[y, i] = this.boundingBox[y, j];
                this.boundingBox[y, j] = temp;
                i++; j--;
            }
        }
        Debug.Log(this);
    }

    private void RotateLeft()
    {
        Transpose();
        for (int x = 0; x < this.boundingBox.GetLength(1); x++)
        {
            int i = 0;
            int j = this.boundingBox.GetLength(0) - 1;

            while (i <= j)
            {
                bool temp = this.boundingBox[i, x];
                this.boundingBox[i, x] = this.boundingBox[j, x];
                this.boundingBox[j, x] = temp;
                i++; j--;
            }
        }
        Debug.Log(this);
    }

    private void Transpose()
    {
        bool[,] transposed = new bool[this.boundingBox.GetLength(1), this.boundingBox.GetLength(0)];

        for (int y = 0; y < this.boundingBox.GetLength(0); y++)
        for (int x = 0; x < this.boundingBox.GetLength(1); x++)
        {
                transposed[x, y] = this.boundingBox[y, x];
        }

        this.boundingBox = transposed;
    }

    public override string ToString()
    {
        string s = "\n";
        for (int y = 0; y < this.boundingBox.GetLength(0); y++)
        {
            s += "[";
            for (int x = 0; x < this.boundingBox.GetLength(1); x++)
            {
                s += this.boundingBox[y, x] + ", ";
            }
            s = s.Substring(0, s.Length - 2) + "]";// + "\n";
        }
        return s;
    }
}
