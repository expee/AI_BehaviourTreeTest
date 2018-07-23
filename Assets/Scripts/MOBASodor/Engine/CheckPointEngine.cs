using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointEngine : MonoBehaviour {

    public float TimeToClaim = 5f;
    public GameManager.Team TeamOwner;

    private float TimeRemainingToClaim;
    private bool isRaid;
    private GameManager.Team WhoIsRaiding;

	// Use this for initialization
	void Start () {
        TeamOwner = GameManager.Team.E_NONE;
        TimeRemainingToClaim = TimeToClaim;
	}
	
	// Update is called once per frame
	void Update () {
        if (isRaid)
        {
            bool isChangeOwner = false;
            if (WhoIsRaiding == TeamOwner)
            {
                if(TimeRemainingToClaim < TimeToClaim)
                    TimeRemainingToClaim += Time.deltaTime;
            }
            else
            {
                if(TimeRemainingToClaim > 0)
                    TimeRemainingToClaim -= Time.deltaTime;
                else
                {
                    isChangeOwner = true;
                    if(TeamOwner != GameManager.Team.E_NONE)
                    {
                        TeamOwner = GameManager.Team.E_NONE;
                    }
                    else
                    {
                        TeamOwner = WhoIsRaiding;
                    }
                    TimeRemainingToClaim = TimeToClaim;
                }
            }
        }

	}
}
