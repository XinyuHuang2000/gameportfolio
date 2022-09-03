using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    protected GameObject player;
    public Conversation conversation;
    public bool canZoomIn = false;
    protected bool inConversation = false; //track if the player is currently interacting with the object
    protected bool inZoomIn = false;
    protected int nextDialogue = 0;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public virtual void TriggerDialogue()
    {
        //if player is not currently interacting with the object, then start interaction
        if (!inConversation && conversation != null && nextDialogue >= 0)
        {
            inConversation = true;
            FindObjectOfType<DialogueManager>().StartDialogue(conversation.dialogues[nextDialogue], this);
            if(player != null) player.GetComponent<Player>().SetFrozen(true);
        }
        
    }
    public virtual void TriggerDialogue(int index)
    {
        inConversation = true;
        FindObjectOfType<DialogueManager>().StartDialogue(conversation.dialogues[index], this);
        if (player != null) player.GetComponent<Player>().SetFrozen(true);



    }
    public virtual void TriggerZoomIn()
    {
        if (!inZoomIn)
        {
            GameObject zoomIn = gameObject.transform.GetChild(0).gameObject;
            FindObjectOfType<ZoomInManager>().AddzoomIn(zoomIn);
            zoomIn.SetActive(true);
            if(player != null) player.GetComponent<Player>().SetFrozen(true);
            inZoomIn = true;
        }
       
    }

    //allow dialogue manager to close interaction
    public virtual void SetInConversation(bool b)
    {
        if (!(b || inZoomIn))
        {
            StartCoroutine(EndDialogue());
        }
        else
        {
            inConversation = b;
        }
    }

    protected IEnumerator EndDialogue()
    {
        if(player != null) player.GetComponent<Player>().SetFrozen(false);
        yield return new WaitForEndOfFrame();
        inConversation = false;
    } 
    public void SetInZoomIn(bool b)
    {
        inZoomIn = b;
        if (!(inConversation || inZoomIn))
        {
            player.GetComponent<Player>().SetFrozen(false);
        }
    }

    public bool CanInteract()
    {
        return !(inZoomIn || inConversation);
    }

    public Question GetQuestion(int index)
    {
        //if the request question does not exist
        if(index > conversation.questions.Length)
        {
            Debug.LogError("Question index out of bundary!");
        }

        return conversation.questions[index];
    }

    public void ChangeDialogue(int index)
    {
        nextDialogue = index;
    }
    public void ChangeDialogueNow(int index)
    {
        nextDialogue = index;
        inConversation = true;
        FindObjectOfType<DialogueManager>().StartDialogue(conversation.dialogues[nextDialogue], this);
        if (player != null) player.GetComponent<Player>().SetFrozen(true);
    }
}
