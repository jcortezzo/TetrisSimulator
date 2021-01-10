using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { true,  true, true },
                                         { false, true, false } };
        ResetHeightAndWidth();
        this.color = Color.magenta;
    }

}
