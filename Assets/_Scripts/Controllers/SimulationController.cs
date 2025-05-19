using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SimulationController : MonoBehaviour 
{
    #region Variables

    public event Action<int?, int, int, int> OnSimulationStateChanged;  // Called anytime the state of the simulation changes

    private Cell[,] cells;      // 2D array of cells
    private int[,] cellStates;  // 2D array where 0's are dead cells and 1's are living cells

    public int totalCellCount { get; private set; }         // Total number of cells
    public int deadCellCount { get; private set; }          // Number of dead cells
    public int livingCellCount { get; private set; }        // Number of living cells

    public bool isSimulating { get; private set; } = false; // Indicates whether the simulation is currently running
    public int simulationSteps { get; private set; } = 0;   // Number of steps simulation has taken since start

    [SerializeField] private SimulationSpeed simulationSpeed = SimulationSpeed.Medium;
    private float secondsToUpdateBase;  // Base value used to set seconds to update
    private float secondsToUpdate;      // Number of seconds simulation will wait before taking another step

    [SerializeField] private bool drawBoundingBox = false;  // Draws a bounding box surrounding the living cells in the scene. WARNING: Enabling has a considerable performance impact as it iterates over the cell state map constantly
    private LineRenderer bbLineRenderer;                    // Used to draw the bounding box surrounding living cells
    private bool updateBoundingBox = false;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        // Initialize the reset timer
        SetResetTimer();

        // Get the line renderer component
        bbLineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // Continue the simulation if enabled
        if (isSimulating)
        {
            // Check if the simulation timer has expired
            secondsToUpdate -= Time.deltaTime;
            if (secondsToUpdate <= 0)
            {
                // Step the simulation
                StepSimulation();

                // Reset the timer
                ResetUpdateTimer();
            }

            // Update the bounding box if enabled
            if (drawBoundingBox && updateBoundingBox)
            {
                // Draw the bounding box
                DrawBoundingBox();

                // Reset the draw flag
                updateBoundingBox = false;
            }
        }
    }

    #endregion

    #region TimerMethods

    private void SetResetTimer()
    {
        switch (simulationSpeed)
        {
            case SimulationSpeed.VerySlow:
                secondsToUpdateBase = 5f;
                break;
            case SimulationSpeed.Slow:
                secondsToUpdateBase = 2.5f;
                break;
            case SimulationSpeed.MediumSlow:
                secondsToUpdateBase = 1f;
                break;
            case SimulationSpeed.Medium:
                secondsToUpdateBase = 0.5f;
                break;
            case SimulationSpeed.MediumFast:
                secondsToUpdateBase = 0.25f;
                break;
            case SimulationSpeed.Fast:
                secondsToUpdateBase = 0.1f;
                break;
            case SimulationSpeed.VeryFast:
                secondsToUpdateBase = 0.05f;
                break;
        }

        secondsToUpdate = secondsToUpdateBase;
    }

    private void ResetUpdateTimer()
    {
        secondsToUpdate = secondsToUpdateBase;
        //Debug.Log($"Simulation Update Timer Reset!");
    }

    #endregion

    #region SimulationMethods

    /// <summary>
    /// Initialize the simulation.
    /// </summary>
    /// <param name="_cells">2D array containing cells to simulate.</param>
    public void InitializeSimulationState(Cell[,] _cells)
    {
        // Set the cell and cell state arrays
        cells = _cells;
        cellStates = new int[cells.GetLength(0), cells.GetLength(1)];

        // Subscribe to the cells delegates
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x, y].OnCellStateSet += UpdateCellState;
            }
        }

        // Set the cell states and update their counts
        totalCellCount = cells.GetLength(0) * cells.GetLength(1);
        UpdateCellStateMap();
        
        // Set the simulation steps count
        simulationSteps = 0;

        // Invoke the on simulation initialized event
        OnSimulationStateChanged?.Invoke(totalCellCount, deadCellCount, livingCellCount, simulationSteps);
    }

    /// <summary>
    /// Start the simulation, causing it to evaluate the next step and beyond.
    /// </summary>
    public void StartSimulation()
    {
        isSimulating = true;
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    /// <summary>
    /// Pause the simulation, stopping it from evaluating the next step.
    /// </summary>
    public void PauseSimulation()
    {
        isSimulating = false;
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    /// <summary>
    /// Evaluate one step of the simulation.
    /// </summary>
    public void StepSimulation()
    {
        // Check if there are any living cells to work with
        if (livingCellCount == 0)
        {
            Debug.Log("Simulation step not completed as there aren't any living cells");
            isSimulating = false;
            return;
        }

        // Evaluate the game rules, iterating through each cell in the grid
        List<List<int>> _indicesToToggle = new List<List<int>>();
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                // Evaluate the cell based on its neighbors
                if (EvaluateCell(cells[x, y], GetCellNeighbors(x, y, cells)))
                {
                    _indicesToToggle.Add(new List<int>() { x, y });
                }
            }
        }

        // Update the cell states and counts
        UpdateCellMap(_indicesToToggle);
        UpdateCellStateMap();

        // Update the simulation step count
        simulationSteps++;

        // Set the bounding box flag
        updateBoundingBox = true;

        //Debug.Log($"Simulation step {simulationSteps}");
        //PrintCellStatesToConsole();
    }

    /// <summary>
    /// Update the living/dead cell statistic.
    /// </summary>
    /// <param name="_isAlive">True if a cell is alive, otherwise, false.</param>
    /// <param name="_posX">X position of the given cell.</param>
    /// <param name="_posY">Y position of the given cell.</param>
    private void UpdateCellState(bool _isAlive, int _posX, int _posY)
    {
        if (_isAlive)
        {
            cellStates[_posX, _posY] = 1;
            livingCellCount++;
            deadCellCount--;
        }
        else
        {
            cellStates[_posX, _posY] = 0;
            livingCellCount--;
            deadCellCount++;
        }
    }

    /// <summary>
    /// Update the map containing each cell.
    /// </summary>
    /// <param name="_indicesToToggle">Indices of cells whose state are to be toggled.</param>
    private void UpdateCellMap(List<List<int>> _indicesToToggle)
    {
        // Toggle each of the cells whose indices were passed
        foreach (List<int> _indices in _indicesToToggle)
        {
            int x = _indices[0];
            int y = _indices[1];
            cells[x, y].ToggleCellState();
        }

        // Invoke the on simulation state changed event
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    /// <summary>
    /// Update the map containing the state of each cell.
    /// </summary>
    private void UpdateCellStateMap()
    {
        // Reset the cell counts
        deadCellCount = 0;
        livingCellCount = 0;

        // Iterate over each cell in the map
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                // Update the cell state value and increment the associated value
                if (cells[x, y].isAlive)
                {
                    cellStates[x, y] = 1;
                    livingCellCount++;
                }
                else
                {
                    cellStates[x, y] = 0;
                    deadCellCount++;
                }
            }
        }
    }

    /// <summary>
    /// Get a given cells surrounding cells.
    /// </summary>
    /// <param name="_posX">X position of the cell to be evaluated.</param>
    /// <param name="_posY">Y position of the cell to be evaluated.</param>
    /// <param name="_cells">Grid of cells being evaluated.</param>
    /// <returns>Array of neighboring cells.</returns>
    private Cell[] GetCellNeighbors(int _posX, int _posY, Cell[,] _cells)
    {
        int _rowCount = _cells.GetLength(0);
        int _colCount = _cells.GetLength(1);

        // List to hold the indices of each existing neighbor
        List<List<int>> _neighborIndices = new List<List<int>>();

        // Left neighbor
        if (_posX > 0)
        {
            _neighborIndices.Add(new List<int>() { _posX - 1, _posY });
        }

        // Right neighbor
        if (_posX < _colCount - 1)
        {
            _neighborIndices.Add(new List<int>() { _posX + 1, _posY });
        }

        // Top neighbor
        if (_posY > 0)
        {
            _neighborIndices.Add(new List<int>() { _posX, _posY - 1 });
        }

        // Bottom neighbor
        if (_posY < _rowCount - 1)
        {
            _neighborIndices.Add(new List<int>() { _posX, _posY + 1 });
        }

        // Top left neighbor
        if (_posX > 0 && _posY > 0) 
        {
            _neighborIndices.Add(new List<int>() { _posX - 1, _posY - 1 });
        }

        // Top right neighbor
        if (_posX < _colCount - 1 && _posY > 0)
        {
            _neighborIndices.Add(new List<int>() { _posX + 1, _posY - 1 });
        }

        // Bottom left neighbor
        if (_posX > 0 && _posY < _colCount - 1)
        {
            _neighborIndices.Add(new List<int>() { _posX - 1, _posY + 1 });
        }

        // Bottom right neighbor
        if (_posX < _colCount - 1 &&  _posY < _rowCount - 1)
        {
            _neighborIndices.Add(new List<int>() { _posX + 1, _posY + 1 });
        }

        Cell[] _neighbors = new Cell[_neighborIndices.Count];
        for (int i = 0; i < _neighborIndices.Count; i++)
        {
            _neighbors[i] = _cells[_neighborIndices[i][0], _neighborIndices[i][1]];
        }

        return _neighbors;
    }

    /// <summary>
    /// Evaluate the cell state.
    /// </summary>
    /// <param name="_cell">Cell to be evaluated.</param>
    /// <param name="_neighbors">Each of the cells surrounding neighbors.</param>
    /// <returns>True if the cell should be marked living, otherwise, false.</returns>
    private bool EvaluateCell(Cell _cell, Cell[] _neighbors)
    {
        // Count the number of living and dead neighbors
        int _totalNeighborCount = _neighbors.Length;
        int _livingNeighborCount = 0;
        int _deadNeighborCount = 0;

        foreach (Cell _neighbor in _neighbors)
        {
            if (_neighbor.isAlive)
            {
                _livingNeighborCount++;
            }
        }
        _deadNeighborCount = _totalNeighborCount - _livingNeighborCount;
        //Debug.Log($"Cell ({(int)_cell.cellPosition.x}, {(int)_cell.cellPosition.y}) has {_livingNeighborCount} living and {_deadNeighborCount} dead neighbors");

        /* 
         * For cells that are alive
         *  Rule 1: Each cell with one or no neighbors dies, as if by solitude.
         *  Rule 2: Each cell with four or more neighbors dies, as it by overpopulation.
         *  Rule 3: Each cell with two or three neighbors survives.
         *  
         * For cells that are not alive
         *  Rule 1: Each cell with three neighbors becomes populated.
         */
        if (_cell.isAlive)
        {
            if (_livingNeighborCount <= 1)
            {
                return true;
            }

            if (_livingNeighborCount >= 4)
            {
                return true;
            }

            if (2 <= _livingNeighborCount && _livingNeighborCount <= 3)
            {
                return false;
            }
        }
        else
        {
            if (_livingNeighborCount == 3)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Set the state of the bounding box renderer.
    /// </summary>
    /// <param name="_state">Enabled when true, otherwise, disabled.</param>
    public void SetBoundingBoxRendererState(bool _state)
    {
        if (_state)
        {
            // Enable the bounding box line renderer component
            bbLineRenderer.enabled = true;
            ResetBoundingBox();
        }
        else
        {
            // Disable the line renderer component
            bbLineRenderer.enabled = false;
            ResetBoundingBox();
        }
    }

    /// <summary>
    /// Draws a bounding box around the living cells within the scene.
    /// </summary>
    private void DrawBoundingBox()
    {
        // Minimum x and y positions of the living cells
        float _minX = float.MaxValue;
        float _minY = float.MaxValue;

        // Maximum x and y positions of the living cells
        float _maxX = float.MinValue;
        float _maxY = float.MinValue;

        // Iterate through each row
        for (int x = 0; x < cellStates.GetLength(0); x++)
        {
            // Iterate through each col
            for (int y = 0; y < cellStates.GetLength(1); y++)
            {
                // Check if the cell is alive
                if (cells[x, y].isAlive)
                {
                    // Grab the cell position in world space
                    Vector3 _cellPos = cells[x, y].transform.position;

                    // Compare the x positions
                    if (_cellPos.x < _minX)
                    {
                        _minX = _cellPos.x;
                    }
                    
                    if (_cellPos.x > _maxX)
                    {
                        _maxX = _cellPos.x;
                    }

                    // Compare the y positions
                    if (_cellPos.y < _minY)
                    {
                        _minY = _cellPos.y;
                    }
                    
                    if (_cellPos.y > _maxY)
                    {
                        _maxY = _cellPos.y;
                    }
                }
            }
        }

        // Pass the points along to the line renderer to be drawn
        bbLineRenderer.SetPositions(new Vector3[]
        {
            new Vector3(_minX, _minY, -1f),  // Bottom left
            new Vector3(_minX, _maxY, -1f),  // Top Left
            new Vector3(_maxX, _maxY, -1f),  // Top Right
            new Vector3(_maxX, _minY, -1f)   // Bottom Right
        });
    }

    /// <summary>
    /// Reset the bounding box.
    /// </summary>
    private void ResetBoundingBox()
    {
        // Set the number of positions to zero
        bbLineRenderer.positionCount = 0;
    }

    #endregion

    #region DebugMethods

    /// <summary>
    /// Outputs the cell state map to the console log.
    /// </summary>
    private void PrintCellStatesToConsole()
    {
        string _cellStates = "";
        for (int y = cellStates.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < cellStates.GetLength(0); x++)
            {
                _cellStates += String.Format("  {0, 0}  ", cellStates[x, y] == 0 ? '_' : 'x');
            }
            _cellStates += '\n';
        }
        Debug.Log(_cellStates);
    }

    #endregion
}

/// <summary>
/// Struct used to control speed at which simulation runs. Faster speeds may impact performance.
/// </summary>
public enum SimulationSpeed
{
    VerySlow = 0,
    Slow = 1,
    MediumSlow = 2,
    Medium = 3,
    MediumFast = 4,
    Fast = 5,
    VeryFast = 6,
};
