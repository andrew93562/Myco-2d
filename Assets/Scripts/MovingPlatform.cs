using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] int ListensToSwitchID;
    [SerializeField] GameObject[] points;
    [SerializeField] float speed;
    private int pointIndex = 0;
    private bool switchActivated = false;

    public void OnActivate(Component sender, object data)
    {
        int switchID = (int)data;
        if (ListensToSwitchID == switchID)
        {
            switchActivated = !switchActivated;
        }
    }

    private void Update()
    {
        if (switchActivated)
        {
            foreach (GameObject point in points)
            {
                transform.position = Vector2.MoveTowards(transform.position, points[pointIndex].transform.position, speed * Time.deltaTime);
                if (transform.position == points[pointIndex].transform.position) 
                {
                    pointIndex = (pointIndex + 1) % points.Length;
                }
            }
        }
    }
}
