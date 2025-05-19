using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    #region Variables

    public event Action<bool, int, int> OnCellStateSet;             // Delegate for informing others the cell state has been set

    public bool isAlive { get; private set; } = false;              // True when alive, false when dead
    public int posX { get; private set; }                           // X position of cell in the grid of cells
    public int posY { get; private set; }                           // Y position of cell in the grid of cells

    [SerializeField] private Color cellAliveColor = Color.yellow;   // Color of the cell when alive
    [SerializeField] private Color cellDeadColor = Color.grey;      // Color of the cell when dead

    private SpriteRenderer spriteRenderer;                          // SpriteRenderer component attached to gameobject
    private BoxCollider2D cellCollider;                             // BoxCollider2D component attached to gameobject

    #endregion

    #region UnityMethods

    private void Awake()
    {
        // Get the sprite renderer and set the color
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetCellColor();

        // Get the box collider
        cellCollider = GetComponent<BoxCollider2D>();
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

    public void OnPointerClick(PointerEventData eventData)
    {
        // Toggle the cell state when clicked
        ToggleCellState();
    }

    #endregion

    #region InitializationMethods

    /// <summary>
    /// Set the position of the cell in the grid of cells.
    /// </summary>
    /// <param name="_posX">X position of the cell.</param>
    /// <param name="_posY">Y position of the cell.</param>
    public void SetCellPosition(int _posX, int _posY)
    {
        // Set the x and y position values
        posX = _posX; 
        posY = _posY;
    }

    #endregion

    #region CellStateMethods

    /// <summary>
    /// Set the state of the cell.
    /// </summary>
    /// <param name="isAlive">True if alive, false, if dead.</param>
    public void SetCellState(bool isAlive)
    {
        // Update the isAlive bool and set the color
        this.isAlive = isAlive;
        SetCellColor();
        OnCellStateSet?.Invoke(isAlive, posX, posY);
    }

    /// <summary>
    /// Toggle the state of the cell.
    /// </summary>
    public void ToggleCellState()
    {
        // Toggle the isAlive bool and color
        isAlive = !isAlive;
        SetCellState(isAlive);
    }

    /// <summary>
    /// Set the color of the cell based on the current state.
    /// </summary>
    private void SetCellColor()
    {
        // Set the sprite color depending on the cell state
        spriteRenderer.color = isAlive ? cellAliveColor : cellDeadColor;
    }

    /// <summary>
    /// Disable the cells collider component.
    /// </summary>
    public void DisableCollider()
    {
        cellCollider.enabled = false;
    }

    /// <summary>
    /// Enable the cells collider component.
    /// </summary>
    public void EnableCollider()
    {
        cellCollider.enabled = true;
    }

    #endregion
}
