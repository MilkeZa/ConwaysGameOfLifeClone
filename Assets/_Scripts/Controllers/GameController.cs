using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    #region Variables

    [SerializeField] private MapGenerator mapGenerator;                 // Generates the cell maps
    [SerializeField] private SimulationController simulationController; // Controls the simulations performed on the cell map
    [SerializeField] private UIController uiController;                 // Controls the user interface
    [SerializeField] private CameraController cameraController;         // Controls the camera zoom/movement

    #endregion

    #region UnityMethods

    private void Start()
    {
        // Initialize the simulation
        InitializeSimulation();
    }

    #endregion

    #region SimulationControlMethods

    /// <summary>
    /// Initialize the simulation state.
    /// </summary>
    public void InitializeSimulation()
    {
        // Subscribe to the simulation controllers delegates
        simulationController.OnSimulationStateChanged += InitializeSimulationCellCountText;
        simulationController.OnSimulationStateChanged += UpdateSimulationStatistics;

        Cell[,] _cells = GenerateCellMap(GetRandomSeedFromUI(), GetLivingProbabilityFromUI());
        simulationController.InitializeSimulationState(_cells);
    }

    /// <summary>
    /// Give initial simulation state values to UI Controller.
    /// </summary>
    /// <param name="_totalCellCount">Total number of cells.</param>
    /// <param name="_deadCellCount">Number of dead cells.</param>
    /// <param name="_livingCellCount">Number of living cells.</param>
    /// <param name="_simulationSteps">Simulation step count.</param>
    private void InitializeSimulationCellCountText(int? _totalCellCount, int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        // Verify the total cell count is present
        if (_totalCellCount != null && _totalCellCount.HasValue)
        {
            // Update the UI text and unsubscribe from the delegate
            uiController.InitializeSimulationStatistics(_totalCellCount.Value, _deadCellCount, _livingCellCount, _simulationSteps);

        }

        // Unsubscribe from the delegate
        simulationController.OnSimulationStateChanged -= InitializeSimulationCellCountText;
    }

    /// <summary>
    /// Update the statistics shown regarding the simulation state.
    /// </summary>
    /// <param name="_">Throwaway variable, a result of the delegate used.</param>
    /// <param name="_deadCellCount">Number of dead cells.</param>
    /// <param name="_livingCellCount">Number of living cells.</param>
    /// <param name="_simulationSteps">Current simulation step.</param>
    private void UpdateSimulationStatistics(int? _, int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        uiController.UpdateSimulationStatistics(_deadCellCount, _livingCellCount, _simulationSteps);
    }

    /// <summary>
    /// Toggle the state of the simulation.
    /// </summary>
    public void ToggleSimulationState()
    {
        // Get the current state of the simulation and reverse it
        bool _simulationState = !simulationController.isSimulating;

        // Set the state
        SetSimulationState(_simulationState);
    }

    /// <summary>
    /// Set the state of the simulation.
    /// </summary>
    /// <param name="_simulationState">True when the simulation is started, false, when paused.</param>
    private void SetSimulationState(bool _simulationState)
    {
        if (_simulationState)
        {
            simulationController.StartSimulation();
        }
        else
        {
            simulationController.PauseSimulation();
        }

        // Update the user interface toggle simulation button text
        uiController.UpdateToggleSimulationButtonText(_simulationState);
        //Debug.Log("Simulation state changed");
    }

    /// <summary>
    /// Reset the simulation to its initial state.
    /// </summary>
    public void ResetSimulation()
    {
        // Pause the simulation state
        SetSimulationState(false);

        // Subscribe the set initialization text function
        simulationController.OnSimulationStateChanged += InitializeSimulationCellCountText;

        // Initialize the simulation state
        InitializeSimulation();
    }

    /// <summary>
    /// Generate a random seed value.
    /// </summary>
    public void GenerateRandomSeed()
    {
        // Create a new random object using an arbitrary value as the object seed.
        System.Random _random = new System.Random((int)DateTime.Now.Ticks / (int)TimeSpan.TicksPerMillisecond);
        int _seed = _random.Next(int.MinValue, int.MaxValue);

        // Update the random seed input field
        uiController.UpdateRandomSeedText(_seed);
        //Debug.Log($"Generated random seed {_seed}");
    }

    /// <summary>
    /// Generate the map of cells to be simulated.
    /// </summary>
    /// <param name="_randomSeed">Random seed used to generate a map.</param>
    /// <param name="_livingProbability">Probability a cell will be alive or dead when generated.</param>
    /// <returns></returns>
    private Cell[,] GenerateCellMap(int? _randomSeed, float _livingProbability)
    {
        // If a random seed is not present,
        if (_randomSeed == null)
        {
            // Generate a map without it
            return mapGenerator.GenerateMap();
        }

        // Otherwise, use it to generate a map
        return mapGenerator.GenerateMapFromSeed(_randomSeed.Value, _livingProbability);
    }
    #endregion

    #region UIMethods

    /// <summary>
    /// Retrieve the random seed from the UI Controller.
    /// </summary>
    /// <returns>Integer value if a seed is present, otherwise, null.</returns>
    private int? GetRandomSeedFromUI()
    {
        return uiController.GetRandomSeedValue();
    }

    /// <summary>
    /// Retrieve the living probability value from the UI Controller.
    /// </summary>
    /// <returns>Float in the range [0.0, 1.0].</returns>
    private float GetLivingProbabilityFromUI()
    {
        return uiController.GetLivingProbabilitySliderValue();
    }

    #endregion
}
