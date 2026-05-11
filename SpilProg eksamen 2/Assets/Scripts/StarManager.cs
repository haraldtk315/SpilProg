using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public GameObject starPrefab;
    public StarBehaviour starBehaviour;
    public LayerMask groundLayer;

    private GameObject currentStar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnStar();
    }

    /*
    private void OnTriggerEnter2D(Collider2D other)
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
    */

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
        if (currentStar != null) return;

        Vector3 spawnPos;
        bool validPosition = false;

        Vector2 mapSize = BorderManager.size;
        Vector2 offset = new Vector2(BorderManager.instance.leftLimit, BorderManager.instance.bottomLimit);

        for (int i = 0; i < 20; i++) 
        {
            spawnPos = new Vector3(
                Random.Range(offset.x + 1, offset.x + mapSize.x - 1),
                Random.Range(offset.y + 1, offset.y + mapSize.y - 1),
                1.85f
            );

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.3f, groundLayer);

            if (hit == null)
            {
                validPosition = true;
                currentStar = Instantiate(starPrefab, spawnPos, Quaternion.identity);
                break;
            }
        }

    }


}
