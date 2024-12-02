using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetDetectable
{
    public Transform DetectTargetVisibleRange();
    public Transform DetectTargetAttackRange();
}
