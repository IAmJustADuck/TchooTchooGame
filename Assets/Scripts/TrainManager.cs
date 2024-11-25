using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class TrainManager : MonoBehaviour
{
    public bool next;

    [Header("Components")]
    [SerializeField] private GameObject outsideTrain;
    [SerializeField] private PassengerManager passengerMan;
    private Animator anim;
    
    [Header("Delays")]
    [SerializeField] private float delayBeforeStop = 30f;
    [SerializeField] private float delayBeforeStart = 10f;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] arrivingAnnouncements;
    [SerializeField] private AudioSource HautParleur;
    [SerializeField] private AudioSource musique;

    [Header("Arrays")]
    [SerializeField] private GameObject stationPassengers;
    [SerializeField] private GameObject trainPassengers;

    [SerializeField] private List<Transform> seats;
    public List<Transform> freeSeats;

    void Start()
    {
        anim = outsideTrain.GetComponent<Animator>();
        freeSeats = new List<Transform>(seats);
        StartCoroutine(StartSlowDown());
    }

    IEnumerator StartSlowDown()
    {
        passengerMan.spawnNextPassengers(10);
        yield return new WaitForSeconds(15f);
        anim.SetTrigger("SlowDown");
        yield return new WaitForSeconds(3f);

        musique.volume = .15f;
        HautParleur.PlayOneShot(arrivingAnnouncements[Random.Range(0, arrivingAnnouncements.Length)]); //Play Announcement
        yield return new WaitForSeconds(5); //Waiting for sound
        musique.volume = .8f;

        yield return new WaitForSeconds(5); //Waiting for anim to complete
        
        AssignSeats();
        StartCoroutine(WaitForPassengers());
    }

    IEnumerator DelayStopTrain()
    {
        passengerMan.spawnNextPassengers(freeSeats.Count);
        yield return new WaitForSeconds(delayBeforeStop - 11f); //Slow down anim length is (delay-11)
        anim.SetTrigger("SlowDown");
        yield return new WaitForSeconds(3);

        musique.volume = .15f;
        HautParleur.PlayOneShot(arrivingAnnouncements[Random.Range(0, arrivingAnnouncements.Length)]); //Play Announcement
        yield return new WaitForSeconds(5); //Waiting for sound
        musique.volume = .8f;

        yield return new WaitForSeconds(5); //Waiting for anim to complete

        CheckForPassengerStop();
        AssignSeats();

        StartCoroutine(WaitForPassengers());
    }

    void CheckForPassengerStop()
    {
        foreach (Transform obj in trainPassengers.transform)
        {
            obj.GetChild(0).GetComponent<PassengerStates>().ReachedStop();
        }
    }

    void AssignSeats()
    {
        //Anti bug d'itération (surement à opti plus tard)
        List<Transform> passengers = new List<Transform>();
        foreach (Transform obj in stationPassengers.transform)
        {
            passengers.Add(obj);
        }

        //Liste temp
        foreach (Transform obj in passengers)
        {
            GameObject passenger = obj.transform.GetChild(0).gameObject;

            Transform pickedSeat = freeSeats[Random.Range(0, freeSeats.Count)];
            passenger.GetComponent<PassengerStates>().sittingPoint = pickedSeat; // Assign seat
            freeSeats.Remove(pickedSeat); // Remove seat from free seats
            Debug.Log(pickedSeat);

            passenger.GetComponent<NavMeshAgent>().enabled = true; // Enable navmesh
            passenger.GetComponent<PassengerStates>().MoveToSittingPoint(); // Move to seat (je t'en supplie fonctionne)
            obj.transform.parent = trainPassengers.transform;
        }
    }


    IEnumerator WaitForPassengers()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        anim.SetTrigger("SpeedUp");
        yield return new WaitForSeconds(10f);
        StartCoroutine(DelayStopTrain());
    }
}
