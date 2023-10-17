using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{  
    [Header("Player Information")]

    [SerializeField] private GetPlayerInstance instPlayer;
    private Player player;
    [SerializeField] private TextMeshProUGUI[] playerTexts;
    

    [Header("Bullet Information")]

    [SerializeField] private GetBulletInstance instBullet;
    private PlayerBullet bullet;
    [SerializeField] private TextMeshProUGUI[] bulletTexts;

    private void Start()
    {
        player = instPlayer.playerInstance;
        bullet = instBullet.bulletInstance;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i < playerTexts.Length; i++)
        {
            string tmpStr ="";
            switch (i)
            {
                case 0:
                    tmpStr = "Velocity : " + player.GetComponent<Rigidbody2D>().velocity.magnitude.ToString();
                    break;
                case 1:
                    tmpStr = "Fire Rate : " + player.attackRate;
                    break;
                    
            }
            playerTexts[i].text = tmpStr;
        }

        for(int j = 0; j < bulletTexts.Length; j++)
        {
            string tmpStr = "";
            switch (j)
            {
                case 0:
                    tmpStr = "Velocity : " + ((int)bullet.currentVelocity.magnitude).ToString();
                    break;
                case 1:
                    tmpStr = "Size : " + bullet.transform.localScale.x;
                    break;
                case 2:
                    tmpStr = "Lerp Time : " + Mathf.Round( bullet.lerpTime * 100f) / 100f;
                    break;

                    
            }
            bulletTexts[j].text = tmpStr;
        }
    }

}
