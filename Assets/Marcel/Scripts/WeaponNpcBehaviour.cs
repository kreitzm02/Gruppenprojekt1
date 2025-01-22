using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponNpcBehaviour : MonoBehaviour
{
    private Animator animator;
    private bool interactionPossible;
    private AnimatorStateInfo animStateInfo;
    private string weaponSlot;
    private GameObject newWeapon;
    private bool hasInteracted;
    private Transform playerTransform;
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        hasInteracted = false;
        animator = GetComponent<Animator>();
        animator.CrossFade("Idle", 0.1f);
        weaponSlot = "Rig/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/handslot.r/";
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, 2f);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                interactionPossible = true;
                playerTransform = col.gameObject.transform;
                break;
            }
            else interactionPossible = false;
        }
        if (interactionPossible)
        {
            ChangeWeaponTest();
        }
        if (hasInteracted)
        {
            hasInteracted = false;
            animator.CrossFade("Interact", 0f);
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float normalizedAnimTime = animStateInfo.normalizedTime % 1.0f;
            try
            {
                Destroy(playerTransform.Find("Rig/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/handslot.r/").GetChild(0).gameObject);
            }
            catch { }
            Instantiate(newWeapon, playerTransform.Find("Rig/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/handslot.r/"));
            if (normalizedAnimTime > 0.9f)
            {
                animator.CrossFade("Idle", 0.1f);
            }
        }
    }

    private void ChangeWeaponTest()
    {
        if (InventoryManager.Instance.obtainedAxe && Input.GetKeyDown(KeyCode.Alpha1) && hasInteracted == false) // axe
        {
            foreach (GameObject weapon in weapons)
            {
                if (weapon.name == "axe_1handed")
                {
                    newWeapon = weapon;
                    hasInteracted = true;
                }
            }
        }
        if (InventoryManager.Instance.obtainedDagger && Input.GetKeyDown(KeyCode.Alpha2) && hasInteracted == false) // dagger
        {
            foreach (GameObject weapon in weapons)
            {
                if (weapon.name == "dagger")
                {
                    newWeapon = weapon;
                    hasInteracted = true;
                }
            }
        }
        if (InventoryManager.Instance.obtainedSword && Input.GetKeyDown(KeyCode.Alpha3) && hasInteracted == false) // sword
        {
            foreach (GameObject weapon in weapons)
            {
                if (weapon.name == "sword_1handed")
                {
                    newWeapon = weapon;
                    hasInteracted = true;
                }
            }
        }
        if (InventoryManager.Instance.obtainedBlade && Input.GetKeyDown(KeyCode.Alpha4) && hasInteracted == false) // blade
        {
            foreach (GameObject weapon in weapons)
            {
                if (weapon.name == "Skeleton_Blade")
                {
                    newWeapon = weapon;
                    hasInteracted = true;
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, 2f); 
    }
}
