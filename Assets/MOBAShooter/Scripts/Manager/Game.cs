using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game> {

    protected override void Init()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        covers = new List<Obstacle.BoxColliderDescriptor>();
    }

    // Use this for initialization
    void Start ()
    {
        GameObject[] coverGameObjects = GameObject.FindGameObjectsWithTag("Obstacle");
        for(int i = 0; i < coverGameObjects.Length; i++)
        {
            covers.Add(coverGameObjects[i].GetComponent<Obstacle.BoxColliderDescriptor>());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<Obstacle.BoxColliderDescriptor> covers;
}
