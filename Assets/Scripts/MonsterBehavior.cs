using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
public class MonsterBehavior: MonoBehaviour {
   // public monsterTransform
    public float monsterHealth = 10000f;
    public float walkSpeed = 30;
    public float turnSpeed = 60;
    public float pushDamage = 100f;
    public AnimationClip[] anims;
    public GameObject monster;
    private Animation anim;

    // grab mechanics
    public bool isGrabbing = false;
    public bool isHolding = false;
    public GameObject currentItem;
    public float holdTimer = 1f;
    public bool holdTimerOn = false;
    public float throwMultiplier = 10f;

    //defense mechanics
    public bool isDucked = false;

    void Start () {
        anim = monster.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(0, 0, Input.GetAxis("Vertical") * walkSpeed * Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);

        Ray down_ray;
        Ray forward_ray;
        RaycastHit hit;

        down_ray = new Ray(transform.position, -1 * transform.up);
        forward_ray = new Ray(transform.position, transform.forward);

        if ((Physics.Raycast(down_ray, out hit, 1) || Physics.Raycast(forward_ray, out hit, 5)) && hit.transform.gameObject.tag == "Crushable")
        { 
           Debug.Log(hit.transform);
           Crush(hit.transform.gameObject);
        }
        //if ((Physics.Raycast(down_ray, out hit, 1) && hit.transform.gameObject.tag == "Ramp"))
        //{
        //    Debug.Log("ramp below");

        //} else
        //{
        //    this.transform.up = Vector3.up;
        //}

        if (holdTimerOn)
        {
            updateTimer();
        }

    }

       void updateTimer()
    {
        if(holdTimer <= 0)
        {
            holdTimerOn = false ;
            holdTimer = 1f;
        } else
        {
            holdTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Animate();
    }

      
    private void Animate()
    {
        if (monster.GetComponent<Animation>().clip != anims[2] || monster.GetComponent<Animation>().clip != anims[3])
        {
            isGrabbing = false;
        } 
        if(monster.GetComponent<Animation>().clip != anims[3] && isDucked)
        {
           // AttachGrabbable()
            isDucked = false;
            BoxCollider bc = GetComponent<BoxCollider>();
            bc.center = new Vector3(bc.center.x, 1.5f, bc.center.z);
        } 
        // grabbing
        if (Input.GetButton("Grab"))
        {
            if (isHolding && !holdTimerOn)
            {
                anim.clip = anims[2];
                anim.Play();
                DetachGrabbable(currentItem, Vector3.Lerp(transform.forward, transform.up, 0.5f));
            }
            else
            {
                isGrabbing = true;
                if (Input.GetButton("Defense"))
                {
                    anim.clip = anims[3];
                    isDucked = true;
                    BoxCollider bc = GetComponent<BoxCollider>();
                    bc.center = new Vector3(bc.center.x, 0.5f, bc.center.z);
                }
                else
                {
                    anim.clip = anims[2];
                }
                anim.Play();
             }

        }
        // resting
        else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            monster.GetComponent<Animation>().clip = anims[0];
            monster.GetComponent<Animation>().Play();
        }
        //walking
        else
        {
            monster.GetComponent<Animation>().clip = anims[1];
            monster.GetComponent<Animation>().Play();
        }
    }

    private void Crush(GameObject crushable)
    {
        crushable.GetComponent<CrushableBehavior>().Detonate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint _contact = collision.contacts[0];
        if (_contact.thisCollider.GetType() == typeof(BoxCollider) && _contact.otherCollider.gameObject.tag == "Grabbable" && isGrabbing && !isHolding)
        {
            AttachGrabbable(_contact.otherCollider.transform.gameObject);
        }
        if (_contact.thisCollider.GetType() == typeof(BoxCollider) && _contact.otherCollider.gameObject.tag == "Missile" && isHolding)
        {
            Debug.Log("missileBlock!");
            MissileBehavior missile =_contact.otherCollider.transform.gameObject.GetComponent<MissileBehavior>();
            missile.Detonate();
            currentItem.GetComponent<ThrowableBehavior>().TakeDamage(missile.missileDamage);
        }
    }

    public void AttachGrabbable(GameObject grabbable)
    {
        Debug.Log("attach item");
        Destroy(grabbable.GetComponent<Rigidbody>());
        grabbable.GetComponent<CapsuleCollider>().enabled = false;
        grabbable.transform.parent = this.transform;
        isHolding = true;
        holdTimerOn = true;
        currentItem = grabbable;
        grabbable.transform.localPosition = new Vector3(0.085f, 0.82f, 1.034f);
        grabbable.transform.rotation = Quaternion.Euler(-4.9f, -72.3f, 35.5f);
        grabbable.transform.rotation = monster.transform.rotation;

    }

    public void DetachGrabbable(GameObject grabbable, Vector3 force)
    {
        isHolding = false;
        Debug.Log("remove item");
        grabbable.transform.parent = null;
        grabbable.AddComponent<Rigidbody>();
        grabbable.transform.Translate(transform.forward);
        grabbable.GetComponent<Rigidbody>().AddForce(force * throwMultiplier, ForceMode.Impulse);
        grabbable.GetComponent<CapsuleCollider>().enabled = true;
        grabbable.GetComponent<Rigidbody>().useGravity = true;

    }
    private void OnTriggerEnter(Collider collisionObject)
    {
        if(collisionObject.gameObject.tag == "Crushable")
        {
            collisionObject.GetComponent<CrushableBehavior>().Detonate();
        }
        else if(collisionObject.gameObject.tag == "Building")
        {
          Debug.Log("you hit building!");
            collisionObject.GetComponent<BuildingBehavior>().MonsterAttack(pushDamage);
        }
        else if(collisionObject.gameObject.tag == "Missile")
        {
            float damage = collisionObject.gameObject.GetComponent<MissileBehavior>().missileDamage;
            collisionObject.gameObject.GetComponent<MissileBehavior>().Detonate();
            TakeDamage(damage);
        }
        else if(collisionObject.gameObject.tag == "Enemy")
        {
            Debug.Log("hit enemy");
            collisionObject.gameObject.GetComponent<HelicopterBehavior>().TakeDamage(pushDamage);

        }
    }

    //private void OnParticleCollision(GameObject other)
    //{

    //}

    public void TakeDamage(float damageLevel)
    {
        if (!isHolding)
        {
            monsterHealth -= damageLevel;
        } else
        {
            currentItem.GetComponent<ThrowableBehavior>().TakeDamage(damageLevel);
        }
    }

    private void OnTriggerExit(Collider collisionObject) { 
        if(collisionObject.gameObject.tag == "Building")
        {
            collisionObject.GetComponent<BuildingBehavior>().isMonsterIntersecting = false;
        }
    }
}
