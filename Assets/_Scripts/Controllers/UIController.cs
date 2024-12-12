using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text cellCountTotalLabel;      // Text displaying total cell count
    [SerializeField] private TMP_Text cellCountDeadLabel;       // Text displaying dead cell count
    [SerializeField] private TMP_Text cellCountLivingLabel;     // Text displaying living cell count
    [SerializeField] private TMP_Text simulationStepCountLabel; // Text displaying simulation step count

    [SerializeField] private UnityEngine.UI.Slider livingProbabilitySlider; // Used to alter the chance a cell will be living or dead upon creation
    [SerializeField] private TMP_InputField randomSeedInputField;           // Used to accept integers representing random seeds

    [SerializeField] private TMP_Text toggleSimulationButtonText;           // Text displayed within the toggle simulation button
    private Dictionary<bool, string> toggleSimulationButtonTextStateLookup = new Dictionary<bool, string>()
    {
        { true, "Pause Simulation" },   // Simulation is currently playing, change to paused
        { false, "Play Simulation" }    // Simulation is currently paused, change to playing
    };

    public void InitializeSimulationStatistics(int _totalCellCount, int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        // Set the initial cell counts
        cellCountTotalLabel.text = _totalCellCount.ToString();
        cellCountDeadLabel.text = _deadCellCount.ToString();
        cellCountLivingLabel.text = _livingCellCount.ToString();
    }

    public void UpdateSimulationStatistics(int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        // Update the dead and living cell counts as well as the simulation step count
        cellCountDeadLabel.text = _deadCellCount.ToString();
        cellCountLivingLabel.text = _livingCellCount.ToString();
        simulationStepCountLabel.text = _simulationSteps.ToString();
    }

    public void UpdateToggleSimulationButtonText(bool _simulationState)
    {
        // Set the simulation buttons text based on the simulation state using the lookup dictionary
        toggleSimulationButtonText.text = toggleSimulationButtonTextStateLookup[_simulationState];
    }

    public void UpdateRandomSeedText(int _seed)
    {
        // Convert the seed value to a string and set the random input fields text to it
        randomSeedInputField.text = _seed.ToString();
    }

    public float GetLivingProbabilitySliderValue()
    {
        // Return the living probability sliders value
        return livingProbabilitySlider.value;
    }

    public int? GetRandomSeedValue()
    {
        if (!int.TryParse(randomSeedInputField.text, out int _seed))    // If the random seed input fields value cannot be parsed to an int,
        {
            return null;                                                // Return null
        }

        // Return the seed value
        return _seed;
    }
}
