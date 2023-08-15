using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridTile : MonoBehaviour, IPointerClickHandler
{
    public event Action<GridTile> OnClick;

    [SerializeField]
    private Vector2 _GridPosition;
    public Vector2 GridPosition => _GridPosition;
    public bool IsOccupied { get; set; }

    private Image _Image;

    public void DoClick()
    {
        OnClick?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnClick?.Invoke(this);
    }

    private void Awake()
    {
        _Image = GetComponent<Image>();

        _SelectedColor = new Color(25f, 25f, 128f);
        _DefaultColor = _Image.color;
    }

    private Color _DefaultColor;
    private Color _PreviousColor;
    private Color _SelectedColor;

    public void Select()
    {
        _PreviousColor = _Image.color;
        _Image.color = _SelectedColor;
    }

    public void Refresh()
    {
        _Image.color = _DefaultColor;
    }

    public void Highlight()
    {
        _Image.color = new Color(125f, 125f, 0f, .3f);
    }

    public void BounceTarget()
    {
        _Image.color = new Color(200f, 0f, 50f, .3f);
    }
}
