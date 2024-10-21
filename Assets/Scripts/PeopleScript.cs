using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleScript : MonoBehaviour
{
    private enum peopleState
    {
        Walking,
        Sitting,
        PickedUp,
        Ragdoll
    }

    private Rigidbody[] _ragdollRigidBodies;
    private peopleState _currentState = peopleState.Walking;
    private Animator _animator;
    private CharacterController _characterController;
    
    void Awake()
    {
        _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        DisableRagdolls();
    }

    #region Disable/Enable Ragdoll
    private void DisableRagdolls()
    {
        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.isKinematic = true;
        }
        _animator.enabled = true;
        _characterController.enabled = true;
    }

    public void EnableRagdolls()
    {
        foreach (var rigidBody in _ragdollRigidBodies)
        {
            rigidBody.isKinematic = false;
        }
        _animator.enabled = false;
        _characterController.enabled = false;
    }

    void ApplyForce()
    {
        Vector3 direction = transform.position - Camera.main.transform.position;
    }
    #endregion

    public void PickedUp()
    {
        _currentState = peopleState.Ragdoll;
        EnableRagdolls();
    }

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
