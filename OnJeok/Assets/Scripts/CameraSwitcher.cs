using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    private bool isFirstPerson = true;

    void Start()
    {
        // ���� ���� �� 1��Ī ���� ����
        SpatialBridge.cameraService.forceFirstPerson = true;
    }

}

