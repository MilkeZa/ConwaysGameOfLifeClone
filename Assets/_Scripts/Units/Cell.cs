using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public Action<Vector2, bool> OnCellStateChanged;

    public bool isAlive { get; private set; } = false;
    public Vector2 cellPosition;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color cellAliveColor = Color.yellow;
    [SerializeField] private Color cellDeadColor = Color.grey;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetCellColor();
    }

    public void SetCellState(bool isAlive)
    {
        // Update the isAlive bool and set the color
        this.isAlive = isAlive;
        SetCellColor();

        // Invoke the on cell state changed action
        OnCellStateChanged?.Invoke(cellPosition, isAlive);
        //Debug.Log($"Cell ({(int)cellPosition.x}, {(int)cellPosition.y}) changed state");
    }

    public void ToggleCellState()
    {
        // Toggle the isAlive bool and color
        isAlive = !isAlive;
        SetCellState(isAlive);
    }

    private void SetCellColor()
    {
        // Set the sprite color depending on the cell state
        spriteRenderer.color = isAlive ? cellAliveColor : cellDeadColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Toggle the cell state when clicked
        ToggleCellState();
    }
}
