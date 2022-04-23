using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGuardianHammer : MonoBehaviour
{
    public GameObject[] ScenarioChangeEffects;
    public GameObject[] SpecialAttacks;
    public GameObject WeaponSwoosh;
    public GameObject ImpactPrefab;
    public GameObject SpecialAttackEffectPrefab;
    public Transform CenterPoint;

    public int BasicDamage;
    public int CloseDamage;
    public int SpecialDamage;

    private Animator animator;
    private Collider2D AttackCollider;

    private enum TypeOfAttack
    {
        Close,
        Basic,
        Special,
        ChangeScenario
    }

    private TypeOfAttack Attack;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        AttackCollider = GetComponent<Collider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            CheckTypeOfAttack();
        }

        if (collision.CompareTag("Player"))
        {
            if (Attack == TypeOfAttack.Basic)
            {
                collision.GetComponent<PlayerController>().TakeDamage(transform.rotation.y, BasicDamage);
            }
            else if (Attack == TypeOfAttack.Close)
            {
                collision.GetComponent<PlayerController>().TakeDamage(transform.rotation.y, CloseDamage);
            }
            else
            {
                collision.GetComponent<PlayerController>().TakeDamage(transform.rotation.y, SpecialDamage);
            }
        }
    }

    public void CheckTypeOfAttack()
    {
        GameObject impact = Instantiate(ImpactPrefab, new Vector3(CenterPoint.position.x, -12f, CenterPoint.position.z), Quaternion.identity);

        AttackCollider.enabled = false;
        impact.GetComponent<HammerImpact>().SetRotation(transform.rotation.y);

        if (Attack == TypeOfAttack.Basic)
        {
            impact.GetComponent<HammerImpact>().SetDamage(BasicDamage / 2);
        }
        else if (Attack == TypeOfAttack.Close)
        {
            impact.GetComponent<HammerImpact>().SetDamage(CloseDamage / 2);
        }
        else if (Attack == TypeOfAttack.Special)
        {
            int result = Random.Range(0, SpecialAttacks.Length);

            Instantiate(SpecialAttackEffectPrefab, new Vector3(CenterPoint.position.x, -12f, 0f), Quaternion.Euler(-90f, 0f, 0f));
            impact.GetComponent<HammerImpact>().SetDamage(SpecialDamage / 2);

            if (result == 0)
            {
                Instantiate(SpecialAttacks[0], new Vector3(260f, 5f, 0f), Quaternion.identity);
            }
            else
            {
                Instantiate(SpecialAttacks[1], new Vector3(260f, -12f, 0f), Quaternion.identity);
            }
        }
        else
        {
            Instantiate(SpecialAttackEffectPrefab, new Vector3(CenterPoint.position.x, -12f, 0f), Quaternion.Euler(-90f, 0f, 0f));
            impact.GetComponent<HammerImpact>().SetDamage(SpecialDamage / 2);

            for (int i = 0; i < ScenarioChangeEffects.Length; i++)
            {
                Instantiate(ScenarioChangeEffects[i], new Vector3(260f, -12f, 0f), Quaternion.identity);
            }
        }
    }

    public void BasicAttackAnimation(bool state)
    {
        Attack = TypeOfAttack.Basic;

        AttackCollider.enabled = state;
        WeaponSwoosh.SetActive(state);
        animator.SetBool("BasicAttack", state);
    }

    public void CloseAttackAnimation(bool state)
    {
        Attack = TypeOfAttack.Close;

        AttackCollider.enabled = state;
        WeaponSwoosh.SetActive(state);
        animator.SetBool("CloseAttack", state);
    }

    public void SpecialAttackAnimation(bool state)
    {
        Attack = TypeOfAttack.Special;

        AttackCollider.enabled = state;
        WeaponSwoosh.SetActive(state);
        animator.SetBool("SpecialAttack", state);
    }
    
    public void ChangeScenarioAttackAnimation(bool state)
    {
        Attack = TypeOfAttack.ChangeScenario;

        AttackCollider.enabled = state;
        WeaponSwoosh.SetActive(state);
        animator.SetBool("SpecialAttack", state);
    }
}
