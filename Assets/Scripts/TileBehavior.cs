using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour {
    public bool isOpen = true;
    public bool containsBuilding = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        manageTiles();
	}


    void manageTiles()
    {   
        Ray up_ray = new Ray(transform.position, transform.up);
        RaycastHit hit;
      //  bool _isOpen = hit.transform.gameObject.GetComponent<TileBehavior>().isOpen;

        if (Physics.Raycast(up_ray, out hit, 35) && isManeuverable(hit.transform.gameObject.tag))
        {
            isOpen = true;
            this.gameObject.tag = "ClosedTile";
        }
        else
        {
            isOpen = false;
            this.gameObject.tag = "OpenTile";
        }
    }

    bool isManeuverable(string tagName)
    {
        return tagName != "Building" && tagName != "Player" && tagName != "Enemy";
    }
}
