using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MannaDisplay : MonoBehaviour
{
    TextMeshProUGUI textBox;

    private void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();
    }

    public void OnMannaChange(Component sender, object data)
    {
        string textToDisplay = data.ToString();
        textBox.text = textToDisplay;
    }
}
