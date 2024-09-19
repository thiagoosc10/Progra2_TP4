using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class WeaponPickup : MonoBehaviour
{
    public float pickupRange = 4f;
    public string weaponName = "Pistola";
    public Vector3 handOffset = Vector3.zero;
    public Vector3 handRotation = Vector3.zero;
    
    private StarterAssetsInputs playerInput;
    private GameObject player;
    private Transform playerHand;
    private TextMeshProUGUI pickupText;

    // Sistema de munición
    public int maxAmmo = 12; // Máxima cantidad de balas en el cartucho
    public int currentAmmo;  // Munición actual
    public int ammoReserve = 24; // Munición en reserva para recargar
    public float reloadTime = 2f;
    private bool isReloading = false;

    public GameObject bulletPrefab;
    public Transform shootPoint; // El punto desde donde se disparan las balas

    private TextMeshProUGUI ammoText;

    // Tiempo entre disparos
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    // Estado de apuntar
    public bool isAiming = false; 

    private void Start()
    {
        playerInput = FindObjectOfType<StarterAssetsInputs>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHand = FindPlayerHand();
        pickupText = GameObject.Find("PickupText")?.GetComponent<TextMeshProUGUI>();
        ammoText = GameObject.Find("AmmoText")?.GetComponent<TextMeshProUGUI>(); // Mostrar munición

        if (playerInput == null) Debug.LogError("No se encontró StarterAssetsInputs");
        if (player == null) Debug.LogError("No se encontró el jugador");
        if (playerHand == null) Debug.LogError("No se encontró RightHandAttach");
        if (pickupText == null) Debug.LogError("No se encontró PickupText");
        if (ammoText == null) Debug.LogError("No se encontró AmmoText");
        if (bulletPrefab == null) Debug.LogError("No se asignó un prefab de bala");
        if (shootPoint == null) Debug.LogError("No se asignó un punto de disparo para la pistola");

        if (pickupText != null) pickupText.gameObject.SetActive(false);

        currentAmmo = maxAmmo; // Inicializamos la munición con la máxima
        UpdateAmmoUI();
    }

    private Transform FindPlayerHand()
    {
        if (player == null) return null;

        // Busca RightHandAttach en toda la jerarquía del jugador
       Transform handAttach = FindChildRecursively(player.transform, "mixamorig7:RightHand/RightHandAttach");

        
        if (handAttach == null)
        {
            Debug.LogError("No se pudo encontrar RightHandAttach. Buscando mixamorig7:RightHand como alternativa.");
            handAttach = FindChildRecursively(player.transform, "mixamorig7:RightHand");
        }

        if (handAttach == null)
        {
            Debug.LogError("No se pudo encontrar ni RightHandAttach ni mixamorig7:RightHand. Verifica la jerarquía del esqueleto.");
        }
        else
        {
            Debug.Log($"Se encontró el punto de anclaje para el arma: {handAttach.name}");
        }

        return handAttach;
    }

    private Transform FindChildRecursively(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            Transform found = FindChildRecursively(child, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void Update()
    {
        if (isReloading) return;

        // Si está apuntando, actualizamos el estado
        if (playerInput.aim)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        // Si intentamos disparar sin munición
        if (playerInput.shoot && currentAmmo <= 0)
        {
            Debug.Log("Sin munición.");
            return;
        }

        // Solo disparar si está apuntando y no ha pasado el tiempo de recarga entre disparos
        if (playerInput.shoot && isAiming && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
        }

        // Recarga solo si no estamos recargando y hay balas para cargar
        if (playerInput.reload && currentAmmo < maxAmmo && ammoReserve > 0 && !isReloading)
        {
            StartCoroutine(Reload());
            return;
        }

        if (IsPlayerInRange())
        {
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(true);
                pickupText.text = $"Presiona E para recoger {weaponName}";
            }

            if (playerInput.pickup)
            {
                PickupWeapon();
            }
        }
        else if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }
    }


    private void Shoot()
    {
        Debug.Log("Disparando...");

        // Actualizamos el cooldown para el próximo disparo
        nextFireTime = Time.time + fireRate;

        currentAmmo--;
        UpdateAmmoUI();

        // Instancia la bala desde el prefab en el punto de disparo
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        
        // Opcional: Si el prefab de la bala tiene un Rigidbody, le damos velocidad inicial
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * 20f; // Cambia la velocidad según tu necesidad
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log($"Distancia al jugador: {distance}");
        return distance <= pickupRange;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = Mathf.Min(maxAmmo - currentAmmo, ammoReserve);
        currentAmmo += ammoToReload;
        ammoReserve -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"Munición: {currentAmmo}/{maxAmmo} - Reserva: {ammoReserve}";
        }
    }

    private void PickupWeapon()
{
    Debug.Log("Iniciando recogida de arma");

    // Desactivar los colliders del arma
    Collider[] colliders = GetComponentsInChildren<Collider>();
    foreach (var collider in colliders)
    {
        collider.enabled = false;
    }

    if (playerHand != null)
    {
        // Asegurarse de que los renderizadores estén activados (en caso de que el arma estuviera oculta)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }

        // Guardar las transformaciones originales
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;
        Vector3 originalScale = transform.localScale;

        // Adjuntar el arma a la mano del jugador
        transform.SetParent(playerHand);

        // Aplicar la posición y rotación correctas basadas en el offset y rotación deseada
        transform.localPosition = handOffset; // Asegúrate de configurar correctamente handOffset
        transform.localRotation = Quaternion.Euler(handRotation); // Asegúrate de configurar correctamente handRotation

        // Restaurar la escala original
        transform.localScale = originalScale;

        Debug.Log($"{weaponName} recogida y colocada en la mano en posición local: {transform.localPosition}, rotación local: {transform.localRotation.eulerAngles}");
        Debug.Log($"Posición global del arma: {transform.position}, Rotación global: {transform.rotation.eulerAngles}");
    }
    else
    {
        Debug.LogError("No se pudo colocar el arma en la mano del jugador. playerHand es null.");
    }

    // Ocultar el texto de recogida y desactivar el script
    if (pickupText != null) pickupText.gameObject.SetActive(false);
    enabled = false;
}



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AmmoPickup")) // Etiqueta del cartucho en el escenario
        {
            // Recogemos el cartucho y lo agregamos a la reserva
            ammoReserve += 12; // Por ejemplo, 12 balas por cartucho
            Destroy(other.gameObject); // Eliminamos el cartucho del escenario
            Debug.Log("Recogiste un cartucho. Munición en reserva: " + ammoReserve);
        }
    }
}