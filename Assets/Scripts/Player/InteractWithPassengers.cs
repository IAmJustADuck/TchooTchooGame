using UnityEngine;
using UnityEngine.UI;

public class InteractWithPassengers : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator uiAnimator;
    [SerializeField] private Image inspectedTicket;
    [SerializeField] private TrainManager trainManagerScript;
    private PlayerMovements playerMovementScript;

    [Header("Passengers")]
    private AudioSource audioS;
    [SerializeField] private AudioClip[] wouahSounds;
    [SerializeField] private AudioClip[] hmmSounds;
    [SerializeField] private LayerMask peopleRaycastMask;
    [SerializeField] private GameObject playerShoulder;
    private bool carryingPassenger = false;
    private bool lookingAtTicket = false;
    private GameObject carriedPassenger;
    private PassengerRagdoll passengerRagdollScript;
    private PassengerStates passengerStateScript;

    [Header("Throw force")]
    [SerializeField] private float throwForce = 3.5f;
    [SerializeField] private float upwardForce = 1.25f;

    private void Start()
    {
        audioS = GetComponent<AudioSource>();
        playerMovementScript = GetComponent<PlayerMovements>();
    }

    void Update()
    {
        #region Pause / Stop Ticket Inspection
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (!lookingAtTicket)
            {
                //PAUSE
            }
            else
            {
                lookingAtTicket = false;
                playerMovementScript.enabled = true;
                uiAnimator.SetTrigger("StopInspect");
            }

        }
        #endregion

        #region AskTicket
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!lookingAtTicket)
            {
                if (!carryingPassenger)
                {
                    Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 1.5f, peopleRaycastMask))
                    {
                        lookingAtTicket = true;
                        playerMovementScript.enabled = false;

                        audioS.PlayOneShot(hmmSounds[Random.Range(0, hmmSounds.Length)]);

                        #region Find the parent with passengerRagdollScript
                        Transform currentTransform = hit.transform;

                        while (currentTransform != null)
                        {
                            PassengerRagdoll passengerRagdollScript = currentTransform.GetComponent<PassengerRagdoll>();
                            if (passengerRagdollScript != null)
                            {

                                carriedPassenger = currentTransform.transform.parent.gameObject;
                                break;
                            }
                            currentTransform = currentTransform.parent;
                        }
                        #endregion

                        passengerRagdollScript = carriedPassenger.transform.GetChild(0).GetComponent<PassengerRagdoll>();

                        inspectedTicket.sprite = passengerRagdollScript.currentTicket;
                        uiAnimator.SetTrigger("Inspect");
                    }
                }
            }
            else
            {
                lookingAtTicket = false;
                playerMovementScript.enabled = true;
                uiAnimator.SetTrigger("StopInspect");
            }
        }
        #endregion

        #region Pick up / Throw passenger / Drop passenger
        if (Input.GetMouseButtonDown(0))
        {
            #region PickUp
            if (!carryingPassenger && !lookingAtTicket)
            {
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1.5f, peopleRaycastMask))
                {
                    carryingPassenger = true;

                    #region Find the parent with passengerRagdollScript
                    Transform currentTransform = hit.transform;

                    while (currentTransform != null)
                    {
                        PassengerRagdoll passengerRagdollScript = currentTransform.GetComponent<PassengerRagdoll>();
                        if (passengerRagdollScript != null)
                        {

                            carriedPassenger = currentTransform.transform.parent.gameObject;
                            break;
                        }
                        currentTransform = currentTransform.parent;
                    }
                    #endregion

                    Transform scriptObjects = carriedPassenger.transform.GetChild(0).transform;
                    passengerRagdollScript = scriptObjects.GetComponent<PassengerRagdoll>();
                    passengerStateScript = scriptObjects.GetComponent<PassengerStates>();

                    passengerRagdollScript.toParent.transform.parent = playerShoulder.transform;
                    passengerRagdollScript.PickUp();

                    AlignParentWithChild(scriptObjects.transform, scriptObjects.transform.GetChild(0));
                    AlignParentWithChild(scriptObjects.transform.parent, scriptObjects.transform);

                    if (passengerStateScript.sittingPoint == null) return;
                    trainManagerScript.freeSeats.Add(passengerStateScript.sittingPoint);
                }
            }
            #endregion

            #region Throw
            else
            {
                if (carriedPassenger != null && !lookingAtTicket)
                {
                    carryingPassenger = false;
                    carriedPassenger.transform.parent = null;

                    //Throw Passenger
                    audioS.PlayOneShot(wouahSounds[Random.Range(0, wouahSounds.Length)]);
                    Vector3 finalDirection = playerCamera.transform.forward + Vector3.up * upwardForce;
                    passengerRagdollScript.Throw(finalDirection, throwForce);

                    carriedPassenger = null;
                }
            }
            #endregion
        }
        #region Drop
        if (carryingPassenger && Input.GetKeyDown(KeyCode.Mouse1))
        {
            carryingPassenger = false;
            carriedPassenger.transform.parent = null;
            carriedPassenger.transform.GetChild(0).gameObject.GetComponent<PassengerRagdoll>().Drop();

            carriedPassenger = null;
        }
        #endregion
        #endregion
    }

    void AlignParentWithChild(Transform parent, Transform child)
    {
        Vector3 childWorldPosition = child.position;
        Quaternion childWorldRotation = child.rotation;

        parent.position = childWorldPosition;
        parent.rotation = childWorldRotation;
    }

}
