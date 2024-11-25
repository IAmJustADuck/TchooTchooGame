using UnityEngine;

public class TrainExterior : MonoBehaviour
{
    [SerializeField] private float accelerationForce = 15f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
        }
    }
}
