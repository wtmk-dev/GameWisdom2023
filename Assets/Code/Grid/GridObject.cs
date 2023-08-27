using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GridObject : MonoBehaviour
{
    [HideInInspector]
    public GridTile CurrentPosition;
    public RectTransform _Model;

    public virtual bool IsLeftPlayer
    {
        get => _IsLeftPLayer;
        set
        {
            _IsLeftPLayer = value;
            if (!_IsLeftPLayer)
                FlipModel(_Model);
        }
    }

    public void Fade()
    {
        _Image.DOFade(.3f, .3f);
    }

    public virtual void DoMove(GridTile tile, Action callback = null)
    {
        if (CurrentPosition != null)
        {
            CurrentPosition.IsOccupied = false;
        }

        tile.IsOccupied = true;
        CurrentPosition = tile;

        transform.DOLocalMove(tile.transform.localPosition, 0.3f).OnComplete(() =>
        {
            
            CurrentPosition.IsOccupied = true;

            if (callback != null)
            {
                callback?.Invoke();
            }
        });
    }

    public virtual List<(int, int)> GridTiles(GridTile startingTile)
    {
        var tiles = new List<(int, int)>();
        tiles.Add(((int)startingTile.GridPosition.x, (int)startingTile.GridPosition.y + 1));
        //tiles.Add(((int)startingTile.GridPosition.x, (int)startingTile.GridPosition.y + 2));
        tiles.Add(((int)startingTile.GridPosition.x + 1, (int)startingTile.GridPosition.y + 1));
        tiles.Add(((int)startingTile.GridPosition.x + 1, (int)startingTile.GridPosition.y - 1));
        tiles.Add(((int)startingTile.GridPosition.x + 1, (int)startingTile.GridPosition.y));

        tiles.Add(((int)startingTile.GridPosition.x, (int)startingTile.GridPosition.y - 1));
        //tiles.Add(((int)startingTile.GridPosition.x, (int)startingTile.GridPosition.y - 2));
        tiles.Add(((int)startingTile.GridPosition.x - 1, (int)startingTile.GridPosition.y + 1));
        tiles.Add(((int)startingTile.GridPosition.x - 1, (int)startingTile.GridPosition.y - 1));
        tiles.Add(((int)startingTile.GridPosition.x - 1, (int)startingTile.GridPosition.y));

        return tiles;
    }

    public virtual void RefreshGrid(Dictionary<(int, int), GridTile> tileMap)
    {
        foreach (var tile in tileMap)
        {
            tile.Value.Refresh();
        }
    }

    protected bool _IsLeftPLayer;
    private Image _Image;

    void Start()
    {
        _Image = _Model.GetComponent<Image>();
    }

    private void FlipModel(RectTransform _Model)
    {
        _Model.rotation = new Quaternion(0f, 180f, 0f, 0f);
    }
}
