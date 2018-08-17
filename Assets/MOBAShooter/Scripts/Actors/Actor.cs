using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public enum Allegiance
    {
        RED,
        BLUE,
        ALLEGIANCE_COUNT
    }

    public enum State
    {
        ALIVE,
        DEAD,
        STATE_COUNT
    }
    public Allegiance allegiance;

    public State state { get; protected set; }
}
