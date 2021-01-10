using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { false, false, true },
                                         { true, true,  true } };
        ResetHeightAndWidth();
        this.color = Color.gray;
    }

}
