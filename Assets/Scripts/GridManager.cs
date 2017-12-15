using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public List<GameObject> TileGrid;
    public GameObject Ground;
    public float expectedTileMin = 140;
	// Use this for initialization
	void Start () {
        TileGrid = new List<GameObject>();
        foreach(Transform tile in Ground.transform)
        {
            TileGrid.Add(tile.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(TileGrid.Count > expectedTileMin)
        {
            foreach( GameObject tile in TileGrid)
            {
                TileBehavior tileScript = tile.GetComponent<TileBehavior>();
                if(!tileScript.containsBuilding)
                tileScript.isOpen = true;
            }
        }
	}

    //public void 
}
