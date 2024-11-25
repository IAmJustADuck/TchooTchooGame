using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PassengerRagdoll : MonoBehaviour
{
    private List<Rigidbody> _ragdollRigidBodies;

    [Header("Tickets")]
    [SerializeField] private Sprite[] goodTickets;
    [SerializeField] private Sprite[] badTickets;
    public bool hasTicket;
    public Sprite currentTicket;

    [Header("Bone To Freeze (hips)")]
    public GameObject toParent;
    [SerializeField] private Rigidbody hipsBone;
    private Animator anim;
    private NavMeshAgent navAgent;
    private PassengerStates statesScript;

    void Awake()
    {
        _ragdollRigidBodies = new List<Rigidbody>(transform.GetChild(0).GetComponentsInChildren<Rigidbody>());
        _ragdollRigidBodies.Remove(hipsBone);

        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        statesScript = GetComponent<PassengerStates>();

        DisableRagdolls();

        hasTicket = Random.value > 0.5f;
        currentTicket = hasTicket ? 
            goodTickets[Random.Range(0, goodTickets.Length)] : 
            badTickets[Random.Range(0, badTickets.Length)];
    }

    public void DisableRagdolls()
    {
        hipsBone.isKinematic = true;
        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.isKinematic = true;
        }
        anim.enabled = true;
    }


    public void EnableRagdolls()
    {
        Destroy(statesScript);
        Destroy(anim);
        Destroy(navAgent);

        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.isKinematic = false;
        }
    }


    public void PickUp()
    {
        EnableRagdolls();

        hipsBone.isKinematic = true;

        toParent.transform.localPosition = new Vector3(0f, 0f, 0f);
        toParent.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        hipsBone.transform.localPosition = new Vector3(0f, 0f, 0f);
        hipsBone.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }


    public void Drop()
    {
        EnableRagdolls();

        hipsBone.isKinematic = false;
        toParent.transform.parent = null;
    }

    public void Throw(Vector3 direction, float force)
    {
        hipsBone.isKinematic = false;
        toParent.transform.parent = null;
        EnableRagdolls();
        hipsBone.AddForce(direction * force, ForceMode.VelocityChange);
        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.AddForce(direction * force, ForceMode.VelocityChange);
        }
    }
}
