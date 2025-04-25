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
        yield return new WaitForSeconds(1f); // 1�� ���

        other.transform.position = new Vector3(0f, 1f, 0f); // ��ġ �̵�
        Cafescene.SetActive(false);
        Scene2.SetActive(true);
;
        StartCoroutine(SmoothCameraSwitch());

    }
    private IEnumerator SmoothCameraSwitch()
    {
        // ���� ������ ���� ���� ��� ���� (ī�޶� Ʈ������ ������ ����)
        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            // �̰����� �ε巯�� ��ȯ ȿ���� ���� ���� ���� (��: ����, ī�޶� ��鸲 ��)
            // Ȥ�� Shader�� Canvas�� ȭ�� ���̵� ȿ���� �� ���� ����
            time += Time.deltaTime;
            yield return null;
        }

        // ���� ���� �� 3��Ī ��ȯ
        SpatialBridge.cameraService.forceFirstPerson = false;
    }
    private IEnumerator TransitionSkyboxExposure(Material oldMat, Material newMat)
    {
        float time = 0f;

        // ���� Skybox ������ ����
        while (time < exposureDuration)
        {
            float t = time / exposureDuration;
            float currentExposure = Mathf.Lerp(fromExposure, peakExposure, t);
            oldMat.SetFloat("_Exposure", currentExposure);
            yield return null;
            time += Time.deltaTime;
        }
        oldMat.SetFloat("_Exposure", peakExposure);

        // Skybox ����
        RenderSettings.skybox = newMat;
        newMat.SetFloat("_Exposure", peakExposure);
        time = 0f;

        // ���ο� Skybox ������ ����
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
