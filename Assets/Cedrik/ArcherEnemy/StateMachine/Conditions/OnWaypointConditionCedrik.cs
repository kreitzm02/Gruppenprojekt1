using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Conditions/OnWaypointCondition")]
public class OnWaypointConditionCedrik : BaseConditionCedrik
{
    public Vector3 ?waypointLocation {  private get; set; }

    private GameObject thisGameObject;
    public override void Initialize(GameObject _thisGameObject)
    {
        if(thisGameObject == null)
        {
            thisGameObject = _thisGameObject;
        }
        waypointLocation = null;
    }

    [SerializeField]
    private Color gizmoColor = Color.red;
    [SerializeField]
    private float gizmoSize = 0.5f;

    public override bool IsConditionMet()
    {
        if (waypointLocation != null)
        {
            return Vector3.Distance(waypointLocation.Value, thisGameObject.transform.position) <= 0.1f;
        }
        return false;
    }

    public override void DrawGizmos()
    {
        if (waypointLocation != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(waypointLocation.Value, gizmoSize);
        }
    }
}
