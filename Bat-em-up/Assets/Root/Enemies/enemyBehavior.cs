using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBehavior : MonoBehaviour
{
    public float bulletSpeed = 4f;
    public float fireRate = 0.5f;
    private float nextFire;
    public GameObject ammo;
    public Transform muzzle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time>= nextFire)
        {
            Shoot();
            nextFire = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        GameObject tmpammo = Instantiate(ammo, muzzle.position, Quaternion.identity);
        tmpammo.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0) * bulletSpeed;
    }
}
