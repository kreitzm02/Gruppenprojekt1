using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    public abstract float attackDamage { get; set; }

    public virtual void AttackHit(GameObject _objectHit, GameObject _originObject)//_objectHit is the unit hit //this method has to be called when the enemy got hit //_originObject can be either this.GameObject or the right one to get the right multiplier from
    {
        float damageMultiplier = _originObject.GetComponent<UnitProperties>().damageMultiplier;
        _objectHit.GetComponent<UnitProperties>().RecieveDamage(attackDamage);
    }
}
