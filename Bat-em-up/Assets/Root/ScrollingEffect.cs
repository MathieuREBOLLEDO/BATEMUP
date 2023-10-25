using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="NewScrolEffect", menuName ="Scrol/Scrol Effect")]
public class ScrollingEffect : ScriptableObject
{
    public float scrolSpeed = 5f;
    public Vector2 scrolDirection = new Vector2(0, -1);

    public void ScrolEffect(Transform transform)
    {
        transform.position += (Vector3)scrolDirection * scrolSpeed * Time.deltaTime;
    }
}
