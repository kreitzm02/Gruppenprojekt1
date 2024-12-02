using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    [SerializeField]
    GameObject arrow;
    [SerializeField]
    float arrowSpeed = 10.0f;
    [SerializeField]
    float despawnDelay = 5;

    
    public void TestFunction()
    {
        Debug.Log("aertjhaerhjerhjeth");
        Vector3 direction = new Vector3(this.gameObject.transform.forward.x,0, this.gameObject.transform.forward.z);
        Quaternion rotation = new Quaternion(0, this.gameObject.transform.rotation.y,0, this.gameObject.transform.rotation.w);
        GameObject projectile = Instantiate(arrow,this.gameObject.transform.position + new Vector3(0,1,0),rotation);
        projectile.transform.Rotate(0, 180, 0);
        projectile.GetComponent<Rigidbody>().AddForce(direction * arrowSpeed,ForceMode.Impulse);
        Destroy(projectile, despawnDelay);
    }
}
