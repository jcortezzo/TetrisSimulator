using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { false, true, true },
                                         { true,  true, false } };
        ResetHeightAndWidth();
        this.color = Color.red;
    }

}
