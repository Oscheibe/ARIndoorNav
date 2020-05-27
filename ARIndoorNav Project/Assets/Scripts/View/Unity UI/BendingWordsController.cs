using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BendingWordsController : MonoBehaviour
{
    public TMP_Text firstWord1_TMP;
    public TMP_Text firstWord2_TMP;
    public TMP_Text secondWord1_TMP;
    public TMP_Text secondWord2_TMP;

    private string firstWord;
    private string secondWord;

    /**
     * The bending words AR visuals is made out of 4 TMP texts
     * There are always two opposing texts that are each invisible when viewed from the back
     * This allows readability from every direction.
     */
    public void UpdateWords(string firstWord, string secondWord)
    {
        this.firstWord = firstWord;
        this.secondWord = secondWord;

        firstWord1_TMP.text = firstWord;
        firstWord2_TMP.text = firstWord;

        secondWord1_TMP.text = secondWord;
        secondWord2_TMP.text = secondWord;
    }
}
