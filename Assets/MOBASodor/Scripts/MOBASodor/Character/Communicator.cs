using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communicator : MonoBehaviour {

    //For communication framework 
    public class Message
    {
        public enum CommunicationType { CHASE_ENEMY, GOTO_CHECKPOINT }

        public Message(GameObject agent, GameObject target, Message.CommunicationType type)
        {
            this.agent = agent;
            this.target = target;
            this.type = type;
        }

        public GameObject agent { get; private set; }
		public CommunicationType type { get; private set; }
		public GameObject target { get; private set; }
	}

    public void AddMessage(GameObject agent, GameObject target, Message.CommunicationType type)
    {
        Message message = new Message(agent, target, type);
		messages.Add(message);
    }

    public void DeleteMessage(GameObject agent)
    {
        Message message = messages.Find(x => x.agent == agent);

        messages.Remove(message);
    }

	public List<Message> Find(Message.CommunicationType type)
	{
		return messages.FindAll(x => x.type == type);
	}

	public List<Message> messages { get; private set; }
}
