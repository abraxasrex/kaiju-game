using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushableBehavior : MonoBehaviour {
    public bool isBeingDestroyed = false;
    public GameObject ExplosionPrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Detonate()
    {
        if (!isBeingDestroyed)
        {
            isBeingDestroyed = true;
            Debug.Log("detonate!!!!!");
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);

        }
     
    }
}
