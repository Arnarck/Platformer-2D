using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivator : MonoBehaviour
{
    private EarthGuardian earthGuardian;

    private void Start()
    {
        earthGuardian = GameObject.FindGameObjectWithTag("Boss").GetComponent<EarthGuardian>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraBehaviour>().ReachedBossArea = true;
            UIManager.instance.BossInterfaces.SetActive(true);
            earthGuardian.isAlive = true;
            earthGuardian.StartCoroutine("CommomAttacksCoolDown");
            //GameObject.FindGameObjectWithTag("Boss").transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Destroy(this.gameObject);
        }
    }
}
