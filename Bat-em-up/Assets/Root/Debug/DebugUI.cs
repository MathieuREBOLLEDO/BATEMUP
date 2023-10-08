using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DebugUI : MonoBehaviour
{
    public Player_Bullet p_bullet;
    public Player player;

    public TextMeshProUGUI [] texts;
    

    // Update is called once per frame
    void Update()
    {
        texts[0].text = "Lerp Time : " + p_bullet.lerpTime.ToString();
        texts[1].text = "Velocity : " + ((int)p_bullet.currentVelocity.magnitude).ToString();
    }
}
