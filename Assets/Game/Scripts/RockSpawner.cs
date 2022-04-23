using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject RockPrefab;

    public float TimeToDestroy;
    public float SpawnRate;

    public float Y_Spawn_Pos;
    public float min_X;
    public float max_X;

    private bool CanSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, TimeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIManager.instance.GameIsPaused && CanSpawn)
        {
            float randomX = Random.Range(min_X, max_X);

            Instantiate(RockPrefab, new Vector3(randomX, Y_Spawn_Pos, 0f), Quaternion.identity);
            StartCoroutine("SpawnCoolDown");
        }
    }

    IEnumerator SpawnCoolDown()
    {
        CanSpawn = false;

        yield return new WaitForSeconds(SpawnRate);

        CanSpawn = true;
    }
}
