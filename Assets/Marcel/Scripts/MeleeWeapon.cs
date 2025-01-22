using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField, Range(0, 100)] private int critProbability = 20;
    [SerializeField] public bool isTwoHanded = false;
    [SerializeField] public AnimationOptions attackAnim;
    public int GetWeaponDamage()
    {
        if (Random.Range(0, 101) <= critProbability)
        {
            return attackDamage * (int)1.5f;
        }
        else return attackDamage;
    }

    public string GetWeaponAnimName()
    {
        return attackAnim.ToString().Substring(1);
    }
}
public enum AnimationOptions
{
    _1H_Melee_Attack_Chop,
    _1H_Melee_Attack_Slice_Diagonal,
    _1H_Melee_Attack_Slice_Horizontal,
    _1H_Melee_Attack_Stab,
    _2H_Melee_Attack_Chop,
    _2H_Melee_Attack_Slice,
    _2H_Melee_Attack_Stab,
}


