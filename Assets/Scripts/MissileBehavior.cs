using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour {
    public float timeLimit = 2f;
    public GameObject ExplosionPrefab;
    public GameObject Monster;
    public float missileDamage = 50f;
	// Use this for initialization
	void Start () {
      //  Quaternion.LookRotation(Monster.transform.position);

    }
    
    // Update is called once per frame
    void Update () {
       // LookAtMonster();
        timeLimit -= Time.deltaTime;
        this.transform.Translate(Vector3.forward);
        if (timeLimit <= 0)
        {
            Detonate();
        }
    }

    public void Detonate()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    void LookAtMonster()
    {
        Quaternion look;
        transform.LookAt(Monster.transform.position);
        look = Quaternion.LookRotation(Monster.transform.position); //Quaternion.LookRotation(carPhysics.velocity.normalized);

        //// Rotate the camera towards the velocity vector.
        look = Quaternion.Slerp(transform.rotation, look, 5.0f * Time.fixedDeltaTime);
        this.transform.rotation = look;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject.tag == "Building")
        {
            other.gameObject.GetComponent<BuildingBehavior>().TakeDamage(missileDamage);
            Detonate();
        }
        else if (other.gameObject.tag == "Ground")
        {
            Debug.Log("ground missiled!");
            Detonate();
        }
         else if (other.gameObject.tag == "Grabbable" || other.gameObject.tag == "Shield")
        {
            Debug.Log("one for the money");
            other.gameObject.GetComponent<ThrowableBehavior>().TakeDamage(missileDamage);
            Detonate();
        }
    }
}
