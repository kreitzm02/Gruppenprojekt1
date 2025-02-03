using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platzhalter-Skript um Funktionalität zu testen
public class PlayerBehaviour_M : MonoBehaviour, IDamageable, IKillable, IAttackable, IInputHandler
{
    public int healthPoints = 100;
    public int unarmedAtkDamage = 8; // fists
    public IWeapon currentWeapon;
    private float mouseDownTime;
    private float clickThreshold = 0.15f;
    private HealthBar healthBar;
    public int maxHealthPoints = 100;

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public void Update()
    {
        currentWeapon = GetComponentInChildren<IWeapon>();
        if (currentWeapon == null)
        {
            Debug.Log("Current Weapon not found");
        }

        healthBar.UpdateHealthBar(healthPoints, maxHealthPoints);
    }

    public string GetAttackAnimation()
    {
        if (currentWeapon != null)
        {
            return currentWeapon.GetWeaponAnimName();
        }
        else return "Unarmed_Melee_Attack_Punch_A";
    }
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
        if (currentWeapon != null)
            return currentWeapon.GetWeaponDamage();
        else return unarmedAtkDamage;
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
