using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;  // Reference to the GameController (GameManager)
    GameObject reference = null;   // Reference to the piece that generated this MovePlate
    public int matrixX;  // X position on the chessboard matrix (0-7)
    public int matrixY;  // Y position on the chessboard matrix (0-7)
    public bool attack = false;  // True if this MovePlate indicates an attack move

    // Start is called before the first frame update
    public void Start()
    {
        // If this MovePlate represents an attack move, change its color to red
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);  // Set color to red
        }
    }

    // This method is called when the player clicks on the MovePlate
    public void OnMouseUp()
    {
        // Find the GameController object using its tag
        controller = GameObject.FindGameObjectWithTag("GameController");

        // If this is an attack move, tell the GameManager to move the piece and attack
        if (attack)
        {
            controller.GetComponent<GameManager>().MovePiece(reference, new Vector2Int(matrixX, matrixY), true);
        }
        else
        {
            // Otherwise, just move the piece without attacking
            controller.GetComponent<GameManager>().MovePiece(reference, new Vector2Int(matrixX, matrixY), false);
        }

        // After the piece has moved, destroy all move plates associated with this piece
        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    // Sets the reference to the piece (Chessman) that created this MovePlate
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    // Returns the reference to the piece (Chessman) that created this MovePlate
    public GameObject GetReference()
    {
        return reference;
    }

    // Sets the coordinates of this MovePlate on the chessboard matrix
    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }
}