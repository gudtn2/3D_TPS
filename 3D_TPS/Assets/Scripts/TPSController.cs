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
    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform spawnBulletPosition;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
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
        if (starterAssetsInputs.aim && !starterAssetsInputs.sprint)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            // ���� ���콺���� ����
            thirdPersonController.SetSensitivity(aimSensitivity);
            // �����ϸ� ĳ���� ȸ������
            thirdPersonController.SetRotateOnMove(false);
            animator.SetBool("Aiming", true);
            //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1),1f, Time.deltaTime * 10f));

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
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            animator.SetBool("Aiming", false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        if (starterAssetsInputs.dodge)
        {
            animator.SetTrigger("Roll");
        }

        if (starterAssetsInputs.shoot)
        {
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(bulletPrefab, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }
    }
}
