using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public Text NameText;
    public Image IconAvater;

    public Character character;
    // Start is called before the first frame update
    void Start()
    {
       if (character != null)
        {
            this.UpdateInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateInfo ();
        this.transform.forward =Camera.main.transform.forward;
    }

    void UpdateInfo()
    {
        if (character != null)
        {
            string name = this.character.Name + " Lv: " + this.character.Info.Level;
            if (name != this.NameText.text)//这样做是为了性能，每一次赋值都会影响性能
            {
                this.NameText .text  = name;
            }
        }
    }
}
