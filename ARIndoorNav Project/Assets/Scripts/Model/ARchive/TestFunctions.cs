using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<string> testList = new List<string>();
        
        testList.Add("3.219\n"); 
        testList.Add("Dr.BenjaminMeyer\n");
        testList.Add("FlorianHerborn\n");
        testList.Add("SaschaLemke\nMarioFaske\nLenaWirtz\nMarkusAlterauge");
        testList.Add("2.411\n");
        testList.Add("3 .211");
        testList.Add("3. 212");
        testList.Add(" 3.213");
        testList.Add(" 3 . 2 1 4 ");
        testList.Add(" ");
        testList.Add("1");
        testList.Add("");
        testList.Add("3.215b");
        /**
         * Result
         * 3.219
         * 2.411
         * 3.211
         * 3.212
         * 3.213
         * 3.214
         * 1
         * 3.215b
         */

        ContainsRoom(testList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Room> ContainsRoom(List<string> potentialMarkerList)
    {
        List<Room> result = new List<Room>();
        foreach (string text in potentialMarkerList)
        {
            bool containsNumber = false;
            foreach (char character in text)
            {
                if (Char.IsDigit(character))
                    containsNumber = true;
            }
            if (containsNumber)
                Debug.Log(text.Replace(" ", ""));
        }

        return result;
    }
}
