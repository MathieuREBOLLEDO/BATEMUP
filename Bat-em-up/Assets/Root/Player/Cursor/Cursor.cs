using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class Cursor : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 playerPos;
    private Vector2 screenBounds;

    void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform.position;
        //line = GetComponent<LineRenderer>();
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);

    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3 ( mousePos.x,mousePos.y,0);

        playerPos = GameObject.FindWithTag("Player").transform.position;

       // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Vector3 direction = (mousePos - playerPos).normalized;

        //Debug.DrawRay(playerPos, direction*500f,Color.white);

        
        //line.SetPosition(0, playerPos);
        //line.SetPosition(1, direction * 500f);
        //line.SetPosition(line.positionCount-1,)


    }
}
