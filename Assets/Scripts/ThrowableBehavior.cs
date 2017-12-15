using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBehavior : MonoBehaviour {
    public float itemHealth = 200f;
    public GameObject ExplosionPrefab;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damageAmount)
    {
        itemHealth -= damageAmount;
        if (itemHealth <= 0)
        {
           Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            this.transform.parent.GetComponent<MonsterBehavior>().isHolding = false;
            Destroy(this.gameObject);
        }
    }


}
