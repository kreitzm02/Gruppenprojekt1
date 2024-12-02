using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IDamageable, IKillable, IAttackable, IInputHandler
{
    public int healthPoints = 100;
    public int attackDamage = 30;

    private float mouseDownTime;
    private float clickThreshold = 0.15f;

    public bool CheckDeathCondition()
    {
        if (healthPoints == 0)
            return true;
        else return false;
    }

    public void GainDamage(int _damage)
    {
        this.healthPoints -= _damage;
        if (this.healthPoints < 0)
            healthPoints = 0;
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public bool IsClick()
    {
        return Input.GetMouseButtonUp(0) && (Time.time - mouseDownTime) < clickThreshold;
    }

    public bool IsHold()
    {
        return Input.GetMouseButton(0) && (Time.time - mouseDownTime) >= clickThreshold;
    }

    public void ButtonDownCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTime = Time.time;
        }
    }
}
