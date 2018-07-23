using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManagement : MonoBehaviour {

    public GameManager.Team Team;
    public List<GameObject> ListPlayerInside;
    public SpawnManager TeamSpawn;
    public float Range = 10000f;

    private void Start()
    {
        ListPlayerInside = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        
        if(collision.transform.tag == "Player")
        {
            AgentController agen = collision.transform.GetComponent<AgentController>();
            if (agen != null)
            {
                if (agen.Team != Team)
                    GameManager.GetInstance().SetWin(agen.Team);
                else
                {
                    agen.RefreshPower();
                    if(ListPlayerInside.Find(x => x.name == collision.name) == null)
                    {
                        //Debug.Log("Player Enter -> " + ListPlayerInside.Count + ", name : " + collision.transform.name);
                        ListPlayerInside.Add(collision.gameObject);
                    }
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            AgentController agen = other.transform.GetComponent<AgentController>();
            if (agen != null)
            {
                if (agen.Team == Team)
                {
                    agen.DrainPower();
                    ListPlayerInside.Remove(other.gameObject);
                }
                    
            }
        }
    }

    public GameObject GetNearestEnemy()
    {
        float enemyNearestDistance = 0f;
        GameObject nearestEnemy = null;

        Collider[] enemies = Physics.OverlapSphere(transform.position, Range);
        foreach(Collider enemy in enemies)
        {
            if(enemy.tag == "Player")
            {
                AgentController agen = enemy.GetComponent<AgentController>();
                if (agen != null && agen.Team != Team)
                {
                    
                    if (enemyNearestDistance > 0 && enemyNearestDistance > Vector3.Distance(agen.transform.position, transform.position))
                    {
                        enemyNearestDistance = Vector3.Distance(agen.transform.position, transform.position);
                        nearestEnemy = agen.gameObject;
                    }
                    else
                    {
                        enemyNearestDistance = Vector3.Distance(agen.transform.position, transform.position);
                        nearestEnemy = agen.gameObject;
                    }
                }
            }
        }

        return nearestEnemy;
    }

    public GameObject GetPlayerNearestWithTower()
    {
        List<GameObject> players = TeamSpawn.ListTeamPlayer;
        GameObject nearestPlayer = null;
        float nearestDistance = 0;
        foreach(GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if(nearestDistance > 0 && nearestDistance > distance)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }

        }
        return nearestPlayer;
    }
}
