using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLineColliderPoints : MonoBehaviour
{
    public LineRenderer lr;
    public EdgeCollider2D edgeColli;

    private void Awake()
    {
        lr.sortingLayerName = "Foreground";
    }

    private void Start()
    {
        List<Vector2> allPoints = new List<Vector2>();
        int numOfPoints = lr.positionCount;
        for (int i = 0; i < numOfPoints; i++)
        {
            Vector3 vertice = lr.GetPosition(i);
            allPoints.Add(vertice);
        }
        edgeColli.SetPoints(allPoints);
    }

}
