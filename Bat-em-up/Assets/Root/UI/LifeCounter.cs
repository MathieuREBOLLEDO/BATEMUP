using TMPro;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GetPlayerInstance pInstance;


    void Start()
    {        
        UpdateCounter();
    }

    public void UpdateCounter()
    {
        text.SetText("x " + pInstance.playerInstance.lifePoints);
    }

}
