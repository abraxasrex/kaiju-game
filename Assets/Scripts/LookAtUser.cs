using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUser : MonoBehaviour {

    public GameObject followCam;
    // Use this for initialization
    void Start()
    {
        followCam = GameObject.Find("Main Camera");
        Debug.Log(followCam);
    }

    // Update is called once per frame
    void Update()
    {
        //if (followCam)
        //{
        transform.rotation = Quaternion.LookRotation(followCam.transform.position);
        //}

    }

    private void LateUpdate()
    {
        //   transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z);

    }
}
