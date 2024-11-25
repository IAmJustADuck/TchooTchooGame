using UnityEngine;

public class ScoreScript : MonoBehaviour
{   
    [SerializeField] private bool scriptEnabled = false;
    
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private bool stopPhase = false;
    

    private void OnTriggerEnter(Collider other)
    {
        if (!scriptEnabled) return;

        if (other.CompareTag("Passenger"))
        {
            PassengerRagdoll interactScript = other.transform.parent.GetComponent<PassengerRagdoll>();

            if (interactScript != null)
            {
                if (!stopPhase)
                {
                    if (interactScript.hasTicket)
                    {
                        scoreData.score--;
                    }
                    else
                    {
                        scoreData.score++;
                    }
                    Destroy(other.gameObject, 5f);
                }
            }
        }
    }
}
