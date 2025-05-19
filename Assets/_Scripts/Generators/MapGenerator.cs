using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform cellPrefab;          // Prefab used when instantiating cells.
    [SerializeField] private float cellScale;               // Size of cell.
    [SerializeField] private int cellCountX, cellCountY;    // Number of cells on the x and y axes.

    #endregion

    #region GenerationMethods

    /// <summary>
    /// Generate the 2D array of cells (map) to be used for the simulation.
    /// </summary>
    /// <returns>2D array of cell objects.</returns>
    public Cell[,] GenerateMap()
    {
        // 2D array to hold the cell objects
        Cell[,] _cellMap = new Cell[cellCountX, cellCountY];

        // Iterate through each index in the array
        for (int x = 0; x < cellCountX; x++)
        {
            for (int y = 0; y < cellCountY; y++)
            {
                // Calculate the position for the cell and scale it using the map scale param
                Vector3 cellPosition = new Vector3((-cellCountX / 2 + 0.5f + x) * cellScale, (-cellCountY / 2 + 0.5f + y) * cellScale, transform.position.z);

                // Instantiate a new cell object as a transform, then scale it using the map scale param to fit each cell in tidy
                Transform newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                newCell.localScale = new Vector3(cellScale, cellScale, 1);

                // Get the cell component, set the position, and insert it into the 2D cell array
                Cell _cell = newCell.gameObject.GetComponentInChildren<Cell>();
                _cell.SetCellPosition(x, y);
                _cellMap[x, y] = _cell;
            }
        }

        // Return the cell array
        return _cellMap;
    }

    /// <summary>
    /// Generate the 2D array of cells.
    /// </summary>
    /// <param name="_seed">Random seed used when generating the array.</param>
    /// <param name="_livingProbability">Probability a cell will be alive when generated.</param>
    /// <returns></returns>
    public Cell[,] GenerateMapFromSeed(int _seed, float _livingProbability)
    {
        // Initialize the map
        Cell[,] _cellMap = GenerateMap();

        // Generate a random map using the seed and probability of living
        int[,] _cellStateMap = GenerateCellStateMap(_seed, _livingProbability);

        // Set the cell states using the cell state map
        for (int x = 0; x < _cellStateMap.GetLength(0); x++)
        {
            for (int y = 0; y < _cellStateMap.GetLength(1); y++)
            {
                if (_cellStateMap[x, y] == 1)
                {
                    _cellMap[x, y].ToggleCellState();
                }
            }
        }

        return _cellMap;
    }

    /// <summary>
    /// Generate the cell state map.
    /// </summary>
    /// <param name="_seedNumber">Random seed used when generating the state map.</param>
    /// <param name="_livingProbability">Probability of a cell being alive when generated.</param>
    /// <returns></returns>
    public int[,] GenerateCellStateMap(int _seedNumber, float _livingProbability)
    {
        // Use the seed number to create a new random object to help with the obvious
        System.Random _randomizer = new System.Random(_seedNumber);

        // 2D array matching the x/y dimensions
        int[,] _cellStates = new int[cellCountX, cellCountY];

        // Check if the chance of any cell being alive is greater than 0
        if (_livingProbability <= 0f)
        {
            return _cellStates;
        }

        // Iterate through the array setting either 0's (dead) or 1's (alive) using the living probability
        for (int x = 0; x < cellCountX; x++)
        {
            for (int y = 0; y < cellCountY; y++)
            {
                // Generate a random value in the range [0, 1]
                float _randomValue = GenerateRandomFloat01(_randomizer);
                _cellStates[x, y] = _randomValue >= (1 - _livingProbability) ? 1 : 0;
            }
        }

        // Return the cell states array
        return _cellStates;
    }

    /// <summary>
    /// Generate a random float in the range [0.0, 1.0]
    /// </summary>
    /// <param name="_random"></param>
    /// <returns></returns>
    private float GenerateRandomFloat01(System.Random _random)
    {
        return MapFloat01((float)_random.Next(NumberCruncher.randomMinInt, NumberCruncher.randomMaxInt), NumberCruncher.randomMinInt, NumberCruncher.randomMaxInt);
    }

    #endregion

    #region UtilityMethods

    /// <summary>
    /// Map a float between 0 and 1.
    /// </summary>
    /// <param name="_value">Value to be mapped.</param>
    /// <param name="_minFrom">Minimum value in range.</param>
    /// <param name="_maxFrom">Maximum value in range.</param>
    /// <returns></returns>
    private float MapFloat01(float _value, float _minFrom, float _maxFrom)
    {
        return (_value - _minFrom) * 1f / (_maxFrom - _minFrom);
    }

    #endregion
}
