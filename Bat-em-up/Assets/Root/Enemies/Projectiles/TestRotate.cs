using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPivot
    : MonoBehaviour
{
    // Start is called before the first frame update
    private Quaternion initRot;
    public float rotateSpeed;
    private float direction = 1;
    void Start()
    {
        initRot = transform.localRotation;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.localRotation.eulerAngles.z >= initRot.eulerAngles.z + 90)
            direction = -1;
        else if (transform.localRotation.eulerAngles.z <= initRot.eulerAngles.z - 90)
            direction = 1;

        transform.RotateAround(transform.position, Vector3.forward,rotateSpeed * direction*Time.deltaTime);
        
    }
}
