using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICharacterView : MonoBehaviour
{

    public GameObject[] characters;
    //public Animator[] animator;
    public Transform oldRotate;
    //public
    //CharacterSellectViewRotate[] CharacterSellectViewRotate;
    public CharacterSellectViewRotate CharacterRotate;
    private int currentCharacter = 0;



    public int CurrectCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            CharacterRotate.current = currentCharacter;
            this.UpdateCharacter();
        }
    }



    // Use this for initialization
    void Start ()
    {
        //CharacterRotate = CharacterSellectViewRotate[currentCharacter];
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateCharacter()
    {
        for(int i=0;i<3;i++)
        {
            characters[i].SetActive(i == this.currentCharacter);
   
        }
        if (characters[currentCharacter].transform.rotation != oldRotate.rotation)////旋转角度重置
        {
            characters[currentCharacter].transform.rotation = oldRotate.rotation;
           // Debug.Log();
            //CharacterRotate = CharacterSellectViewRotate[currentCharacter];
        }
    }
}
