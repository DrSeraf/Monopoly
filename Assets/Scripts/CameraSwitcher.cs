using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher instance;
    [SerializeField] CinemachineVirtualCamera topDownCamera;
    [SerializeField] CinemachineVirtualCamera diceCamera;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;

    private void Awake()
    {
        instance = this;
    }

    public void SwitchToTopDowd()
    {
        topDownCamera.Priority = 2;
        diceCamera.Priority = 0;    
        playerFollowCamera.Priority = 0;
    }

    public void SwitchToDiceCamera()
    {
        topDownCamera.Priority = 0;
        diceCamera.Priority = 2;
        playerFollowCamera.Priority = 0;
    }

    public void SwitchToPlayer(Transform followTarger)
    {
        topDownCamera.Priority = 0;
        diceCamera.Priority = 0;
        playerFollowCamera.Priority = 2;
        playerFollowCamera.Follow = followTarger;
        playerFollowCamera.LookAt = followTarger;
    }
}
