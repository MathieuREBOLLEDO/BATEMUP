using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Scrolling_Effect : MonoBehaviour
{
    public float speedX = 0.2f;
    public float speedY = 0.2f;
    // Update is called once per frame
    void Update()
    {
        float offsetX = Time.time * speedX;
        float offsetY = Time.time * speedY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
