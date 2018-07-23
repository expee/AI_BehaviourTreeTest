using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointEngine : MonoBehaviour {

    public float TimeToClaim = 5f;
    public GameManager.Team TeamOwner;

    private float TimeRemainingToClaim;
    private bool isRaid;
    private GameManager.Team WhoIsRaiding;
    private Renderer m_objMaterial;

	// Use this for initialization
	void Start () {
        TeamOwner = GameManager.Team.E_NONE;
        TimeRemainingToClaim = TimeToClaim;
        m_objMaterial = GetComponent<Renderer>();
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
            if (isChangeOwner)
            {
                Color color;
                switch(TeamOwner)
                {
                    case GameManager.Team.E_BLUE:
                        color = Color.blue;

                        break;
                    case GameManager.Team.E_RED:
                        color = Color.red;
                        break;
                    default:
                        color = Color.white;
                        break;
                }
                m_objMaterial.material.SetColor("_Color", color);

            }
        }
        
	}

    private void OnMouseDown()
    {
        Debug.Log("Red Team is Raiding");
        isRaid = true;
        if(TeamOwner != GameManager.Team.E_RED)
            WhoIsRaiding = GameManager.Team.E_RED;
        else if(TeamOwner != GameManager.Team.E_BLUE)
            WhoIsRaiding = GameManager.Team.E_BLUE;
    }

    private void OnMouseUp()
    {
        isRaid = false;
    }
}
