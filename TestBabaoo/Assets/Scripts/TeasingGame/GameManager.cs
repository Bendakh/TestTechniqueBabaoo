using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Sound successSoundEffect;

    public static string BEST_SCORE = "BestScore";
    public static string BEST_SCORE_PLAYER = "BestScorePlayer";

    int bestScoreInSeconds;
    
    public int BestScoreInSeconds { get => bestScoreInSeconds; }

    public static GameManager _instance;

    [SerializeField]
    GameObject puzzlePlaceHolder;

    [SerializeField]
    List<PuzzleData> puzzleDatabase;

    PuzzleData puzzleToCreate;

    [SerializeField]
    GameObject puzzlePiecePrefab;

    [SerializeField]
    GameObject mirroredPuzzleHolder;

    private List<PuzzlePiece> piecesList = new List<PuzzlePiece>();

    private Vector2Int[] solution = new Vector2Int[8];

    [SerializeField]
    float timer;

    [SerializeField]
    float timerCountdown;

    public float Timer { get => timer; }
    public float TimerCountdown { get => timerCountdown; }

    private bool isPlaying = false;

    public bool IsPlaying { get => isPlaying; }

    //The minimum and maximum number of shuffle moves for the initial puzzle randomization
    [SerializeField]
    int minShuffleMoves;
    [SerializeField]
    int maxShuffleMoves;

    Vector2Int emptySlot = new Vector2Int();

    public Vector2Int EmptySlot { get => emptySlot; }

    private void Awake()
    {
        if (_instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        ConfigGame();
    }

    private void Update()
    {
        if(isPlaying)
        {
            timerCountdown -= Time.deltaTime;
        }

        if(timerCountdown <= 0)
        {
            timerCountdown = 0;
            Defeat();
        }

    }

    private void SetTimer()
    {
        timerCountdown = timer;
    }

    //Launch the game
    public void LaunchTimer()
    {
        isPlaying = true;
    }

    //Save the solution of the puzzle
    private Vector2Int[] SaveSolution()
    {
        Vector2Int[] solutionArray = new Vector2Int[8];
        for (int i = 0; i < solution.Length; i++)
        {
            solutionArray[i] = new Vector2Int(piecesList[i].X, piecesList[i].Y);
        }

        return solutionArray;
    }

    //Configure the game 
    public void ConfigGame()
    {
        solution = new Vector2Int[8];

        for (int i = 0; i < piecesList.Count; i++)
        {
            Destroy(piecesList[i].gameObject);
        }

        piecesList.Clear();

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            puzzleToCreate = puzzleDatabase[1];
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            puzzleToCreate = puzzleDatabase[0];
        }
        //We can eventually use other platforms but let's put by default the android puzzle
        else
        {
            puzzleToCreate = puzzleDatabase[0];
        }
            

        puzzleToCreate.DisplayPuzzle();

        solution = SaveSolution();

        ShufflePieces();

        SetTimer();

        if (PlayerPrefs.HasKey(BEST_SCORE))
        {
            bestScoreInSeconds = PlayerPrefs.GetInt(BEST_SCORE);
        }
        else
        {
            bestScoreInSeconds = int.MaxValue;
        }

        UIManager._instance.DisplayStartGameButton();

    }

    //Function that will generte the puzzle and display it 
    public void DisplayPuzzle(List<Sprite> puzzlePieces)
    {
        int index = 0;
        int x = 0;
        int y = 0;
        foreach(Sprite puzzlePiece in puzzlePieces)
        {
            //We don't generate the central piece
            if (index != 4)
            {
                PuzzlePiece piece = Instantiate(puzzlePiecePrefab, puzzlePlaceHolder.transform).GetComponent<PuzzlePiece>();
                piece.InitializePuzzlePiece(index, x, y, puzzlePiece);
                piecesList.Add(piece);
            }

            index++;

            //Here we define the (x,y) position of the next puzzle piece
            x++;

            if(x > 2)
            {
                y++;
                x = 0;
            }
        }

        emptySlot = new Vector2Int(1, 1);
    }

    //A copy of the puzzle with all the pieces displayed in the screen
    public void UpdateMirroredPuzzle()
    {
        //We clear the puzzle container
        SpriteRenderer[] allChildren = mirroredPuzzleHolder.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in allChildren)
        {
            Destroy(sr.gameObject);
        }

        //And we repopulate it with the updated puzzle
        for (int i = 0; i < piecesList.Count + 1; i++)
        {
            if (i == piecesList.Count)
            {
                //Creating the middle piece
                PuzzlePiece middlePiece = Instantiate(puzzlePiecePrefab, mirroredPuzzleHolder.transform).GetComponent<PuzzlePiece>();
                middlePiece.InitializePuzzlePiece(8, emptySlot.x, emptySlot.y, puzzleDatabase[0].PuzzlePieces[4]);
                middlePiece.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                PuzzlePiece piece = Instantiate(puzzlePiecePrefab, mirroredPuzzleHolder.transform).GetComponent<PuzzlePiece>();
                piece.InitializePuzzlePiece(piecesList[i].Index, piecesList[i].X, piecesList[i].Y, piecesList[i].GetComponent<SpriteRenderer>().sprite);
                piece.GetComponent<BoxCollider2D>().enabled = false;
            }           
        }  
    }
  
    //We update the empty slot new position
    public void UpdateEmptySlot(Vector2Int newVector)
    {
        emptySlot = newVector;
    }

    //Puzzle pieces randomization
    void ShufflePieces()
    {
        int shuffleMoves = Random.Range(minShuffleMoves, maxShuffleMoves + 1);

        for(int i = 0; i < shuffleMoves; i++)
        {
            List<PuzzlePiece> validPiecesToMove = SelectValidPiecesToMove();
            int randomPieceIndex = Random.Range(0, validPiecesToMove.Count);
            validPiecesToMove[randomPieceIndex].SwapPuzzlePiece(validPiecesToMove[randomPieceIndex].X, validPiecesToMove[randomPieceIndex].Y, false);
        }
    }

    //Function that will select valid pieces to move in the randomization
    List<PuzzlePiece> SelectValidPiecesToMove()
    {
        List<PuzzlePiece> piecesToReturn = new List<PuzzlePiece>();

        foreach(PuzzlePiece puzzlePiece in piecesList)
        {
            //Select the upper, lower, right and left pieces to move if it's possible
            if ((puzzlePiece.X == emptySlot.x + 1 && puzzlePiece.Y == emptySlot.y) || (puzzlePiece.X == emptySlot.x - 1 && puzzlePiece.Y == emptySlot.y ) || (puzzlePiece.Y == emptySlot.y + 1 && puzzlePiece.X == emptySlot.x) || (puzzlePiece.Y == emptySlot.y - 1 && puzzlePiece.X == emptySlot.x))
                piecesToReturn.Add(puzzlePiece);
        }

        return piecesToReturn;
    }

    //Function that will check if the puzzle is completed
    public bool CheckIfPuzzleCompleted()
    {
        bool puzzleCompleted = true;

        for (int i = 0; i < solution.Length; i++)
        {
            if (!ComparePieceToSolution(solution[i], piecesList[i]))
            {
                puzzleCompleted = false;
                break;
            }
        }
        
        return puzzleCompleted;
    }

    //The player won the game
    public void CompletePuzzle()
    {
        isPlaying = false;
        AudioManager._instance.PlaySound(successSoundEffect);
        UIManager._instance.DisplayEndGamePanel(true);
        if((int)(timer - timerCountdown) < bestScoreInSeconds)
        {
            bestScoreInSeconds = (int)(timer - timerCountdown);
            UIManager._instance.DisplaySaveScorePanel();
        }
    }

    //The player lost the game
    public void Defeat()
    {
        isPlaying = false;
        UIManager._instance.DisplayEndGamePanel(false);
    }

    //Compare a piece position to it's solution
    private bool ComparePieceToSolution(Vector2Int pieceSolution, PuzzlePiece piece)
    {
        return (pieceSolution.x == piece.X && pieceSolution.y == piece.Y);
    }
}
