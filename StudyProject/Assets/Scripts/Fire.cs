using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    private ParticleSystem muzzleFlash;

    private PhotonView pv;
    // 왼쪽 마우스 버튼 클릭 이벤트 저장
    private bool isMouseClick => Input.GetMouseButtonDown(0);

    void Start() {
        pv = GetComponent<PhotonView>();
        muzzleFlash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }    

    void Update() {
        // 로컬 유저 여부와 마우스 왼쪽 버튼을 클릭했을 때 총알 발사
        if (pv.IsMine && isMouseClick) {
            FireBullet(pv.Owner.ActorNumber);
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    void FireBullet(int acntorNo)  {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = acntorNo;
    }
}
