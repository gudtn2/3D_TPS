using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ** ī�޶� ���� �ó׸ӽ� **
using Cinemachine;
// ** Ű �Է¹޴� ��Ÿ�� ����
using StarterAssets;
using UnityEngine.InputSystem;

public class TPSController : MonoBehaviour
{
    // �������� �� ��ȯ�� ī�޶�
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    // ������ ���콺 ����
    [SerializeField] private float normalSensitivity;
    // ���� ���콺 ����
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    private void Update()
    {
        // ���콺 ��ġ �ʱ�ȭ
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        // ����ĳ��Ʈ�� ������ Layer�� �浹�ϸ�
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        // aimŰ�� �Է¹�����
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            // ���� ���콺���� ����
            thirdPersonController.SetSensitivity(aimSensitivity);
            // �����ϸ� ĳ���� ȸ������
            thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
        }


    }
}
