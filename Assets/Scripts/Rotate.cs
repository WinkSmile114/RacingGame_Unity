using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed = 5;
    private float tempSpeed = 00;
    public float pickupSpeed;
    public Vector3 rotationAxis;

    private void OnEnable()
    {
        tempSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (tempSpeed < rotateSpeed)
            tempSpeed += pickupSpeed;
        transform.Rotate(rotationAxis * tempSpeed * Time.deltaTime, Space.Self);
    }
}