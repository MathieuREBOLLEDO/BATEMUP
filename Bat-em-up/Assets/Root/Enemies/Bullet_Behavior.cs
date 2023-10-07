using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behavior : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Rigidbody2D>().velocity = - transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
