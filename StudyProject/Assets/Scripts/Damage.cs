using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour {
    private Renderer[] renderers;

    private int initHp = 100;
    private int currHp = 100;

    private Animator anim;
    private CharacterController cc;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");

    void Awake() {
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        currHp = initHp;
    }

    void OnCollisionEnter(Collision coll) {
        if (currHp > 0 && coll.collider.CompareTag("BULLET")) {
            currHp -= 20;
            if (currHp <= 0) {
                StartCoroutine(PlayerDie());
            }
        }
    }

    
}