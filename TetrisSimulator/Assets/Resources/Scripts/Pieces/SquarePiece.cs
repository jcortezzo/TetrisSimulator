using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { true, true },
                                         { true, true } };
        ResetHeightAndWidth();
        color = Color.yellow;
    }

}
