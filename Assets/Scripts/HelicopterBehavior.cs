using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelicopterBehavior : MonoBehaviour {
    public float copterHealth = 100f;
    public float copterHeight = 25f;
    public List<Attack> AttacksArray;
    public GameObject Monster;
    public float speed = 1f;
    public bool Attacking = false;
    public GameObject MissilePrefab;
    public float attackFrequency = 10f;
    public float timeTillAttack = 10f;
    public float shootDistance = 100f;
    public GameObject ExplosionPrefab;
    public bool loadingNextPosition = false;
    float rotLag = 2.5f;

    // pathfinding
    public Vector3 lastTilePosition;
    public Vector3 currentTilePosition;
    public Vector3 nextTilePosition;
    public float pathfindingRadius = 100f;

    // attack tells
    public GameObject thisLight;

    // guns
    public GameObject gunParticles;


	// Use this for initialization
	void Start () {
        nextTilePosition = transform.position;
	}

	private List<Attack> BootstrapAttacks()
    {
        return new List<Attack>
        {
            new Attack()
            {
                AttackLevel = 100f,
                AttackName = "MachineGun"
            },
            new Attack()
            {
                 AttackLevel = 200f,
                AttackName = "Missile"
            }
        };
    }

	// Update is called once per frame
	void Update () {
        loadingNextPosition = checkForPathUpdate();
        if (loadingNextPosition)
        {
            findPath();
        }
        else if (Vector3.Distance(Monster.transform.position, this.transform.position) > shootDistance)
            {
           // Debug.Log("moving moving moving....")
               moveTowardsNextPosition();
             }
            else if(Vector3.Distance(Monster.transform.position, this.transform.position) <= shootDistance)
            {
                LookAtMonster();
                if (Attacking)
                {
                    Attack();
                }
             UpdateTimer();
        }

        if (copterHealth <= 0)
        {
            CopterDeath();
        }
    }

    bool checkForPathUpdate()
    {
        if(Vector3.Distance(transform.position,nextTilePosition) < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void findPath()
    {
        Ray down_ray = new Ray(transform.position, -1 * transform.up);
        RaycastHit hit;

        if (Physics.Raycast(down_ray, out hit, copterHeight * 2) && (hit.collider.transform.gameObject.tag == "OpenTile" || hit.collider.transform.gameObject.tag == "ClosedTile"))
        {
            currentTilePosition = new Vector3(hit.collider.transform.position.x, 0, hit.collider.transform.position.z);
            lastTilePosition = currentTilePosition;
            nextTilePosition =  getNextPosition();
            loadingNextPosition = false;
        }
    }

    void moveTowardsNextPosition()
    {
        this.transform.position = Vector3.Slerp(transform.position, new Vector3(nextTilePosition.x, copterHeight, nextTilePosition.z), (speed * Time.deltaTime));
    }

    void UpdateTimer()
    {
        if(timeTillAttack >= 0f)
        {
            timeTillAttack -= Time.deltaTime;
        } else
        {
            Attacking = true;
            ResetAttackTimer();
        }
    }


    public Vector3 getNextPosition()
    {
        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(currentTilePosition.x, 0, currentTilePosition.z ), pathfindingRadius);
        int i = 0;
        List<Vector3> possibleNexts = new List<Vector3>();
  
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].transform.gameObject.tag == "OpenTile")
            {
                
                if (hitColliders[i].transform.position.x != currentTilePosition.x || hitColliders[i].transform.position.z != currentTilePosition.z)
                {
                    //Debug.Log("added " + hitColliders[i].transform.position.ToString() + "to possibles!");
                    possibleNexts.Add(new Vector3(hitColliders[i].transform.position.x, copterHeight, hitColliders[i].transform.position.z));
                } else
                {
                  //  Debug.Log("current (rejected) square is number :" + i);
                }
            } 
            i++;
        }
        if(possibleNexts.Count > 0)
        {
           // Debug.Log("possible squares");
            return possibleNexts.OrderBy(
                x => Vector3.Distance(x, Monster.transform.position)
            ).ToList().First();
        } else
        {
           // Debug.Log("no possible squares");
            return this.transform.position;
        }

    }

    void LookAtMonster()
    {
        Quaternion look;
        transform.LookAt(new Vector3(Monster.transform.position.x, Monster.transform.position.y + 25f, Monster.transform.position.z));

        look = Quaternion.LookRotation(Monster.transform.position); //Quaternion.LookRotation(carPhysics.velocity.normalized);

        //// Rotate the camera towards the velocity vector.
        look = Quaternion.Slerp(transform.rotation, look, rotLag * Time.fixedDeltaTime);
        this.transform.rotation = look;
    }

    void Attack()
    {
        Attacking = false;
        thisLight.SetActive(true);
        this.gameObject.GetComponent<CopterLight>().enabled = true;

        float rnd = Random.Range(0, 3);
        Color clr = Color.blue;
        string clrStr = "blue";
        Debug.Log(rnd);
        if(rnd > 1)
        {
            clr = Color.red;
            clrStr = "red";
        }
        this.gameObject.GetComponent<CopterLight>().getColor(clr);
        StartCoroutine(manageLights(clrStr));
    }

    IEnumerator manageLights(string color)
    {
        yield return new WaitForSeconds(1f);
        thisLight.SetActive(false);
        this.gameObject.GetComponent<CopterLight>().enabled = false;
        Debug.Log(color);
        if(color == "red")
        {
            Debug.Log("missile");
            FireMissile();
        } else
        {
            FireMachineGun();
            Debug.Log("machine gun");
        }
        ResetAttackTimer();
    }

    IEnumerator continueShooting()
    {
        yield return new WaitForSeconds(0.5f);
        if (gunParticles.activeSelf == true)
        {
            Debug.Log("shooty");
            Ray shoot_line = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(shoot_line, out hit, shootDistance) && (hit.collider.transform.gameObject.tag == "Player" || hit.collider.transform.gameObject.tag == "Grabbable"))
            {
                if (hit.collider.transform.gameObject.tag == "Grabbable")
                {
                    hit.transform.gameObject.GetComponent<ThrowableBehavior>().TakeDamage(1f);
                }
                if (hit.collider.transform.gameObject.tag == "Player")
                {
                    hit.transform.gameObject.GetComponent<MonsterBehavior>().TakeDamage(1f);
                }
            }
            StartCoroutine(continueShooting());
        }
    }

    void FireMissile()
    {
        GameObject missile = Instantiate(MissilePrefab, this.transform.position, this.transform.rotation);
        missile.GetComponent<MissileBehavior>().Monster = Monster;
    }
    void FireMachineGun()
    {
        gunParticles.SetActive(true);

        // blah

        //Ray down_ray = new Ray(transform.position, -1 * transform.up);
        //RaycastHit hit;

        //if (Physics.Raycast(down_ray, out hit, copterHeight * 2) && (hit.collider.transform.gameObject.tag == "opentile" || hit.collider.transform.gameObject.tag == "closedtile"))
        //{
        //    currentTilePosition = new Vector3(hit.collider.transform.position.x, 0, hit.collider.transform.position.z);
        //    lastTilePosition = currentTilePosition;
        //    nextTilePosition = getNextPosition();
        //    loadingNextPosition = false;
        //}

        StartCoroutine(turnOffGuns());
        StartCoroutine(continueShooting());
        //Ray shoot_line = new Ray(transform.position, transform.forward);
        //RaycastHit hit;

        //while(gunParticles.activeSelf == true)
        //{


        //if (Physics.Raycast(shoot_line, out hit, shootDistance) && (hit.collider.transform.gameObject.tag == "Player" || hit.collider.transform.gameObject.tag == "Grabbable"))
        //{
        //    if(hit.collider.transform.gameObject.tag == "Grabbable")
        //    {
        //        hit.transform.gameObject.GetComponent<ThrowableBehavior>().TakeDamage(0.1f);
        //    }
        //    if (hit.collider.transform.gameObject.tag == "Player")
        //    {
        //        hit.transform.gameObject.GetComponent<MonsterBehavior>().TakeDamage(0.1f);
        //    }
        //}
        //}

    }

    IEnumerator turnOffGuns()
    {
        yield return new WaitForSeconds(4f);
        gunParticles.SetActive(false);
    }

    void ResetAttackTimer()
    {
        timeTillAttack = attackFrequency;
    }

    public void TakeDamage(float damageLevel)
    {
        copterHealth -= damageLevel;
        if(copterHealth <= 0)
        {
            CopterDeath();
        }
    }

    void CopterDeath()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
