using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] TextAsset[] dialogueAssets;
    [SerializeField] public string objectName;
    
    public enum ObjectType
    {
        Person,
        Book
    }

    public ObjectType objectType;

    public void Start()
    {
    }

    public string ReturnLinesOfText(int textToReturn)
    {
        return dialogueAssets[textToReturn].ToString();
    }
    public Sprite[] ReturnSpriteList()
    {
        return sprites;
    }
}
