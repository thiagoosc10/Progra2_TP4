using UnityEngine;
using TMPro;

public class FlashlightController : MonoBehaviour
{
    public float pickupRange = 1f;
    public string weaponName = "Linterna";
    public Vector3 handOffset = Vector3.zero;
    public Vector3 handRotation = Vector3.zero;
    
    [Header("Light Settings")]
    public Light flashlightLight;
    public float lightIntensity = 1f;
    public float lightRange = 10f;
    public float spotAngle = 45f;
    
    private StarterAssetsInputs playerInput;
    private GameObject player;
    private Transform playerHand;
    private TextMeshProUGUI pickupText;
    private bool isPickedUp = false;
    
    public bool forceAlwaysOn = false;


    private void Start()
    {
        playerInput = FindObjectOfType<StarterAssetsInputs>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHand = FindPlayerHand();
        pickupText = GameObject.Find("PickupText")?.GetComponent<TextMeshProUGUI>();
        
        if (flashlightLight == null)
        {
            flashlightLight = GetComponentInChildren<Light>();
        }
    
        if (playerInput == null) Debug.LogError("No se encontró StarterAssetsInputs");
        if (player == null) Debug.LogError("No se encontró el jugador");
        if (playerHand == null) Debug.LogError("No se encontró RightHandAttach");
        if (pickupText == null) Debug.LogError("No se encontró PickupText");
        if (flashlightLight == null) Debug.LogError("No se encontró el componente Light en la linterna");

        if (pickupText != null) pickupText.gameObject.SetActive(false);
        if (flashlightLight != null) 
        {
            flashlightLight.enabled = false;
            ConfigureLight();
        }

        if (flashlightLight != null) 
        {
            ConfigureLight();
            flashlightLight.enabled = forceAlwaysOn;
            Debug.Log($"Flashlight light configured. Force Always On: {forceAlwaysOn}");
        }
    }

    private void ConfigureLight()
    {
        flashlightLight.type = LightType.Spot;
        flashlightLight.intensity = lightIntensity;
        flashlightLight.range = lightRange;
        flashlightLight.spotAngle = spotAngle;
        Debug.Log($"Light configured - Intensity: {lightIntensity}, Range: {lightRange}, Angle: {spotAngle}");
        
        // Additional debug information
        Debug.Log($"Light color: {flashlightLight.color}");
        Debug.Log($"Light shadows: {flashlightLight.shadows}");
        Debug.Log($"Light culling mask: {flashlightLight.cullingMask}");
        Debug.Log($"Light render mode: {flashlightLight.renderMode}");
    }

    private void Update()
    {
        if (!isPickedUp)
        {
            if (IsPlayerInRange())
            {
                if (pickupText != null)
                {
                    pickupText.gameObject.SetActive(true);
                    pickupText.text = $"Presiona E para recoger {weaponName}";
                }

                if (playerInput.pickup)
                {
                    PickupFlashlight();
                }
            }
            else if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false);
            }
        }
        else
        {
            // Controlar la luz de la linterna
            if (flashlightLight != null)
            {
                flashlightLight.enabled = playerInput.aim;
            }
        }

        if (isPickedUp && flashlightLight != null)
        {
            bool shouldBeOn = forceAlwaysOn || playerInput.aim;
            flashlightLight.enabled = shouldBeOn;
            Debug.Log($"Flashlight state: {(shouldBeOn ? "ON" : "OFF")}, Force Always On: {forceAlwaysOn}, Player aiming: {playerInput.aim}");
            
            // Additional real-time debug info
            Debug.Log($"Flashlight position: {flashlightLight.transform.position}");
            Debug.Log($"Flashlight forward direction: {flashlightLight.transform.forward}");
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.transform.position) <= pickupRange;
    }

    private void PickupFlashlight()
    {
        if (playerHand != null)
        {
            transform.SetParent(playerHand);
            transform.localPosition = handOffset;
            transform.localRotation = Quaternion.Euler(handRotation);
            transform.localScale = Vector3.one;

            // Desactivar los colliders
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            isPickedUp = true;
        Debug.Log("Flashlight picked up");
        if (pickupText != null) pickupText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("No se pudo colocar la linterna en la mano del jugador. playerHand es null.");
        }
    }

    private Transform FindPlayerHand()
    {
        if (player == null) return null;
        return FindChildRecursively(player.transform, "mixamorig7:RightHand/RightHandAttach") 
            ?? FindChildRecursively(player.transform, "mixamorig7:RightHand");
    }

    private Transform FindChildRecursively(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;
            Transform found = FindChildRecursively(child, childName);
            if (found != null) return found;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
