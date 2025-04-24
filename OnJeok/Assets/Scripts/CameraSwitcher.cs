using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject firstPersonCamObject;

    private Transform headTransform;
    private Vector3 velocity = Vector3.zero;

    public float smoothTime = 0.1f;
    public float rotationSmoothSpeed = 10f;
    public Vector3 positionOffset = new Vector3(0, 0.05f, 0.1f);

    void Start()
    {
        StartCoroutine(InitCameraWhenReady());
    }

    IEnumerator InitCameraWhenReady()
    {
        while (SpatialBridge.actorService.localActor == null ||
               SpatialBridge.actorService.localActor.avatar == null ||
               SpatialBridge.actorService.localActor.avatar.GetAvatarBoneTransform(HumanBodyBones.Head) == null)
        {
            yield return null;
        }

        headTransform = SpatialBridge.actorService.localActor.avatar.GetAvatarBoneTransform(HumanBodyBones.Head);

        SpatialBridge.cameraService.forceFirstPerson = true;
        firstPersonCamObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (!SpatialBridge.cameraService.forceFirstPerson)
            return;

        if (headTransform == null || !headTransform.gameObject.activeInHierarchy)
        {
            var actor = SpatialBridge.actorService.localActor;
            if (actor != null && actor.avatar != null)
            {
                headTransform = actor.avatar.GetAvatarBoneTransform(HumanBodyBones.Head);
            }

            if (headTransform == null)
            {
                if (firstPersonCamObject.activeSelf)
                    firstPersonCamObject.SetActive(false);
                return;
            }
        }

        if (!firstPersonCamObject.activeSelf)
            firstPersonCamObject.SetActive(true);

        Vector3 targetPosition = headTransform.position + positionOffset;


        if (Vector3.Distance(firstPersonCamObject.transform.position, targetPosition) > 1.5f)
        {
            firstPersonCamObject.transform.position = targetPosition;
        }
        else
        {
            firstPersonCamObject.transform.position = Vector3.SmoothDamp(
                firstPersonCamObject.transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );
        }

        // 회전은 그대로
        Quaternion targetRotation = headTransform.rotation;
        firstPersonCamObject.transform.rotation = Quaternion.Slerp(
            firstPersonCamObject.transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothSpeed
        );
    }

    public void SwitchToThirdPerson()
    {
        SpatialBridge.cameraService.forceFirstPerson = false;
        firstPersonCamObject.SetActive(false);
    }
}

