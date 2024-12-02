using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Properties/Attacks")]
public class AttackProperties : ScriptableObject
{
    [SerializeField]
    public List<BaseAttack> attacks;
}
