using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 12; // Cantidad de munición que recarga el cartucho

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponPickup weapon = other.GetComponentInChildren<WeaponPickup>();
            if (weapon != null)
            {
                weapon.ammoReserve += ammoAmount;
                Debug.Log("Cartucho recogido. Munición en reserva: " + weapon.ammoReserve);
                Destroy(gameObject); // Destruye el cartucho después de recogerlo
            }
        }
    }
}

