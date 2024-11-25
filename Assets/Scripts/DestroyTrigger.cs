using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    private string targetLayerName = "People";
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            Destroy(other.transform.root.gameObject);
        }
    }
}
