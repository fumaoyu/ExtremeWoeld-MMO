using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour
{

    public Transform owner;//属于谁的

    public float height = 1.5f;//高度
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (owner != null)
        {
            this.transform.position = owner.position+Vector3.up*height;   //自动跟踪
        }


    }
}
