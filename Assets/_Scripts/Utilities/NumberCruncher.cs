using UnityEngine;
using System;

public static class NumberCruncher
{
    public static int randomMinInt { get; private set; } = -100000;
    public static int randomMaxInt { get; private set; } = 100000;

    public static float DistanceV2(Vector2 _a, Vector2 _b)
    {
        /* 
         * Forumla for distance in 2D is sqrt((xx1 - x0)^2 + (y1 - y0)^2)
         * Using the multiplication operator here is faster than the Mathf.Pow function as it doesn't need to deal with the general case
        */
        return Mathf.Sqrt((_b.x - _a.x) * (_b.x - _a.x) + (_b.y - _a.y) * (_b.y - _a.y));
    }

    public static float DistanceV3(Vector3 _a, Vector3 _b)
    {
        /* 
         * Forumla for distance in 3D is sqrt((xx1 - x0)^2 + (y1 - y0)^2 + (z1 - z0)^2)
         * Using the multiplication operator here is faster than the Mathf.Pow function as it doesn't need to deal with the general case
        */
        return Mathf.Sqrt((_b.x - _a.x) * (_b.x - _a.x) + (_b.y - _a.y) * (_b.y - _a.y) + (_b.z - _a.z) * (_b.z - _a.z));
    }

    public static float RoundFloat(float _value, int _precision = 0)
    {
        return (float)Math.Round(_value, _precision);
    }
}
