using System.Collections.Generic;
using UnityEngine;

public class ChessAI : MonoBehaviour
{
    // Information about a potential move: the piece, target move plate, capture value, and defense value
    public class MoveInfo
    {
        public GameObject piece; // The piece that will make the move
        public GameObject movePlate; // The target move plate
        public int Value; // Value associated with a capture
    }

    // Gets the value of a piece, used to evaluate captures
    private int GetPieceValue(Chessman chessman)
    {
        switch (chessman.name)
        {
            case "black_queen":
            case "white_queen":
                return 90;
            case "black_rook":
            case "white_rook":
                return 50;
            case "black_bishop":
            case "white_bishop":
                return 30;
            case "black_knight":
            case "white_knight":
                return 30;
            case "black_pawn":
            case "white_pawn":
                return 10;
            case "black_king":
            case "white_king":
                return 1000;
            default:
                return 0;
        }
    }

    // Evaluates the value of a capture based on the target position
    private int EvaluatePieceValue(Vector2Int targetPosition)
    {
        int Value = 0;
        GameManager gameManager = GetComponent<GameManager>();

        // Get the piece at the target position
        GameObject targetPiece = gameManager.GetPosition(targetPosition.x, targetPosition.y);
        if (targetPiece != null)
        {
            Chessman targetChessman = targetPiece.GetComponent<Chessman>();
            Value = GetPieceValue(targetChessman);
        }

        return Value;
    }

    // Evaluates all possible moves and selects the best one
    public void MakeAIMove()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("ChessPiece");
        List<MoveInfo> moveInfos = new List<MoveInfo>();

        foreach (GameObject piece in pieces)
        {
            Chessman chessman = piece.GetComponent<Chessman>();

            if (!chessman.isWhite) // Only consider black pieces for AI
            {
                chessman.GetAvailableMoves(); // Get possible moves
                List<GameObject> movePlates = FindMovePlates(chessman); // Find available move plates
                chessman.DestroyMovePlates(); // Clean up the move plates
                foreach (GameObject movePlate in movePlates)
                {
                    MoveInfo moveInfo = EvaluateMove(piece, movePlate); // Evaluate each move
                    moveInfo.piece = piece;
                    moveInfo.movePlate = movePlate;
                    moveInfos.Add(moveInfo);
                }
            }
        }

        // If there are no available moves, return
        if (moveInfos.Count == 0) return;

        // Sort the moveInfos list based on the Value property of each move.
        // Moves with higher Value come first (descending order).
        moveInfos.Sort((a, b) =>
        {
            if (b.Value != a.Value)
                return b.Value.CompareTo(a.Value);

            return 0; // If they are equal, no change in order
        });


        // Select the best move
        MoveInfo bestMoveInfo = moveInfos[0];
        MovePlate bestMovePlate = bestMoveInfo.movePlate.GetComponent<MovePlate>();
        GameObject pieceToMove = bestMoveInfo.piece;

        // Make the move using the GameManager's MovePiece method
        GetComponent<GameManager>().MovePiece(pieceToMove, new Vector2Int(bestMovePlate.matrixX, bestMovePlate.matrixY), bestMovePlate.attack);
    }

    // Evaluates a move based on its capture, defense, and neutral values
    private MoveInfo EvaluateMove(GameObject piece, GameObject movePlate)
    {
        MoveInfo moveInfo = new MoveInfo();
        MovePlate plate = movePlate.GetComponent<MovePlate>();

        // Assume the target position is that of the move plate
        Vector2Int targetPosition = new Vector2Int(plate.matrixX, plate.matrixY);

        // Evaluate the value
        moveInfo.Value = EvaluatePieceValue(targetPosition);

        // Assign a random neutral value if no capture or defense is involved
        if (moveInfo.Value == 0 && moveInfo.Value == 0)
        {
            moveInfo.Value = Random.Range(1, 10); // Assign a random value for neutral moves
        }

        return moveInfo;
    }

    // Finds all move plates associated with a given piece
    private List<GameObject> FindMovePlates(Chessman chessman)
    {
        List<GameObject> movePlates = new List<GameObject>();
        GameObject[] movePlatesArray = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject plate in movePlatesArray)
        {
            MovePlate mp = plate.GetComponent<MovePlate>();
            // Add move plates that belong to the specified chessman
            if (mp.GetReference() == chessman.gameObject)
            {
                movePlates.Add(plate);
            }
        }
        return movePlates;
    }
}