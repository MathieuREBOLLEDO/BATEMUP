using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShaker : MonoBehaviour
{

    private Transform camera;
    [SerializeField] private Vector3 positionStrengh;
    [SerializeField] private Vector3 rotationStrengh;

    private void Awake()
    {
        camera = GetComponent<Camera>().transform;
    }

    private static event Action Shake;

    public static void Invoke()
    {
        Shake?.Invoke();
    }

    private void OnEnable() => Shake += CameraShake;
    private void OnDisable() => Shake += CameraShake;

    private void CameraShake()
    {
        camera.DOComplete();
        camera.DOShakePosition(0.3f, positionStrengh);
        camera.DOShakeRotation(0.3f, rotationStrengh);
    }
 
    
}
