using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class PointsView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float animationTime;


    public void UpdatePoints(float previousPoints, float points)
    {
        DOVirtual.Float(previousPoints, points, animationTime, value =>
        {
            text.text = value.ToString("F");
        });
    }

    private void OnEnable()
    {
        ServiceLocator.Register<PointsView>(this);
    }
}
