using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField]
    MyPlayerHealth health;
    float inithealth;
    // Start is called before the first frame update
    void Start()
    {
        inithealth = health.health;
    }

    // Update is called once per frame
    void Update()
    {
        if(inithealth>health.health)
        {
            Debug.Log("Health is lowering");
        }
    }
}
