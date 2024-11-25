using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class PassengerStates : MonoBehaviour
{
    public enum CharacterState
    {
        Walking,
        Sitting,
        Idle
    }

    private bool animDone = false;

    public CharacterState currentState;
    public CharacterState destinationState;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private PassengerRagdoll interactScript;

    public int stopsBeforeLeaving;

     public Transform sittingPoint;
    [HideInInspector] public Transform stationParent;
    public BoxCollider exitZone;

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        interactScript = GetComponent<PassengerRagdoll>();
        currentState = CharacterState.Idle;

        stopsBeforeLeaving = Random.Range(0, 3);
    }


    void Update()
    {
        Debug.Log(currentState);

        switch (currentState)
        {
            case CharacterState.Walking:
                WalkingBehavior();
                break;
            case CharacterState.Sitting:
                break;
            case CharacterState.Idle:
                IdleBehavior();
                break;
        }
    }

    private void WalkingBehavior()
    {
        float speed = _navMeshAgent.velocity.magnitude;
        _animator.SetFloat("Speed", speed);

        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (destinationState == CharacterState.Sitting) 
            {
                SetState(destinationState);
                destinationState = CharacterState.Walking;
            }
            else if(destinationState == CharacterState.Idle)
            {
                transform.parent = stationParent;
                Debug.Log("Test??");
            }
            
        }
    }

    private IEnumerator WaitForSitTransition()
    {
        yield return new WaitUntil(() => animDone);
        animDone = false;
        SetState(CharacterState.Sitting);
    }

    private void IdleBehavior()
    {
        _animator.SetFloat("Speed", 0);
    }

    private void SetState(CharacterState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case CharacterState.Walking:
                break;
            case CharacterState.Sitting:
                TurnToSitDirection();
                break;
            case CharacterState.Idle:
                break;
        }
    }

    public void MoveToSittingPoint()
    {
        currentState = CharacterState.Walking;
        destinationState = CharacterState.Sitting;

        _navMeshAgent.SetDestination(sittingPoint.position);
    }

    public void ReachedStop()
    {
        if (stopsBeforeLeaving <= 0)
        {
            StandUpAndExit();
        }
        stopsBeforeLeaving--;
        Debug.Log(stopsBeforeLeaving);
    }    

    public void StandUpAndExit()
    {
        _animator.SetTrigger("StandUp");
        StartCoroutine(WaitForAnimationThenMoveToExit());
    }

    private IEnumerator WaitForAnimationThenMoveToExit()
    {
        yield return new WaitUntil(() => animDone);
        animDone = false;

        currentState = CharacterState.Walking;
        destinationState = CharacterState.Idle;

        MoveToExitZone();
    }

    public void MoveToExitZone()
    {
        currentState = CharacterState.Walking;
        destinationState = CharacterState.Idle;

        float x = Random.Range(exitZone.bounds.min.x, exitZone.bounds.max.x);
        float y = 0f;
        float z = Random.Range(exitZone.bounds.min.z, exitZone.bounds.max.z);
        Vector3 exitPositon = new Vector3(x, y, z);

        _navMeshAgent.SetDestination(exitPositon);
    }

    public void TurnToSitDirection()
    {
        StartCoroutine(LerpRotationToSitDirection());
    }

    private IEnumerator LerpRotationToSitDirection()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = sittingPoint.rotation;

        float timeElapsed = 0f;
        float lerpDuration = .3f;

        while (timeElapsed < lerpDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        _animator.SetTrigger("Sit");
        SetState(CharacterState.Sitting);
    }

    public void OnAnimationCompleted()
    {
        animDone = true;
    }
}
