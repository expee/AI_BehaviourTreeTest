using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager _this;

    public enum Team {E_NONE, E_BLUE, E_RED, E_TEAM_COUNT}
    public enum GameState { IDLE, STARTED, FINISH}

    public List<GameObject> Towers;
    public GameState State;

    private GameState t_state;
    private UnityEvent m_pauseEvent;
    private Team? m_winTeam = null;

    private void Awake()
    {
        _this = this;
        State = GameState.IDLE;
    }
    // Use this for initialization
    void Start () {
        State = GameState.STARTED;	
	}
	
	// Update is called once per frame
	void Update () {
        StateMachineUpdate();

    }

    void StateMachineUpdate()
    {
        if (t_state == State)
            return;

        t_state = State;
        switch(State)
        {
            case GameState.IDLE:
                
                break;
            case GameState.STARTED:
                break;
            case GameState.FINISH:
                SceneManager.LoadScene("MOBASodor");

                break;
        }
    }

    public static GameManager GetInstance()
    {
        return _this;
    }

    public void SetWin(Team team)
    {
        m_winTeam = team;
        Debug.Log("Winner -> " + m_winTeam);
        State = GameState.FINISH;
    }

    public UnityEvent PauseEvent
    {
        get
        {
            if (m_pauseEvent == null)
                m_pauseEvent = new UnityEvent();

            return m_pauseEvent;
        }
    }
}
