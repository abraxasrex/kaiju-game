using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InitStage : MonoBehaviour {
    public GameObject Ground;
    public GameObject Tile_Prefab;
    public int numberOfRows = 12;
    public int numberOfColumns = 12;
    public float sizeMultiplier = 1f;
	// Use this for initialization
	void Start () {

        foreach( Transform child in Ground.transform)
        {
            /// why no destroyyyy
            Destroy(child.gameObject);
        }

    

        // save constructed stage for sandboxing
        //string prefabPath = "Assets/Prefabs/ProgramaticGround.prefab";
        //object emptyPrefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/ProgramaticGround.prefab");
        //PrefabUtility.CreatePrefab(prefabPath, Ground);

    }
	public void continueInit()
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                float xCoord = (i - 6) * sizeMultiplier;
                float zCoord = (j - 6) * sizeMultiplier;
                AppendTile(xCoord, zCoord);
            }
        }

        string prefabPath = "Assets/Prefabs/ProgramaticGround.prefab";
      //  object emptyPrefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/ProgramaticGround.prefab");
        PrefabUtility.CreatePrefab(prefabPath, Ground);

    }
	// Update is called once per frame
	void Update () {
        if (Ground.transform.childCount < 1)
        {
            continueInit();
        }

    }

    void AppendTile(float x, float y)
    {
        GameObject newTile = Instantiate(Tile_Prefab, new Vector3(x, 0.5f, y), Quaternion.identity);
         newTile.transform.parent = Ground.transform;

         newTile.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        newTile.transform.localPosition = new Vector3(x, 0.5f, y);
        newTile.tag = "Tile";
    }
}
