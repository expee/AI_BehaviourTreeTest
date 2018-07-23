using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communicator : MonoBehaviour {

    //For communication framework 
    public class Message
    {
        public enum CommunicationType { CHASE_ENEMY, GOTO_CHECKPOINT }
        GameObject agent;
        CommunicationType type;
        GameObject target;

        public Message(GameObject agent, GameObject target, Message.CommunicationType type)
        {
            this.agent = agent;
            this.target = target;
            this.type = type;
        }

        public GameObject Agent
        {
            get { return agent; }
        }
    }

    public List<Message> Messages;

    public void AddMessage(GameObject agent, GameObject target, Message.CommunicationType type)
    {
        Message message = new Message(agent, target, type);
    }

    public void DeleteMessage(GameObject agent)
    {
        Message message = Messages.Find(x => x.Agent == agent);

        Messages.Remove(message);
    }
}
