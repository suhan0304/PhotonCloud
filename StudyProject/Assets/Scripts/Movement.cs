using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.IO.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Analytics;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;

    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    private PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    public float moveSpeed = 10.0f;

    void Start() {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        // PhotonView 컴포넌트가 자신의 것일 경우 시네머신 가상 카메라를 연결
        if (pv.IsMine) {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        plane = new Plane(transform.up, transform.position);
    }

    void Update() {
        // 자신이 생성한 네트워크 객체만 컨트롤
        if (pv.IsMine) {
            Move();
            Turn();
        }
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    void Move() {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;

        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        controller.SimpleMove(moveDir * moveSpeed);

        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn() {
        ray = camera.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;

        plane.Raycast(ray, out enter);
        hitPoint = ray.GetPoint(enter);

        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        transform.localRotation = Quaternion.LookRotation(lookDir);

    }
}
