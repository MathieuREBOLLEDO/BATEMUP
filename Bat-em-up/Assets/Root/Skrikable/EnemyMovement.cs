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
        sin,
        waypoint,
        spline_waypoint,
    }

    public Behavior movement;
    public Vector2 direction;
    public float speed;

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

        }
        
    }

    private void ProcessStatique()
    {

    }

    private void ProcessStraight()
    {
        targetTransform.position += (Vector3)direction * speed * Time.deltaTime; 
    }

    private void ProcessSin()
    {

    }
}
