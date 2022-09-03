using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sentence
{
    public string name;
    public string content;
    public string effect;
}

[System.Serializable]
public class Dialogue
{
    public Sentence[] sentences;
    public int nextQuestion = -1;
}

[System.Serializable]
public struct Option
{
    public string text;
    public string effect;
}

[System.Serializable]
public struct Question
{
    public string question;
    public Option[] options;
}

[CreateAssetMenu(fileName = "new Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public Dialogue[] dialogues;
    public Question[] questions;
    public int nextDialogue = 0;
}
