using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleData", menuName = "TeasingGame/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    [SerializeField]
    List<Sprite> puzzlePieces;

    public List<Sprite> PuzzlePieces { get => puzzlePieces; }

    public void DisplayPuzzle()
    {
        if(puzzlePieces.Count > 0)
        {
            GameManager._instance.DisplayPuzzle(puzzlePieces);
        }
    }
}
