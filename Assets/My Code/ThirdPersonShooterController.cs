using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TerceraPersonaDisparadorController : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("La camara para apuntar")]
    public CinemachineVirtualCamera aimCamera;
    public int prioridadApuntando = 5;

    public LayerMask aimColliderMask = new LayerMask();

    public Animator animator;
    public StarterAssetsInputs input;
    public GameObject crosshair;

    public Transform aimTarget;

    [Header("Disparo")]


    public Rig aimRig;

    private int aimLayerIndex;

    // Variable de control para disparo único
    private bool canShoot = true;

    private void Start()
    {
        aimLayerIndex = animator.GetLayerIndex("Aim");
    }

    private void Update()
    {
        if (input.aim)
            Aiming();
        else
            NoAiming();

        // Disparo solo si el jugador está apuntando
        if (input.aim && input.shoot && canShoot)
        {
            canShoot = false; // Evitar que dispare continuamente
        }

        // Si se suelta el botón de disparo, permite disparar de nuevo
        if (!input.shoot)
        {
            canShoot = true;
        }
    }

    private void Aiming()
    {
        aimCamera.Priority = prioridadApuntando;

        crosshair.SetActive(true);
        aimTarget.gameObject.SetActive(true);

        var weight = Mathf.Lerp(aimRig.weight, 1f, Time.deltaTime * 10f);
        aimRig.weight = weight;
        animator.SetLayerWeight(aimLayerIndex, weight);

        MoveAimTarget();
    }

    private void NoAiming()
    {
        aimCamera.Priority = 0;

        crosshair.SetActive(false);
        aimTarget.gameObject.SetActive(false);

        var weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 10f);
        aimRig.weight = weight;
        animator.SetLayerWeight(aimLayerIndex, weight);
    }

    private void MoveAimTarget()
    {
        // Aim
        var mouseWorldPosition = Vector3.zero;

        var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
        {
            // Muevo el objetivo para que la pistola lo mire
            aimTarget.position = hit.point;
            mouseWorldPosition = hit.point;
        }

        // Hago que el jugador mire en la dirección del mouse en el mundo
        var aimDirection = (mouseWorldPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 1f);
    }
}
