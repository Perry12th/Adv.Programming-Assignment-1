using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum MinigameMode { NotStarted, Scan, Extract, Finished }
public class MinigameScript : MonoBehaviour
{

    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI messages;
    [SerializeField]
    Image modeIcon;
    [SerializeField]
    TextMeshProUGUI modeText;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    Sprite extractSprite;
    [SerializeField]
    Sprite scanSprite;

    [Header("Mini-game Info")]
    [SerializeField]
    int startScanAmount;
    [SerializeField]
    int startExtractAmount;
    int score;
    int scansLeft;
    int extractsLeft;
    bool isGameOver;
    public MinigameMode mode = MinigameMode.NotStarted;
    GameObject[] tiles = new GameObject[1024];
    [SerializeField]
    int NumberOfMaxTiles;
    [SerializeField]
    GameObject tileBase;

    public void Start()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = Instantiate(tileBase);
            tiles[i].transform.SetParent(transform);
            int tileNum = (i + 1);
            int rowNum = tileNum > 32 ? tileNum % 32 : tileNum;
            int colNum = tileNum > 32 ? (tileNum / 32) : 0;
            if (tileNum % 32 == 0)
            {
                rowNum = 32;
                if (colNum != 0)
                {
                    colNum--;
                }
            }
            TileScript tileScript = tiles[i].GetComponent<TileScript>();
            tileScript.SetGridIndices(rowNum, colNum);
        }

        messages.text = "Welcome to this little tile base game. Objective is to scan titles to find the most valuable titles to get the highest score. Press the Start button to begin.";
    }
    public void ResetGame()
    {
        foreach(GameObject tile in tiles)
        {
            TileScript script = tile.GetComponent<TileScript>();
            script.SetState(TileState.EMPTY);
            script.SetVisability(false);
        }
        for (int i = 0; i < NumberOfMaxTiles; i ++)
        {
            int row = Random.Range(1, 32);
            int col = Random.Range(0, 31);

            SetTileMaterial(row, col);
        }
        scansLeft = startScanAmount;
        extractsLeft = startExtractAmount;
        score = 0;
        scoreText.text = "Score: " + score;
        mode = MinigameMode.Extract;
        messages.text = "Extracts Left: " + extractsLeft;
        modeText.text = "Extract Mode";
        modeIcon.sprite = extractSprite;
    }

    public void SetTileMaterial(int row, int col)
    {
        for (int c = col - 2; c <= col + 2; c++)
        {
            for (int r = row - 2; r <= row + 2; r++)
            {
                if ((r >= 1 && r <= 32) && (c >= 0 && c <= 31))
                {
                    TileScript tile = tiles[(c * 32 + r) - 1].GetComponent<TileScript>();
                    if (r == row && c == col)
                    {
                        tile.RaiseState(TileState.MAX);
                    }
                    else
                    {
                        float distance = Mathf.Sqrt(  (Mathf.Pow(r - row, 2) + Mathf.Pow(c - col, 2) ) );

                        if (distance >= 2.0f)
                        {
                            tile.RaiseState(TileState.QUARTER);
                        }
                        else
                        {
                            tile.RaiseState(TileState.HALF);
                        }
                    }
                }
            }
        }
    }

    public void RevealTile(int row, int col)
    {
        if (scansLeft > 0)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                for (int r = row - 1; r <= row + 1; r++)
                {
                    if ( (r >= 1 && r <= 32) && (c >= 0 && c <= 31) )
                    {
                        TileScript tile = tiles[(c * 32 + r) - 1].GetComponent<TileScript>();
                        tile.SetVisability(true);
                    }
                }
            }

            scansLeft--;
            messages.text = "Scans Left: " + scansLeft;
        }
        else
        {
            messages.text = "You have no more scans left";
        }
    }

    public void ExtractTile(int row, int col)
    {
        if (extractsLeft > 0)
        {
            for (int c = col - 2; c <= col + 2; c++)
            {
                for (int r = row - 2; r <= row + 2; r++)
                {
                    if ((r >= 1 && r <= 32) && (c >= 0 && c <= 31))
                    {
                        TileScript tile = tiles[(c * 32 + r) - 1].GetComponent<TileScript>();
                        if (row == r && col == c)
                        {
                            score += tile.Extract();
                            scoreText.text = "Score: " + score;
                            tile.SetVisability(true);

                        }
                        else
                        {
                            tile.Degrade();
                            if (tile.isVisable)
                            {
                                tile.SetVisability(true);
                            }
                        }
                    }
                }
            }

            extractsLeft--;
            if (extractsLeft <= 0)
            {
                mode = MinigameMode.Finished;
                messages.text = "Game Over, you have no more extracts left, please press the start button to play again.";
            }
            else
            {
                messages.text = "Extracts Left: " + extractsLeft;
            }
        }
    }

    public void ToggleMode()
    {
        if (mode.Equals(MinigameMode.Extract))
        {
            mode = MinigameMode.Scan;
            messages.text = "Scans Left: " + scansLeft;
            modeText.text = "Scan Mode";
            modeIcon.sprite = scanSprite;

        }
        else if (mode.Equals(MinigameMode.Scan))
        {
            mode = MinigameMode.Extract;
            messages.text = "Extracts Left: " + extractsLeft;
            modeText.text = "Extract Mode";
            modeIcon.sprite = extractSprite;
        }
    }
}
