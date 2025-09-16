using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Transform viewPoint;

    public GameObject player;

    public Camera cametra;

    public Vector3 targetPos;

    public float moveSpeed = 5f;

    public float rotateSpeed = 10f;


    private void LateUpdate()
    {
        if (player == null && User.Instance.CurrentCharacterObject != null)
        {
            player = User.Instance.CurrentCharacterObject.gameObject/*.gameObject*/;
        }

        if (player == null)
            return;

        // this.transform.position = player.transform.position;
        // this.transform.rotation = player.transform.rotation;
        this.transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * moveSpeed);
        this.transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, Time.deltaTime * rotateSpeed);

        // cametra.transform.rotation = player.transform.localRotation;
        cametra.transform.LookAt(player.transform.position + (Vector3.up * 1.8f));
        cametra.transform.localPosition = Vector3.up * 4 + Vector3.forward * -4;
    }
}
