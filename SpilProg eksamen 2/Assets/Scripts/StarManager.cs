using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StarManager : NetworkBehaviour
{
    public GameObject starPrefab;
    public StarBehaviour starBehaviour;
    public LayerMask groundLayer;

    private NetworkObject currentStar;
    private bool isSpawningStar = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnStar();
        }
    }

    public IEnumerator DestroyStar()
    {
        if (!IsServer)
            yield break;

        yield return new WaitForSeconds(0.1f);

        if (currentStar != null && currentStar.IsSpawned)
        {
            currentStar.Despawn(true);
        }
        else if (currentStar != null)
        {
            Destroy(currentStar.gameObject);
        }

        currentStar = null;

        yield return new WaitForSeconds(0.1f);

        SpawnStar();
    }

    public void SpawnStar()
    {
        if (!IsServer)
            return;

        if (isSpawningStar)
            return;

        if (currentStar != null && currentStar.IsSpawned)
            return;

        if (starPrefab == null)
        {
            Debug.LogWarning("StarManager: starPrefab is missing.");
            return;
        }

        isSpawningStar = true;

        Vector3 spawnPos = Vector3.zero;
        bool validPosition = false;

        Vector2 mapSize = BorderManager.size;
        Vector2 offset = new Vector2(
            BorderManager.instance.leftLimit,
            BorderManager.instance.bottomLimit
        );

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
                break;
            }
        }

        if (!validPosition)
        {
            Debug.LogWarning("StarManager: Could not find a valid star spawn position.");
            isSpawningStar = false;
            return;
        }

        GameObject starObject = Instantiate(starPrefab, spawnPos, Quaternion.identity);

        currentStar = starObject.GetComponent<NetworkObject>();

        if (currentStar == null)
        {
            Debug.LogWarning("StarManager: Star prefab needs a NetworkObject.");
            Destroy(starObject);
            isSpawningStar = false;
            return;
        }

        StarBehaviour spawnedStarBehaviour = starObject.GetComponent<StarBehaviour>();

        if (spawnedStarBehaviour != null)
        {
            spawnedStarBehaviour.starManager = this;
        }

        currentStar.Spawn(true);

        isSpawningStar = false;
    }

    public void ClearCurrentStar()
    {
        if (!IsServer)
            return;

        currentStar = null;
    }
}