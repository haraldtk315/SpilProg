using System.Collections;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public GameObject starPrefab;
    private GameObject currentStar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SpawnStar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Increase player 1's score by 1
            StartCoroutine(DestroyStar());
        }

        if (other.gameObject.CompareTag("Player2"))
        {
            //Increase player 2's score by 1
            StartCoroutine(DestroyStar());
        }
    }


    public IEnumerator DestroyStar()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(currentStar);
        currentStar = null;
        yield return new WaitForSeconds(0.5f);
        //Spawn a new star at a random position within the bounds of the game area
        SpawnStar();
    }

    public void SpawnStar()
    {
        if (currentStar == null)
        {
            Vector3 randomPosition = new Vector3(Random.Range((float)-9.16, (float)9.16), Random.Range((float)-3.76, (float)5.69), 0);
            currentStar = Instantiate(starPrefab, randomPosition, Quaternion.identity);
        }
    }


}
