using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    //reference to dialogue UI
    public Text nameText;
    public Text dialogueText;
    public Image choiceBox;
    public Text question;
    public Text[] options;

    //dialogue queue
    private Queue<Sentence> sentences;
    //the interaction trigger of the object that the player is currently interact with
    private InteractionTrigger currentIT;
    private bool inConversation = false;
    private bool inChoice = false;
    private int nextQuestion = -1;
    private Coroutine currentCor = null;
    private Question choice;
    private SceneSwitcher sceneSwitcher;
    public GameObject[] instructions;
    private int instrIndex = 0;
    private bool inSentence = false;
    private string currentSentence;
    void Awake()
    {
        //initialize dialogue queue
        sentences = new Queue<Sentence>();
        instructions[instrIndex].SetActive(true);
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
    }

    private void Update()
    {
        //if "SPACE" is pressed, display next sentence in the queue
        if (inConversation && !inChoice && (Input.GetKeyDown(KeyCode.Space)))
        {
            if (inSentence)
            {
                StopCoroutine(currentCor);
                dialogueText.text = currentSentence;
                inSentence = false;
            }
            else
            {
                DisplayNextSentence();
            }

            //DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue, InteractionTrigger it)
    {
        
        sentences.Clear(); //clear all remaining dialogue
        currentIT = it; //update current dialogue trigger

        //set up next question number
        nextQuestion = dialogue.nextQuestion;
        //add each sentence to the dialogue queue
        foreach(Sentence s in dialogue.sentences)
        {
            sentences.Enqueue(s);
        }
        StartCoroutine(StartSentence());
    }
    IEnumerator StartSentence()
    {
        DisplayNextSentence();
        yield return new WaitForEndOfFrame();
        inConversation = true;

    }

    public void DisplayNextSentence()
    {
        if (currentCor != null)
        {
            StopCoroutine(currentCor);
        }
        //if the queue is empty, end the dialogue
        if (sentences.Count == 0)
        {
            //first check if there is a following question/choice to this dialogue
            if(nextQuestion > -1)
            {
                DisplayChoice();

                return;
            }
            EndDialogue();
            return;
        }
        //else, pop the first sentence and display
        Sentence s = sentences.Dequeue();
        nameText.text = s.name;
        currentSentence = s.content;
        currentCor = StartCoroutine(TypeSentense(currentSentence));
        if(s.effect != null && s.effect.Length > 0)
        {
            string[] effects = s.effect.Split(';');
            foreach (string e in effects)
            {
                TakeEffect(e);
            }
        }
    }

    IEnumerator TypeSentense(string s)
    {
        inSentence = true;
        dialogueText.text = "";
        foreach(char letter in s.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        inSentence = false;
    }

    public void EndDialogue()
    {
        //end the dialogue and inform the dialogue trigger
        if (inConversation)
        {
            dialogueText.text = "";
            nameText.text = "";
            sentences.Clear();
            currentIT.SetInConversation(false);
            inConversation = false;
        }


    }


    private void DisplayChoice()
    {
        inChoice = true;
        choice = currentIT.GetQuestion(nextQuestion);

        //envisible choice box ui
        choiceBox.gameObject.SetActive(true);
        //set up question text
        question.text = choice.question;
        //set up option text
        for(int i = 0; i < choice.options.Length; ++i)
        {
            options[i].text = choice.options[i].text;
        }
    }

    private void DisplayChoice(int index)
    {
        inChoice = true;
        choice = currentIT.GetQuestion(index);
        //envisible choice box ui
        choiceBox.gameObject.SetActive(true);
        //set up question text
        question.text = choice.question;
        //set up option text
        for (int i = 0; i < choice.options.Length; ++i)
        {
            options[i].text = choice.options[i].text;
        }
    }

    private void CloseChoice()
    {
        for (int i = 0; i < choice.options.Length; ++i)
        {
            options[i].text = "";
        }
        question.text = "";
        choiceBox.gameObject.SetActive(false);
        inChoice = false;
        EndDialogue();
    }

    private void JustCloseChoice()
    {
        for (int i = 0; i < choice.options.Length; ++i)
        {
            options[i].text = "";
        }
        question.text = "";
        choiceBox.gameObject.SetActive(false);
        inChoice = false;
    }

    public void ChooseOption(int index)
    {
        //if invalid option index
        if (index >= choice.options.Length)
        {
            return;
        }
        

        string effect = choice.options[index].effect;
        Debug.Log("choosed option" + index + ", effect: " + effect);
        //TO DO: options take effect
        if(effect.Length > 0)
        {
            string[] effects = effect.Split(';');
            foreach (string s in effects)
            {
                TakeEffect(s);
                string s0 = s.Split(',')[0];
                if (s0.Equals("DialogueNow"))
                {
                    JustCloseChoice();
                    return;
                }else if (s0.Equals("Question"))
                {
                    return;
                }
            }
        }
        CloseChoice();
        
    }
    
    private void TakeEffect(string s)
    {
        string[] effect = s.Split(',');
        string target = effect[0].Trim(' ');
        string command = effect[1].Trim(' ');
        switch (target)
        {
            
            case "Dialogue":
                currentIT.ChangeDialogue(int.Parse(command));
                break;
            case "DialogueNow":
                currentIT.ChangeDialogueNow(int.Parse(command));
                break;
            case "Question":
                DisplayChoice(int.Parse(command));
                break;
            case "Object":
                currentIT.gameObject.GetComponent<Interactable>().OptionTakeEffect(command);
                break;
            case "Scene":
                sceneSwitcher.ChangeScene(int.Parse(command));
                break;
            case "Blood":
                PlayableDirector blood = GameObject.Find("blood").GetComponent<PlayableDirector>();
                blood.Play();
                break;
            case "CG":
                int i = int.Parse(command);
                if (i < 0) FindObjectOfType<CGManager>().ChangeCG();
                else FindObjectOfType<CGManager>().ChangeCG(i);
                break;
            case "bgm":
                AudioManager.instance.PlayBgm(command);
                break;
            case "sfx":
                AudioManager.instance.Play(command);
                break;
            default:
                Debug.LogWarning("Invalid Option Effect :" + target + " | " + command +".");
                break;
        }

    }
}
