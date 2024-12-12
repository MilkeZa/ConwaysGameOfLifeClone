using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;                 // Generates the cell maps
    [SerializeField] private SimulationController simulationController; // Controls the simulations performed on the cell map
    [SerializeField] private UIController uiController;                 // Controls the user interface
    [SerializeField] private CameraController cameraController;         // Controls the camera zoom/movement

    private void Start()
    {
        InitializeSimulation();
    }

    public void InitializeSimulation()
    {
        // Subscribe to the simulation controllers delegates
        simulationController.OnSimulationStateChanged += InitializeSimulationCellCountText;
        simulationController.OnSimulationStateChanged += UpdateSimulationStatistics;

        Cell[,] _cells = GenerateCellMap(GetRandomSeedFromUI(), GetLivingProbabilityFromUI());
        simulationController.InitializeSimulationState(_cells);
    }

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

    private void UpdateSimulationStatistics(int? _, int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        uiController.UpdateSimulationStatistics(_deadCellCount, _livingCellCount, _simulationSteps);
    }

    public void ToggleSimulationState()
    {
        // Get the current state of the simulation and reverse it
        bool _simulationState = !simulationController.isSimulating;

        // Set the state
        SetSimulationState(_simulationState);
    }

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

    public void ResetSimulation()
    {
        // Pause the simulation state
        SetSimulationState(false);

        // Subscribe the set initialization text function
        simulationController.OnSimulationStateChanged += InitializeSimulationCellCountText;

        // Initialize the simulation state
        InitializeSimulation();
    }

    private int? GetRandomSeedFromUI()
    {
        return uiController.GetRandomSeedValue();
    }

    private float GetLivingProbabilityFromUI()
    {
        return uiController.GetLivingProbabilitySliderValue();
    }

    public void GenerateRandomSeed()
    {
        // Create a new random object using an arbitrary value as the object seed.
        System.Random _random = new System.Random((int)DateTime.Now.Ticks / (int)TimeSpan.TicksPerMillisecond);
        int _seed = _random.Next(int.MinValue, int.MaxValue);

        // Update the random seed input field
        uiController.UpdateRandomSeedText(_seed);
        //Debug.Log($"Generated random seed {_seed}");
    }

    private Cell[,] GenerateCellMap(int? _randomSeed, float _livingProbability)
    {
        if (_randomSeed == null)
        {
            return mapGenerator.GenerateMap();
        }

        return mapGenerator.GenerateMapFromSeed(_randomSeed.Value, _livingProbability);
    }
}
