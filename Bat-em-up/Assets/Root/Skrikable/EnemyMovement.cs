using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyMovement", menuName = "Enemy/EnemyMovement")]
public class EnemyMovement : ScriptableObject
{

    public enum Behavior
    {
        statique,
        straight,
        cos,
        waypoint,
        spline_waypoint,
    }

    public Behavior movement;

    [Header ("Basic")]
    public Vector2 direction = new Vector2(0,-1);
    public float speed = 5f;

    [Header("Sin")]
    public float magnitude = 1f;
    public Vector2 axis = new Vector2(1, 0);
    private Transform targetTransform;

    
    public void Init(Transform transform)
    {
        targetTransform = transform;
    }

    // Update is called once per frame

    public void Movement()
    {
        switch (movement)
        {
            case Behavior.statique:
                ProcessStatique();
                break;

            case Behavior.straight:
                ProcessStraight();
                break;

            case Behavior.cos:
                ProcessCos();
                break;

        }
        
    }

    private void ProcessStatique()
    {

    }

    private void ProcessStraight()
    {
        targetTransform.position += (Vector3)direction * speed * Time.deltaTime; 
    }

    private void ProcessCos()
    {
        ProcessStraight();
        float newMovement = Mathf.Cos(Time.time * speed) * magnitude;
        Vector3 offset = axis * newMovement;

        targetTransform.position += offset;
    }
}
