using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Conditions/InRangeCondition")]
public class InRangeConditionCedrik : BaseConditionCedrik
{
    private GameObject detectedObject;
    private GameObject thisGameObject;

    [Tooltip("If true it will be checked if the Object is within the detection range otherwise it will be checked if it is outside")]
    [SerializeField]
    bool withinRange = true;
    [SerializeField]
    private float detectionRadius = 0.0f;
    [Tooltip("Objects with the Layer that can be detected")]
    [SerializeField]
    LayerMask layerToDetect;
    [Tooltip("Tag of the Object to be detected")]
    [SerializeField]
    string objectTagToDetect;

    [SerializeField]
    private Color gizmoColor = Color.red;

    public override void Initialize(GameObject _thisGameObject)
    {
        detectedObject = null;
        thisGameObject = _thisGameObject;
    }

    public override bool IsConditionMet()
    {
        detectedObject = null;
        foreach (Collider _coll in Physics.OverlapSphere(thisGameObject.transform.position, detectionRadius, layerToDetect))
        {
            if (_coll.CompareTag(objectTagToDetect))
            {
                detectedObject = _coll.gameObject;
                break;
            }
        }
        if (withinRange == true)
        {
            if (detectedObject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (withinRange == false)
        {
            if(detectedObject != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    public float GetDetectionRadius()
    {
        return detectionRadius;
    }

    public override void DrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(thisGameObject.transform.position, detectionRadius);
    }
}
