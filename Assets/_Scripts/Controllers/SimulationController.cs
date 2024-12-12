using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationController : MonoBehaviour 
{
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

    private void Awake()
    {
        SetResetTimer();
    }

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

    public void InitializeSimulationState(Cell[,] _cells)
    {
        // Set the cell and cell state arrays
        cells = _cells;
        cellStates = new int[cells.GetLength(0), cells.GetLength(1)];

        // Set the cell states and subscribe to their delegates
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                Cell _cell = cells[x, y];
                cellStates[x, y] = _cell.isAlive ? 1 : 0;
                _cell.OnCellStateChanged += Cell_OnStateChanged;
            }
        }
        
        // Update the cell counts
        totalCellCount = cells.GetLength(0) * cells.GetLength(1);
        UpdateCellStates();
        simulationSteps = 0;

        // Invoke the on simulation initialized event
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    private void Cell_OnStateChanged(Vector2 cellPosition, bool cellState)
    {
        // Update the cell states array
        cellStates[(int) cellPosition.x, (int) cellPosition.y] = cellState ? 1 : 0;

        // Update the dead/alive cell counts
        UpdateCellCounts();
    }

    private void UpdateCellCounts()
    {
        // Reset the dead/alive cell counts
        deadCellCount = 0;
        livingCellCount = 0;

        /* 
         * To find the number of living cells, just find the sum of all elements in the cellStates array. The zeros will not affect the sum so the total will be the number of living cells.
         * To find the number of dead cells, subtract the number of living from the total
         */
        for (int x = 0; x < cellStates.GetLength(0); x++) 
        {
            for (int y = 0; y < cellStates.GetLength(1); y++)
            {
                livingCellCount += cellStates[x, y];
            }
        }

        deadCellCount = totalCellCount - livingCellCount;

        // Invoke the on simulation state changed event
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    public void StartSimulation()
    {
        isSimulating = true;
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    public void PauseSimulation()
    {
        isSimulating = false;
        OnSimulationStateChanged?.Invoke(null, deadCellCount, livingCellCount, simulationSteps);
    }

    public void StepSimulation()
    {
        // Check if there are any living cells to work with
        if (livingCellCount == 0)
        {
            Debug.Log("Simulation step not completed as there aren't any living cells");
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
        UpdateCellStates(_indicesToToggle);

        // Update the simulation step count
        simulationSteps++;
        //Debug.Log($"Simulation step {simulationSteps}");
        //PrintCellStatesToConsole();
    }

    private void UpdateCellStates(List<List<int>> _indicesToToggle = null)
    {
        if (_indicesToToggle != null)
        {
            foreach (List<int> _indices in _indicesToToggle)
            {
                int x = _indices[0];
                int y = _indices[1];
                cells[x, y].ToggleCellState();
            }
        }

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cellStates[x, y] = cells[x, y].isAlive ? 1 : 0;
            }
        }

        UpdateCellCounts();
    }

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

    private void Update()
    {
        if (isSimulating)
        {
            secondsToUpdate -= Time.deltaTime;
            if (secondsToUpdate <= 0)
            {
                StepSimulation();
                ResetUpdateTimer();
            }
        }
    }
}

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
