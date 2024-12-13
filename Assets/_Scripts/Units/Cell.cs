using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public event Action<bool, int, int> OnCellStateSet;

    public bool isAlive { get; private set; } = false;
    public int posX { get; private set; }
    public int posY { get; private set; }

    [SerializeField] private Color cellAliveColor = Color.yellow;
    [SerializeField] private Color cellDeadColor = Color.grey;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D cellCollider;

    private void Awake()
    {
        // Get the sprite renderer and set the color
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetCellColor();

        // Get the box collider
        cellCollider = GetComponent<BoxCollider2D>();
    }

    public void SetCellPosition(int _posX, int _posY)
    {
        // Set the x and y position values
        posX = _posX; 
        posY = _posY;
    }

    public void SetCellState(bool isAlive)
    {
        // Update the isAlive bool and set the color
        this.isAlive = isAlive;
        SetCellColor();
        OnCellStateSet?.Invoke(isAlive, posX, posY);
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

    public void DisableCollider()
    {
        cellCollider.enabled = false;
    }

    public void EnableCollider()
    {
        cellCollider.enabled = true;
    }

    private void OnBecameInvisible()
    {
        // Disable the cell collider as it can no longer be clicked. This improves performance considerably as colliders are taxing.
        DisableCollider();
    }

    private void OnBecameVisible()
    {
        // Enable the cell collider as it may now be clicked
        EnableCollider();
    }
}
