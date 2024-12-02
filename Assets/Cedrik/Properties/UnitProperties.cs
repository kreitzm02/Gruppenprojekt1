using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProperties : MonoBehaviour
{
    [SerializeField]
    AttackProperties attackProperties;

    [SerializeField]
    private float maxHealthPoints;

    private float healthPoints;

    public float damageMultiplier = 1;

    public float defense { private get; set; }


    void Start()
    {
        //change following line if unit should not get full hp after reloading/loading scene
        healthPoints = maxHealthPoints;
    }

    public void RecieveDamage(float _damage)
    {
        healthPoints = healthPoints - (_damage - defense);
        if (healthPoints < 0)
        {
            healthPoints = 0;
        }
    }
}
