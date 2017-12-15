using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehavior : MonoBehaviour {

    public GameObject ExplosionPrefab;
    public bool isMonsterIntersecting = false;
    public float monsterAttackLevel;
    public float buildingHealth = 1000f;
    public bool damageTimerOn = false;
    public float damageDelay = 1f;
    public float damageAmount = 1f;
    public float damagePerHit = 100f;
    public float buildingMass = 10000f;
    //gggggg
    // Use this for initialization
    private void Awake()
    {
        this.GetComponent<Rigidbody>().mass = buildingMass;
    }
    void Start () {
		
	}
    private void OnTriggerEnter(Collider thisBoxCollider)
    {
       // Debug.Log("triggerd");
    }
    // Update is called once per frame
    void Update () {

        //if(isMonsterIntersecting && !damageTimerOn)
        //{

        //}
        if (isMonsterIntersecting)
        {
            checkDamage();
        }
       // checkDamage();
	}
    public void checkDamage()
    {
        if (damageTimerOn)
        {
            damageDelay -= Time.deltaTime;
           // Debug.Log(damageDelay);
        }
        if(damageDelay <= 0)
        {
            damageDelay = damageAmount;
            TakeDamage(monsterAttackLevel);
        }
    }

    public void MonsterAttack(float damageAmount)
    {
        isMonsterIntersecting = true;
        if (!damageTimerOn)
        {
            startDamageTimer(damageAmount);
        }
    }

    void startDamageTimer(float damageAmount)
    {
        damageTimerOn = true;
        Debug.Log("start damageeeeeeeeeeeeeeeeeeeeeee");
        TakeDamage(damageAmount);
    }

    public void TakeDamage(float damageAmount)
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        buildingHealth -= damageAmount;
        Debug.Log("building hit once!");
       // Debug.Log(buildingHealth);
        if(buildingHealth <= 0)
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
