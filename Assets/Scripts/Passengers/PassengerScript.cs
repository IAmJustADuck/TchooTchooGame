using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PassengerScript : MonoBehaviour
{
    private enum peopleState
    {
        Walking,
        Sitting,
        PickedUp,
        Ragdoll
    }

    [SerializeField] private Rigidbody hipsBone;

    private List<Rigidbody> _ragdollRigidBodies;
    private peopleState _currentState = peopleState.Walking;
    private Animator _animator;
    //private CharacterController _characterController;

    void Awake()
    { 
        _ragdollRigidBodies = new List<Rigidbody>(transform.GetChild(0).GetComponentsInChildren<Rigidbody>());
        _ragdollRigidBodies.Remove(hipsBone);
        _animator = GetComponent<Animator>();
        //_characterController = GetComponent<CharacterController>();

        DisableRagdolls();
    }

    #region Disable/Enable Ragdoll
    private void DisableRagdolls()
    {
        hipsBone.isKinematic = true;
        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.isKinematic = true;
        }

        _animator.enabled = true;
        //_characterController.enabled = true;
    }

    public void EnableRagdolls()
    {
        _animator.enabled = false;
        //_characterController.enabled = false;

        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.isKinematic = false;
        }
    }

    void ApplyForce()
    {
        Vector3 direction = transform.position - Camera.main.transform.position;
    }
    #endregion

    #region PickUp / Drop / Throw 
    public void PickUp()
    {
        _currentState = peopleState.PickedUp;

        hipsBone.isKinematic = true;

        hipsBone.transform.localPosition = new Vector3(0f, 1f, 0f);
        hipsBone.transform.localRotation = Quaternion.Euler(14.911f, 0f, 0f);
        
        EnableRagdolls();
    }

    public void DropPassenger()
    {
        _currentState = peopleState.Ragdoll;
        
        //Unfreeze hips
        hipsBone.isKinematic = false;
    }

    public void ThrowPassenger(Vector3 direction, float force)
    {
        _currentState = peopleState.Ragdoll;
        //Unfreeze hips
        hipsBone.isKinematic = false;

        hipsBone.AddForce(direction * force, ForceMode.VelocityChange);
        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.AddForce(direction * force, ForceMode.VelocityChange);
        }
    }
    #endregion

    #region Behavior Tree
    private void Update()
    {
        switch(_currentState)
        {
            case peopleState.Walking:
                WalkingBehavior();
                break;

            case peopleState.Sitting:
                SittingBehavior();
                break;

            case peopleState.PickedUp:
                PickedUpBehavior();
                break;

            case peopleState.Ragdoll:
                RagdollBehavior();
                break;
        }
    }
    #endregion

    #region Behaviors
    private void WalkingBehavior() 
    {
    
    }

    private void SittingBehavior()
    {

    }

    private void PickedUpBehavior()
    {

    }

    private void RagdollBehavior()
    {

    }
    #endregion
}
