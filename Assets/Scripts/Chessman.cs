using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;  // Reference to the GameManager (the controller of the game)
    public GameObject movePlate;   // Reference to the move plate prefab

    private int xBoard = -1;  // X position of the piece on the board
    private int yBoard = -1;  // Y position of the piece on the board

    private string player;    // Represents which player this piece belongs to

    // Sprites for each type of piece for both black and white
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public bool isWhite;      // True if the piece is white, false if it's black

    public bool hasMoved = false;  // Track if a piece has moved (for the pawn's first double move)

    // Triggered when the player clicks on the piece
    private void OnMouseUp()
    {
        // Check if the game is still ongoing and if it's the player's turn
        if (!controller.GetComponent<GameManager>().IsGameOver() && controller.GetComponent<GameManager>().GetCurrentPlayer() == player)
        {
            // Remove all existing move plates on the board
            DestroyMovePlates();

            // Generate new move plates for this piece
            GetAvailableMoves();
        }
    }

    // Called to initialize the chess piece's position and appearance
    public void Activate()
    {
        // Find the GameController (GameManager) object
        controller = GameObject.FindGameObjectWithTag("GameController");

        // Determine if this piece is white or black
        isWhite = GetIsWhite();

        // Set the piece's coordinates on the board
        SetCoords();

        // Assign the correct sprite and player based on the piece's name
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    // Set the actual position of the piece on the Unity board using its board coordinates
    public void SetCoords()
    {
        // Convert board coordinates to Unity world position
        float x = xBoard;
        float y = yBoard;

        // Adjust coordinates with an offset factor for proper alignment on the board
        x *= 0.66f;
        y *= 0.66f;

        // Further adjust by adding constants to get the correct position
        x += -2.3f;
        y += -2.3f;

        // Set the piece's actual position in the Unity world
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    // Getters and setters for the board coordinates
    public int GetXBoard() { return xBoard; }
    public int GetYBoard() { return yBoard; }
    public void SetXBoard(int x) { xBoard = x; }
    public void SetYBoard(int y) { yBoard = y; }

    // Destroy all move plates on the board (called when a new piece is selected)
    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    // Generate all valid moves for this piece
    public void GetAvailableMoves()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;
        }
    }

    // Generate move plates for pawns, including a special move of two squares if it hasn't moved yet
    public void PawnMovePlate(int x, int y)
    {
        GameManager sc = controller.GetComponent<GameManager>();

        if (sc.PositionOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                // Standard move forward one square
                MovePlateSpawn(x, y);

                // If the pawn hasn't moved yet, allow it to move two squares forward
                if (!hasMoved)
                {
                    int yDoubleMove = isWhite ? y + 1 : y - 1;  // If white, move up; if black, move down
                    if (sc.PositionOnBoard(x, yDoubleMove) && sc.GetPosition(x, yDoubleMove) == null)
                    {
                        MovePlateSpawn(x, yDoubleMove);  // Spawn move plate for the double move
                    }
                }
            }

            // Check for diagonal captures (pawns capture pieces diagonally)
            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    // Generate move plates in a line (for queens, rooks, bishops)
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        GameManager sc = controller.GetComponent<GameManager>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        // Continue in a straight line until the board boundary or an occupied square is reached
        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        // If an enemy piece is encountered, spawn an attack move plate
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    // Generate move plates for knights (L-shaped moves)
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    // Helper function to generate move plates at a specific board position
    public void PointMovePlate(int x, int y)
    {
        GameManager sc = controller.GetComponent<GameManager>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    // Generate move plates around the king (one square in any direction)
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    // Spawn a move plate at the given board coordinates
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        // Convert board coordinates to Unity world coordinates
        float x = matrixX;
        float y = matrixY;

        // Adjust by offsets
        x *= 0.66f;
        y *= 0.66f;

        // Further adjust by adding constants for the base position
        x += -2.3f;
        y += -2.3f;

        // Instantiate the move plate at the calculated position
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);  // Set this piece as the reference for the move plate
        mpScript.SetCoords(matrixX, matrixY);  // Set the board coordinates for the move plate
    }

    // Spawn an attack move plate at the given board coordinates
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        // Convert board coordinates to Unity world coordinates
        float x = matrixX;
        float y = matrixY;

        // Adjust by offsets
        x *= 0.66f;
        y *= 0.66f;

        // Further adjust by adding constants for the base position
        x += -2.3f;
        y += -2.3f;

        // Instantiate the move plate at the calculated position
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;  // Mark this move plate as an attack move
        mpScript.SetReference(gameObject);  // Set this piece as the reference for the move plate
        mpScript.SetCoords(matrixX, matrixY);  // Set the board coordinates for the move plate
    }

    // Determine if the piece is white based on its name
    public bool GetIsWhite()
    {
        if (gameObject.name.Contains("white"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Get the position of the piece on the board as a Vector2Int
    public Vector2Int GetPosition()
    {
        return new Vector2Int(GetXBoard(), GetYBoard());
    }
}