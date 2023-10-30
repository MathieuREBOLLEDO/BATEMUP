using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    start,
    second,
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] InstanceLevelManager instLevel;
 
    public States states;

    public ScrollingEffect scrolType ;

    public void SetScrolType(ScrollingEffect scrolling)
    {
        scrolType = scrolling;
    }


    private void Awake()
    {
        instLevel.levelInstance = this; 
    }

    void Start()
    {
        switch(states)
        {
            case States.start:
                break;
        }
    }

    void UpdateScrol()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
