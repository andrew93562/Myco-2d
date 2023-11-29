using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplayBox;
    [SerializeField] GameObject characterPortraitObject;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] GameEvent ExitInteraction;
    [SerializeField] QuestManager questManager;
    //[SerializeField] GameObject npcPortraitObject;
    public float textSpeed = 0.1f;

    [Header("Character Portraits")]
    [SerializeField] Sprite[] characterSprites;
    //[SerializeField] Sprite defaultCharacterPortrait = null;
    //[SerializeField] Sprite secondCharacterPortrait = null;
    //[SerializeField] Sprite thirdCharacterPortrait = null;

    
    private Image characterPortraitSlot;
    //private Image npcPortraitSlot;
    //private List<Sprite> characterPortraits = new List<Sprite>();
    private List<string> textToDisplay = new List<string>();
    private Sprite[] npcPictureList;
    private int textToPass = 0;

    private int index = 0;
    private InteractableObject interactableObject = null;
    //private TextMeshProUGUI textDisplayBox;

    private enum InteractionState
    {
        initialState,
        listening,
        displaying,
        endOfDialogue
    }

    private InteractionState currentInteractionState;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this);
        //textDisplayBox = dialogueBox.GetComponent<TextMeshProUGUI>();
        characterPortraitSlot = characterPortraitObject.GetComponent<Image>();
        //npcPortraitSlot = npcPortraitObject.GetComponent<Image>();
        //consider building the list below only when entering an interaction, and then destroying it upon exiting the interaction
        // or, since they're already in memory, just do an if-statement in the text parsing method. this way, you can just have the 
        // portrait cues in the text file be read as a string, and have it match up with a string
        //characterPortraits.Add(defaultCharacterPortrait);
        //characterPortraits.Add(secondCharacterPortrait);
        //characterPortraits.Add(thirdCharacterPortrait);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnterInteraction(Component sender, object data)
    {
        interactableObject = (InteractableObject)data;
        //Debug.Log
        index = 0;
        SwitchState(InteractionState.listening);
    }

    public void OnExitInteraction(Component sender, object data)
    {
        SwitchState(InteractionState.initialState);
    }

    public void OnTriggerInteraction(Component sender, object data)
    {
        switch (currentInteractionState)
        {
            case InteractionState.initialState:
                return;

            case InteractionState.listening:
                SwitchState(InteractionState.displaying);
                return;

            case InteractionState.displaying:
                NextLine();
                //if no lines left, switch case to end of dialogue
                return;

            case InteractionState.endOfDialogue:
                //ExitInteraction.Raise(this, null);
                //Debug.Log("Exit interaction triggered from Trigger Interaction in End of Dialogue");
                return;
        }
    }

    private void SwitchState(InteractionState newState)
    {
        switch (newState)
        {
            case InteractionState.initialState:
                currentInteractionState = InteractionState.initialState;
                textToDisplay = new List<string>();
                npcPictureList = new Sprite[0];
                characterPortraitSlot.sprite = null;
                textDisplayBox.text = string.Empty;
                characterPortraitObject.SetActive(false);
                dialogueBox.SetActive(false);
                interactableObject = null;
                textToPass = 0;
                return;

            case InteractionState.listening:
                currentInteractionState = InteractionState.listening;
                textToPass = questManager.ReturnTextNumber(interactableObject);
                textToDisplay = DialogueParser(interactableObject.ReturnLinesOfText(textToPass));
                npcPictureList = interactableObject.ReturnSpriteList();
                return;

            case InteractionState.displaying:
                currentInteractionState = InteractionState.displaying;
                characterPortraitObject.SetActive(true);
                dialogueBox.SetActive(true);
                //StartCoroutine(TypeLine());
                NextLine();
                return;

            case InteractionState.endOfDialogue:
                currentInteractionState = InteractionState.endOfDialogue;
                ExitInteraction.Raise(this, null);
                return;

        }
    }
    /*
    public void DisplayDialogue(Component sender, object data)
    {
        interactableObject = (InteractableObject)data;
        List<string> textToDisplay = DialogueParser(interactableObject.ReturnLinesOfText());
        List<Sprite> characterPortraits = interactableObject.ReturnSpriteList();

        //string[] texttodisplay = dialogue script[0]
        //for line in texttodisplay, 
        //if line starts with $, then interpret the rest as the number portrait that should be displayed
        // from the dialoguescript[1] array, or the MC portrait
        Debug.Log(textToDisplay);
    }
    */

    private List<string> DialogueParser(string text)
    {
        List<string> result = text.Split("\n").ToList<string>();
        return result;
    }

    IEnumerator TypeLine()
    {
        foreach (char c in textToDisplay[index].ToCharArray())
        {
            textDisplayBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index  < textToDisplay.Count - 1)
        {
            StopAllCoroutines();
            // if the line starts with a $ or #, DisplayPicture(), index++, return
            // $ = 0 = playercharacter
            // # = 1 = npc
            if (textToDisplay[index][0] == '$')
            {
                int lineLength = textToDisplay[index].Length - 1;
                //Debug.Log(textToDisplay[index][1..]);
                int pictureToDisplay = int.Parse(textToDisplay[index][1..]);
                DisplayPicture(0, pictureToDisplay);
                index++;
            }
            if (textToDisplay[index][0] == '#')
            {
                int lineLength = textToDisplay[index].Length - 1;
                //Debug.Log(textToDisplay[index][1..]);
                int pictureToDisplay = int.Parse(textToDisplay[index][1..]);
                DisplayPicture(1, pictureToDisplay);
                index++;
            }
            textDisplayBox.text = string.Empty;
            StartCoroutine(TypeLine());
            index++;
            //textDisplayBox.text = textToDisplay[index];
        }
        else
        {
            SwitchState(InteractionState.endOfDialogue);
        }
    }

    void DisplayPicture(int whichCharacter, int pictureToDisplay)
    {
        if (whichCharacter == 0)
        {
            characterPortraitSlot.sprite = characterSprites[pictureToDisplay];
        }
        else if (whichCharacter == 1)
        {
            characterPortraitSlot.sprite = npcPictureList[pictureToDisplay];
        }
    }
}
