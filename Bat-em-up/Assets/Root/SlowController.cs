using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoController : MonoBehaviour
{
    public float slowMoFactor = 0.5f; // Adjust this value for desired slow-motion effect

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Change the input key as needed
        {
            ToggleSlowMotion();
        }
    }

    void ToggleSlowMotion()
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = slowMoFactor;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
