using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    private bool isFirstPerson = true;

    void Start()
    {
        // 게임 시작 시 1인칭 모드로 강제
        SpatialBridge.cameraService.forceFirstPerson = true;
    }

}

