using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { true, true, true, true } };
        ResetHeightAndWidth();
        this.color = Color.cyan;
    }

}
