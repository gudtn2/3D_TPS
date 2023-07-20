using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ** 카메라 조작 시네머신 **
using Cinemachine;
// ** 키 입력받는 스타터 에셋
using StarterAssets;
using UnityEngine.InputSystem;

public class TPSController : MonoBehaviour
{
    // 조준했을 때 전환할 카메라
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    // 비조준 마우스 감도
    [SerializeField] private float normalSensitivity;
    // 조준 마우스 감도
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
        // 마우스 위치 초기화
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        // 레이캐스트가 지정한 Layer에 충돌하면
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        // aim키를 입력받으면
        if (starterAssetsInputs.aim && !starterAssetsInputs.sprint)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            // 조준 마우스감도 설정
            thirdPersonController.SetSensitivity(aimSensitivity);
            // 조준하면 캐릭터 회전안함
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
