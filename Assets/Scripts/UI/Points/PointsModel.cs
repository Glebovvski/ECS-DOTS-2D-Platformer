using UnityEngine;

public class PointsModel
{
    public float Points { get; private set; }
    public float PreviousPoints { get; private set; }

    public void SetPoints(float points)
    {
        Points = points;
    }

    public void SetPreviousPoints(float previousPoints)
    {
        PreviousPoints = previousPoints;
    }
}
