using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerManager : MonoBehaviour
{
    [Header("Spawn related")]
    [SerializeField] private BoxCollider spawnZone;
    [SerializeField] private GameObject passenger;
    [SerializeField] private GameObject[] passengerPrefab;
    [SerializeField] private Transform stationGameobject;
    [SerializeField] private float minSpawnDistance = 1.5f;

    private List<Vector3> spawnPositions = new List<Vector3>();


    public void spawnNextPassengers(float passengerNmbr)
    {
        int spawnedCount = 0;

        while (spawnedCount < passengerNmbr)
        {
            //Spawn random
            float x = Random.Range(spawnZone.bounds.min.x, spawnZone.bounds.max.x);
            float y = 0f;
            float z = Random.Range(spawnZone.bounds.min.z, spawnZone.bounds.max.z);
            Vector3 spawnPosition = new Vector3(x, y, z);

            //Anti overlap & Spawn
            if (ValidSpawn(spawnPosition))
            {
                passenger = Instantiate(passengerPrefab[Random.Range(0, passengerPrefab.Length)], spawnPosition, Quaternion.Euler(0f, -90f, 0f), stationGameobject);
                spawnPositions.Add(spawnPosition);
                spawnedCount++;

                passenger.transform.GetChild(0).GetComponent<PassengerStates>().exitZone = spawnZone;
                passenger.transform.GetChild(0).GetComponent<PassengerStates>().stationParent = stationGameobject.transform;
            }
        }
    }

    private bool ValidSpawn(Vector3 position)
    {
        foreach (Vector3 existingPosition in spawnPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minSpawnDistance)
            {
                return false;
            }
        }
        return true;
    }
}
