using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public int maxSpawnCharacter = 4;
    public GameManager.Team TeamColor;
    public GameObject Agent;
    public float SpawnRange;

    public static int CurrentCharacterAlreadySpawn;
    [HideInInspector] public List<GameObject> ListTeamPlayer;

    private int PlayerId;
    
    private void Awake()
    {
        CurrentCharacterAlreadySpawn = 0;
    }

    // Use this for initialization
    void Start () {
        ListTeamPlayer = new List<GameObject>();
        for(int i = 0; i < maxSpawnCharacter; i++)
        {
            StartCoroutine("SpawnAgent");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (CurrentCharacterAlreadySpawn < maxSpawnCharacter)
        {
            StartCoroutine("SpawnAgent");
        }
    }

    IEnumerator SpawnAgent()
    {
        yield return new WaitForSeconds(0);

        float randomValueX = Random.value * SpawnRange;
        float randomValueZ = Random.value * SpawnRange;

        Vector3 agentPosition = transform.position + new Vector3(randomValueX, 0, randomValueZ);

        GameObject obj = Instantiate(Agent, agentPosition, transform.rotation);

        obj.name = TeamColor + "_" + PlayerId;

        ListTeamPlayer.Add(obj);
        CurrentCharacterAlreadySpawn++;
        PlayerId++;
    }


}
