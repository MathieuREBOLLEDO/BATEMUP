using TMPro;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    private Player playerInstance;


    void Start()
    {
        playerInstance = Player.instance;
        UpdateCounter();

    }

    public void UpdateCounter()
    {
        text.SetText("x " + playerInstance.lifePoints);
    }

}
