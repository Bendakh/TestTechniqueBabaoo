using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    //The index and x and y position in the puzzle
    int index = 0;
    int x = 0;
    int y = 0;

    float startPosX;
    float startPosY;
    bool isMoving = false;

    //Sound effect for moving the tiles
    [SerializeField]
    Sound moveSoundEffect;

    public int X { get => x; }
    public int Y { get => y; }
    public int Index { get => index; }

    //Sprite for the puzzle pieces
    [SerializeField]
    Sprite picture;

    public Sprite Picture { get => picture; }

    //Last position taken by the tile
    Vector3 lastPosition;

    //Initializing the puzzle piece informations
    public void InitializePuzzlePiece(int index, int x, int y, Sprite picture)
    {
        this.index = index;
        this.x = x;
        this.y = y;
        this.picture = picture;
        GetComponent<SpriteRenderer>().sprite = picture;
        UpdatePosition(x, y, false);
    }

    private void Update()
    {
        if(isMoving)
        {
            Vector3 touchPos;
            touchPos = Input.mousePosition;
            touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, -Camera.main.transform.position.z));

            this.gameObject.transform.localPosition = new Vector3(touchPos.x - startPosX, touchPos.y - startPosY, this.gameObject.transform.localPosition.z);
        }
    }

    

    public void UpdatePosition(int x, int y, bool smoothMove)
    {
        this.x = x;
        this.y = y;
        if (!smoothMove)
        {       
            gameObject.transform.localPosition = new Vector2(x * 2 * picture.bounds.extents.x, -y * 2 * picture.bounds.extents.y);
        }
        else
            StartCoroutine(MovePieceSmoothly());
    }

    //We can directly use OnMouseDown as it supports touch
    private void OnMouseDown()
    {
        if(GameManager._instance.IsPlaying)
        {
            lastPosition = this.transform.localPosition;
           
            //SwapPuzzlePiece(x, y, true);
            Vector3 touchPos;
            touchPos = Input.mousePosition;
            
            touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, -Camera.main.transform.position.z));
            

            startPosX = touchPos.x - this.transform.localPosition.x;
            startPosY = touchPos.y - this.transform.localPosition.y;

            isMoving = true;
        }
    }

    //What happens when we release the mouse
    private void OnMouseUp()
    {
        if (GameManager._instance.IsPlaying)
        {
            isMoving = false;

            if (new Vector2Int(x, y - 1) == GameManager._instance.EmptySlot || new Vector2Int(x, y + 1) == GameManager._instance.EmptySlot || new Vector2(x - 1, y) == GameManager._instance.EmptySlot || new Vector2(x + 1, y) == GameManager._instance.EmptySlot)
            {
                Vector2Int temp = new Vector2Int(x, y);
                if (Mathf.Abs(GameManager._instance.EmptySlot.x - x) <= 1f && Mathf.Abs(GameManager._instance.EmptySlot.y - y) <= 1f)
                {
                    UpdatePosition(GameManager._instance.EmptySlot.x, GameManager._instance.EmptySlot.y, false);
                    GameManager._instance.UpdateEmptySlot(temp);
                    AudioManager._instance.PlaySound(moveSoundEffect);
                }
                else
                {
                    this.transform.localPosition = lastPosition;
                }
            }
            else
            {
                this.transform.localPosition = lastPosition;
            }

            GameManager._instance.UpdateMirroredPuzzle();


            if (GameManager._instance.CheckIfPuzzleCompleted())
            {
                GameManager._instance.CompletePuzzle();
            }
        }
    }

    //Function that will swap the pieces of the puzzle
    //SmoothMove is true when the player moves the piece
    public void SwapPuzzlePiece(int x, int y, bool smoothMove)
    {
        //Check is empty slot is on top, on bottom, on the right or on the left
        if (new Vector2Int(x, y - 1) == GameManager._instance.EmptySlot || new Vector2Int(x, y + 1) == GameManager._instance.EmptySlot || new Vector2(x - 1, y) == GameManager._instance.EmptySlot || new Vector2(x + 1, y) == GameManager._instance.EmptySlot)
        {
            Vector2Int temp = new Vector2Int(x, y);
            UpdatePosition(GameManager._instance.EmptySlot.x, GameManager._instance.EmptySlot.y, smoothMove);
            GameManager._instance.UpdateEmptySlot(temp);
        }

        GameManager._instance.UpdateMirroredPuzzle();


        if (GameManager._instance.CheckIfPuzzleCompleted() && smoothMove)
        {
            GameManager._instance.CompletePuzzle();
        }
    }
    
    //A little animation that I used before I create the drag and drop
    IEnumerator MovePieceSmoothly()
    {
        float elapsedTime = 0;
        float duration = 0.2f;
        Vector2 start = gameObject.transform.localPosition;
        Vector2 end = new Vector2(x * 2 * picture.bounds.extents.x, -y * 2 * picture.bounds.extents.y);

        while(elapsedTime < duration)
        {
            gameObject.transform.localPosition = Vector2.Lerp(start, end, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.localPosition = end;
    }

    
}
