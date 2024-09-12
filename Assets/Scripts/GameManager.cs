using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Chestpiece;  // Reference to the Chesspiece prefab

    private GameObject[,] positions = new GameObject[8, 8];  // 2D array representing the board and pieces
    private GameObject[] playerBlack = new GameObject[16];   // Array holding all black pieces
    private GameObject[] playerWhite = new GameObject[16];   // Array holding all white pieces

    private string currentPlayer = "white";  // Tracks the current player, starts with white

    private bool gameOver = false;  // Tracks if the game is over

    // Start is called before the first frame update
    public void Start()
    {
        // Initialize the board with white and black pieces in their starting positions
        playerWhite = new GameObject[] { Create("white_rook", 0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0), Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1) };

        playerBlack = new GameObject[] { Create("black_rook", 0, 7), Create("black_knight",1,7),
            Create("black_bishop",2,7), Create("black_queen",3,7), Create("black_king",4,7),
            Create("black_bishop",5,7), Create("black_knight",6,7), Create("black_rook",7,7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6) };

        // Set initial positions on the board for all pieces
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    // Called once per frame to handle game reset on user input
    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            // Reloads the game scene to restart the game
            SceneManager.LoadScene("Game");
        }
    }

    // Create a new chess piece at given board coordinates
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(Chestpiece, new Vector3(0, 0, -1), Quaternion.identity);  // Instantiate the piece
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;  // Set the piece's name
        cm.SetXBoard(x);  // Set its X position on the board
        cm.SetYBoard(y);  // Set its Y position on the board
        cm.Activate();  // Call the piece's Activate() function to initialize it
        return obj;
    }

    // Set the piece's position in the positions array based on its board coordinates
    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;  // Place the piece at the correct board position
    }

    // Mark a specific position on the board as empty
    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    // Get the piece at a specific position on the board
    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    // Check if a position is within the bounds of the board
    public bool PositionOnBoard(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1));
    }

    // Switch to the next player's turn
    public void NextTurn()
    {
        if(gameOver) { return; }

        if (currentPlayer == "white")
        {
            currentPlayer = "black";  // Switch to black player's turn
            GetComponent<ChessAI>().MakeAIMove();  // Call AI to make a move for black
        }
        else
        {
            currentPlayer = "white";  // Switch back to white player's turn
        }
    }

    // Get the current player (either "white" or "black")
    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    // Check if the game is over
    public bool IsGameOver()
    {
        return gameOver;
    }

    // Declare the winner and display end-game UI
    public void Winner(string playerWinner)
    {
        gameOver = true;

        // Display the winner text on the UI
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = playerWinner + " is the winner";

        // Display the restart instruction text on the UI
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<TextMeshProUGUI>().enabled = true;
    }

    // Move a piece to a new position on the board (with optional attack flag)
    public void MovePiece(GameObject piece, Vector2Int newPosition, bool isAttack = false)
    {
        StartCoroutine(MovePieceCoroutine(piece, newPosition, isAttack));  // Move the piece asynchronously
    }

    // Coroutine to handle the movement of a piece on the board
    private IEnumerator MovePieceCoroutine(GameObject piece, Vector2Int newPosition, bool isAttack)
    {
        Chessman chessman = piece.GetComponent<Chessman>();
        int oldX = chessman.GetXBoard();  // Store the current X position
        int oldY = chessman.GetYBoard();  // Store the current Y position
        chessman.GetComponent<Chessman>().hasMoved = true;

        // Check if the new position is within the board's bounds
        if (newPosition.x < 0 || newPosition.x >= 8 || newPosition.y < 0 || newPosition.y >= 8)
        {
            Debug.LogError("Move out of board bounds");
            yield break;
        }

        // If it's an attack, destroy the piece at the target position
        if (isAttack)
        {
            GameObject pieceAtDestination = GetPosition(newPosition.x, newPosition.y);
            if (pieceAtDestination != null)
            {
                Chessman destinationChessman = pieceAtDestination.GetComponent<Chessman>();

                // Check if the captured piece is the king to declare a winner
                if (destinationChessman.name == "white_king") Winner("black");
                if (destinationChessman.name == "black_king") Winner("white");

                // Destroy the captured piece
                Destroy(pieceAtDestination);
            }
        }

        // Clear the old position on the board
        positions[oldX, oldY] = null;

        // Update the piece's board position to the new coordinates
        chessman.SetXBoard(newPosition.x);
        chessman.SetYBoard(newPosition.y);

        // Convert board coordinates to Unity world coordinates for positioning in the game
        Vector3 unityPosition = new Vector3(ConvertToUnityCoord(newPosition.x), ConvertToUnityCoord(newPosition.y), -1);
        piece.transform.position = unityPosition;  // Move the piece visually in Unity

        // Update the board's internal tracking with the new position
        positions[newPosition.x, newPosition.y] = piece;

        // Wait for a short duration to ensure smooth transition before changing the turn
        yield return new WaitForSeconds(0.3f);

        // Switch to the next player's turn
        NextTurn();
    }

    // Converts board coordinates (0-7) to Unity world coordinates
    private float ConvertToUnityCoord(int boardCoord)
    {
        return -2.3f + boardCoord * 0.66f;
    }

    // Stack to record move history for undo functionality (if needed)
    private Stack<MoveRecord> moveHistory = new Stack<MoveRecord>();

    // Internal class to track moves for undo functionality
    private class MoveRecord
    {
        public GameObject piece;  // The piece that was moved
        public Vector2Int originalPosition;  // The position the piece moved from
        public Vector2Int targetPosition;  // The position the piece moved to

        public MoveRecord(GameObject piece, Vector2Int originalPosition, Vector2Int targetPosition)
        {
            this.piece = piece;
            this.originalPosition = originalPosition;
            this.targetPosition = targetPosition;
        }
    }
}