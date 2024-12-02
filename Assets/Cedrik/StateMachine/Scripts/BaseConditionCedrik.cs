using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseConditionCedrik : ScriptableObject
{
    public virtual void Initialize(GameObject _thisGameObject)
    {

    }

    public abstract bool IsConditionMet();

    public virtual void DrawGizmos()
    {

    }

    public virtual bool IsImplemented()
    {
        return this.GetType().GetMethod("DrawGizmos").DeclaringType != typeof(BaseConditionCedrik);
    }
}
