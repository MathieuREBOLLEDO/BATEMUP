using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private GetBulletInstance instBullet;
    [SerializeField] private GetPlayerInstance instPlayer;


    public TextMeshProUGUI [] texts;
    

    // Update is called once per frame
    void Update()
    {
        texts[0].text = "Lerp Time : " + instBullet.bulletInstance.lerpTime.ToString();
        texts[1].text = "Velocity : " + ((int)instBullet.bulletInstance.currentVelocity.magnitude).ToString();
    }
}
