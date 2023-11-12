using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHiteable 
{
    void Hitting(Vector2 directio, float damage);
}
