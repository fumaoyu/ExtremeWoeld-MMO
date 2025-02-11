using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Collider miniBoundBox;

    // Start is called before the first frame update
    void Start()
    {
        MiniMapManager.Instance.UpdateMiniMap(miniBoundBox);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
