using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private enum MainQuestState
    {
        HasntStartedYet,
        TalkedToJeremiah
    }

    private MainQuestState mainQuestState = MainQuestState.HasntStartedYet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int ReturnTextNumber(DialogueObject interactableObject)
    {
        if (interactableObject.objectType == DialogueObject.ObjectType.Book)
        {
            return 0;
        }
        else if (interactableObject.objectType == DialogueObject.ObjectType.Person)
        {
            if (interactableObject.objectName == "Jeremiah")
            {
                if (mainQuestState == MainQuestState.HasntStartedYet)
                {
                    mainQuestState = MainQuestState.TalkedToJeremiah;
                    return 0;
                }
                else if (mainQuestState == MainQuestState.TalkedToJeremiah)
                {
                    return 1;
                }
                else { return 0; }
            }
            else { return 0; }
        }
        else { return 0; }
    }
}
