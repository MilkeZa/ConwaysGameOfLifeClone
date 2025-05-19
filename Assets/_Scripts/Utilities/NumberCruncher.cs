using UnityEngine;
using System;

public static class NumberCruncher
{
    #region Variables

    public static int randomMinInt { get; private set; } = -100000; // Minimum possible random integer generated
    public static int randomMaxInt { get; private set; } = 100000;  // Maximum possible random integer generated

    #endregion

    #region UtilityMethods

    /// <summary>
    /// Calculate the distance between two points in 2D space.
    /// </summary>
    /// <param name="_a">First vector.</param>
    /// <param name="_b">Second vector.</param>
    /// <returns>Distance between the two vectors.</returns>
    public static float DistanceV2(Vector2 _a, Vector2 _b)
    {
        /* 
         * Forumla for distance in 2D is sqrt((xx1 - x0)^2 + (y1 - y0)^2)
         * Using the multiplication operator here is faster than the Mathf.Pow function as it doesn't need to deal with the general case
        */
        return Mathf.Sqrt((_b.x - _a.x) * (_b.x - _a.x) + (_b.y - _a.y) * (_b.y - _a.y));
    }

    /// <summary>
    /// Calculate the distance between two points in 3D space.
    /// </summary>
    /// <param name="_a">First vector.</param>
    /// <param name="_b">Second vector.</param>
    /// <returns>Distance between the two vectors.</returns>
    public static float DistanceV3(Vector3 _a, Vector3 _b)
    {
        /* 
         * Forumla for distance in 3D is sqrt((xx1 - x0)^2 + (y1 - y0)^2 + (z1 - z0)^2)
         * Using the multiplication operator here is faster than the Mathf.Pow function as it doesn't need to deal with the general case
        */
        return Mathf.Sqrt((_b.x - _a.x) * (_b.x - _a.x) + (_b.y - _a.y) * (_b.y - _a.y) + (_b.z - _a.z) * (_b.z - _a.z));
    }

    /// <summary>
    /// Round a float to a given precision.
    /// </summary>
    /// <param name="_value">Value to be rounded.</param>
    /// <param name="_precision">Number of decimal places to round to.</param>
    /// <returns>Rounded float value.</returns>
    public static float RoundFloat(float _value, int _precision = 0)
    {
        return (float)Math.Round(_value, _precision);
    }

    #endregion
}
