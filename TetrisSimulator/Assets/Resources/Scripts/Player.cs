using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Tile selection;
    [SerializeField] private Queue<Piece> pieces;
    [SerializeField] private Piece tempPieceForTesting;

    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Piece currentPiece;

    public LayerMask layerMask;

    private bool wasRotated;

    // Start is called before the first frame update
    void Start()
    {
        pieces = new Queue<Piece>();
        //pieces.Enqueue(new JPiece());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPiece == null)
        {
            currentPiece = Instantiate(piecePrefab).GetComponent<Piece>();
        }
        RotatePiece();
        SimpleMouseOver();
        if (Input.GetMouseButtonDown(0) && selection != null)
        {
            Debug.Log(tempPieceForTesting);
            Debug.Log(selection);

            //Board.Instance.PlacePiece(/*pieces.Dequeue()*/ Instantiate(tempPieceForTesting), selection);
            Board.Instance.PlacePiece(currentPiece, selection);
        }
    }

    private void RotatePiece()
    {
        bool rotated = false;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentPiece.RotateLeft();
            this.wasRotated = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentPiece.RotateRight();
            this.wasRotated = true;
        }

        if (rotated)
        {
            //selection.Unselect();
            //selection = null;  // kinda hacky but lets the reselection happen
        }
    }

    private RaycastHit2D MouseRayCast()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, layerMask);
        return hit;
    }

    protected void SimpleMouseOver()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.GetPoint(0), ray.direction, Color.red);
            RaycastHit2D hit2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit2.collider != null)
            {
                //Debug.Log("Raycast hit!");
                //if (selection != null) selection.Unselect();
                Tile t = hit.transform.GetComponent<Tile>();
                if (t != null && (selection != t || wasRotated))
                {
                    //Debug.Log("Found Tile object");
                    if (selection != null)
                    {
                        selection.Unselect();
                        // TODO: Clear preview tiles
                    }
                    Board.Instance.ClearPreviews();
                    //Board.Instance.PreviewPiece(Instantiate(tempPieceForTesting), t);
                    Board.Instance.PreviewPiece(currentPiece, t);
                    //t.Preview();
                    selection = t;
                    t.Select();
                }
            }
        }

    }
}
