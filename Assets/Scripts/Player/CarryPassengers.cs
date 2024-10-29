using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryPassengers : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Passengers")]
    [SerializeField] private LayerMask peopleRaycastMask;
    [SerializeField] private GameObject playerShoulder;
    private bool carryingPassenger = false;
    private GameObject carriedPassenger;

    [Header("Throw force")]
    [SerializeField] private float throwForce = 3.5f;
    [SerializeField] private float upwardForce = 1.25f;


    void Update()
    {
        #region Pick up / Throw passenger / Drop passenger
        if (Input.GetMouseButtonDown(0))
        {
            #region PickUp
            if (!carryingPassenger)
            {
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1.5f, peopleRaycastMask))
                {
                    carryingPassenger = true;
                    carriedPassenger = hit.transform.root.gameObject;

                    hit.transform.root.transform.GetChild(0).GetComponent<PassengerScript>().PickUp();
                    hit.transform.root.transform.position = playerShoulder.transform.position;
                    hit.transform.root.transform.rotation = playerShoulder.transform.rotation;

                    //Set parent at the end to avoid messing up the root
                    hit.transform.root.parent = playerShoulder.transform;
                }
            }
            #endregion

            #region Throw
            else
            {
                if (carriedPassenger != null)
                {
                    carryingPassenger = false;
                    carriedPassenger.transform.parent = null;

                    //Throw Passenger
                    Vector3 finalDirection = playerCamera.transform.forward + Vector3.up * upwardForce;
                    carriedPassenger.transform.GetChild(0).gameObject.GetComponent<PassengerScript>().ThrowPassenger(finalDirection, throwForce);

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
            carriedPassenger.transform.GetChild(0).gameObject.GetComponent<PassengerScript>().DropPassenger();

            carriedPassenger = null;
        }
        #endregion
        #endregion
    }
}
