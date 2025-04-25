using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

public class Example : MonoBehaviour
{
    public GameObject Cafescene;
    public GameObject Scene2;
    public GameObject teleport;
    public Vector3 teleportPosition = Vector3.zero;
    public Material newSkyboxMaterial;
    public float exposureDuration = 1.0f;
    public float fromExposure = 1.5f;
    public float peakExposure = 8.0f;

    private void Start()
    {
        Cafescene.SetActive(true);
        Scene2.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (newSkyboxMaterial != null)
        {

            StartCoroutine(TransitionSkyboxExposure(RenderSettings.skybox, newSkyboxMaterial));
            StartCoroutine(MoveAfterDelay(other));
        }
        
    }
    private IEnumerator MoveAfterDelay(Collider other)
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        other.transform.position = new Vector3(0f, 1f, 0f); // 위치 이동
        Cafescene.SetActive(false);
        Scene2.SetActive(true);
;
        StartCoroutine(SmoothCameraSwitch());

    }
    private IEnumerator SmoothCameraSwitch()
    {
        // 강제 시점을 끄기 전에 잠깐 지연 (카메라 트랜지션 연출을 위해)
        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            // 이곳에서 부드러운 전환 효과를 위한 연출 가능 (예: 블렌딩, 카메라 흔들림 등)
            // 혹은 Shader나 Canvas로 화면 페이드 효과를 줄 수도 있음
            time += Time.deltaTime;
            yield return null;
        }

        // 시점 해제 → 3인칭 전환
        SpatialBridge.cameraService.forceFirstPerson = false;
    }
    private IEnumerator TransitionSkyboxExposure(Material oldMat, Material newMat)
    {
        float time = 0f;

        // 기존 Skybox 노출을 증가
        while (time < exposureDuration)
        {
            float t = time / exposureDuration;
            float currentExposure = Mathf.Lerp(fromExposure, peakExposure, t);
            oldMat.SetFloat("_Exposure", currentExposure);
            yield return null;
            time += Time.deltaTime;
        }
        oldMat.SetFloat("_Exposure", peakExposure);

        // Skybox 변경
        RenderSettings.skybox = newMat;
        newMat.SetFloat("_Exposure", peakExposure);
        time = 0f;

        // 새로운 Skybox 노출을 감소
        while (time < exposureDuration)
        {
            float t = time / exposureDuration;
            float currentExposure = Mathf.Lerp(peakExposure, fromExposure, t);
            newMat.SetFloat("_Exposure", currentExposure);
            yield return null;
            time += Time.deltaTime;
        }
        teleport.SetActive(false);

        newMat.SetFloat("_Exposure", fromExposure);
    }

}
