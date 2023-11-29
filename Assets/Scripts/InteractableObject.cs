using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //[SerializeField] TextAsset dialogueAsset;
    [SerializeField] Sprite[] sprites;
    //[SerializeField] Sprite defaultPortrait = null;
    //[SerializeField] Sprite secondPortrait = null;
    //[SerializeField] Sprite thirdPortrait = null;
    //private List<Sprite> imageList;
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
        //imageList = new List<Sprite>();
        //imageList.Add(defaultPortrait);
        //imageList.Add(secondPortrait);
        //imageList.Add(thirdPortrait);
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
