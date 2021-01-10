using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZPiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { true,  true, false },
                                         { false, true, true } };
        ResetHeightAndWidth();
        this.color = Color.green;
    }

}
