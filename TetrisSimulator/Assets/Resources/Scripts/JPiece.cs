using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPiece : Piece
{
    protected override void Awake()
    {
        this.boundingBox = new bool[,] { { true, false, false },
                                         { true, true,  true } };
        ResetHeightAndWidth();
    }
}
