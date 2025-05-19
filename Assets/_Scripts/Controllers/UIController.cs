using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    #region Variables

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

    #endregion

    #region InitializationMethods

    /// <summary>
    /// Initialize the text displayed in the cell count and simulation step labels.
    /// </summary>
    /// <param name="_totalCellCount">Integer representing number of total cells.</param>
    /// <param name="_deadCellCount">Integer representing number of dead cells.</param>
    /// <param name="_livingCellCount">Integer representing number of living cells.</param>
    /// <param name="_simulationSteps">Integer representing number of steps simulated.</param>
    public void InitializeSimulationStatistics(int _totalCellCount, int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        // Set the initial cell counts
        cellCountTotalLabel.text = _totalCellCount.ToString();
        cellCountDeadLabel.text = _deadCellCount.ToString();
        cellCountLivingLabel.text = _livingCellCount.ToString();
    }

    #endregion

    #region UIUpdateMethds

    /// <summary>
    /// Update the text displayed in the cell count labels.
    /// </summary>
    /// <param name="_deadCellCount">Integer representing number of dead cells.</param>
    /// <param name="_livingCellCount">Integer representing number of living cells.</param>
    /// <param name="_simulationSteps">Integer representing number of steps simulated.</param>
    public void UpdateSimulationStatistics(int _deadCellCount, int _livingCellCount, int _simulationSteps)
    {
        // Update the dead and living cell counts as well as the simulation step count
        cellCountDeadLabel.text = _deadCellCount.ToString();
        cellCountLivingLabel.text = _livingCellCount.ToString();
        simulationStepCountLabel.text = _simulationSteps.ToString();
    }

    /// <summary>
    /// Update the text displayed in the simulation state button label.
    /// </summary>
    /// <param name="_simulationState">A boolean seed used to set text.</param>
    public void UpdateToggleSimulationButtonText(bool _simulationState)
    {
        // Set the simulation buttons text based on the simulation state using the lookup dictionary
        toggleSimulationButtonText.text = toggleSimulationButtonTextStateLookup[_simulationState];
    }

    /// <summary>
    /// Update the text displayed in the random seed label.
    /// </summary>
    /// <param name="_seed">An integer seed.</param>
    public void UpdateRandomSeedText(int _seed)
    {
        // Convert the seed value to a string and set the random input fields text to it
        randomSeedInputField.text = _seed.ToString();
    }

    #endregion

    #region UIDataGettingMethods

    /// <summary>
    /// Get the value from the living probability slider.
    /// </summary>
    /// <returns>A float in the range [0.0, 1.0].</returns>
    public float GetLivingProbabilitySliderValue()
    {
        // Return the living probability sliders value
        return livingProbabilitySlider.value;
    }

    /// <summary>
    /// Get the value from the random seed input field.
    /// </summary>
    /// <returns>An integer if a value is present, otherwise, null.</returns>
    public int? GetRandomSeedValue()
    {
        if (!int.TryParse(randomSeedInputField.text, out int _seed))    // If the random seed input fields value cannot be parsed to an int,
        {
            return null;                                                // Return null
        }

        // Return the seed value
        return _seed;
    }

    #endregion
}
