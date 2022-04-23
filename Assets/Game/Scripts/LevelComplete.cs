using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public GameObject FinalJumpPoint;
    private CameraBehaviour cameraBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        cameraBehaviour = Camera.main.GetComponent<CameraBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DelayToLevelComplete()
    {
        Destroy(GameObject.Find("FirePoints(Clone)"));
        Destroy(GameObject.Find("JumpPoints(Clone)"));
        FinalJumpPoint.SetActive(true);
        yield return new WaitForSeconds(3f);
        UIManager.instance.BossInterfaces.SetActive(false);
        cameraBehaviour.BossIsDefeated = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UIManager.instance.LevelComplete();
        }
    }
}
