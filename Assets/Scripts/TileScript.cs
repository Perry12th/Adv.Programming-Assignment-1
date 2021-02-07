using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TileState { EMPTY, QUARTER, HALF, MAX };
public class TileScript : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField]
    Image icon;

    [Header("Tile Stats")]
    bool hasBeenModded;
    public bool isVisable;
    public int row, col;
    public MinigameScript miniGame;
    public TileState state;

    [Header("Tile Class Stats")]
    [SerializeField]
    Color emptyColor;
    [SerializeField]
    Color quarterColor;
    [SerializeField]
    Color halfColor;
    [SerializeField]
    Color maxColor;
    [SerializeField]
    int emptyValue;
    [SerializeField]
    int quarterValue;
    [SerializeField]
    int halfValue;
    [SerializeField]
    int maxValue;
    [SerializeField]
    Sprite diamondSprite;
    [SerializeField]
    Sprite questionMarkSprite;




    void Start()
    {
        miniGame = transform.parent.GetComponent<MinigameScript>();
    }

    public void SetGridIndices(int r, int c)
    {
        row = r;
        col = c;
    }

    public void SetState(TileState newState)
    {
        state = newState;
    }

    public void RaiseState(TileState newState)
    {
        if (newState > state)
        {
            state = newState;
        }
    }
    public void SetVisability(bool visable)
    {
        isVisable = visable;

        if (isVisable)
        {
            icon.sprite = diamondSprite;
            switch (state)
            {
                case TileState.EMPTY:
                    icon.color = emptyColor;
                    break;
                case TileState.QUARTER:
                    icon.color = quarterColor;
                    break;
                case TileState.HALF:
                    icon.color = halfColor;
                    break;
                case TileState.MAX:
                    icon.color = maxColor;
                    break;
            }
        }
        else
        {
            icon.sprite = questionMarkSprite;
            icon.color = Color.white;
        }

    }

    public int Extract()
    {
        switch (state)
        {
            case TileState.EMPTY:
                return emptyValue;
            case TileState.QUARTER:
                SetState(TileState.EMPTY);
                SetVisability(true);
                return quarterValue;
            case TileState.HALF:
                SetState(TileState.EMPTY);
                SetVisability(true);
                return halfValue;
            case TileState.MAX:
                SetState(TileState.EMPTY);
                SetVisability(true);
                return maxValue;
            default:
                return 0;
        }

    }

    public void Degrade()
    {
        switch (state)
        {
            case TileState.EMPTY:
                break;
            case TileState.QUARTER:
                SetState(TileState.EMPTY);
                break;
            case TileState.HALF:
                SetState(TileState.QUARTER);
                break;
            case TileState.MAX:
                SetState(TileState.HALF);
                break;
            default:
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (miniGame.mode)
        {
            case (MinigameMode.Scan):
                miniGame.RevealTile(row, col);
                break;

            case (MinigameMode.Extract):
                miniGame.ExtractTile(row, col);
                break;
        }

    }
}
