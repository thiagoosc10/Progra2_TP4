using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimController : MonoBehaviour
{
    public MultiAimConstraint headConstraint;  // Constraint para la cabeza
    public MultiAimConstraint bodyConstraint;  // Constraint para el cuerpo
    public MultiAimConstraint armConstraint;   // Constraint para los brazos

    public StarterAssetsInputs inputScript;  // Referencia al sistema de input

    // Pesos o valores para cuando no está apuntando (Idle o camina)
    public float headRestWeight = 0f;
    public float bodyRestWeight = 0f;
    public float armRestWeight = 0f;

    // Pesos o valores para cuando está apuntando
    public float headAimWeight = 1f;
    public float bodyAimWeight = 1f;
    public float armAimWeight = 1f;

    // Variables para otros ajustes de rotación o posicionamiento según el estado
    public Vector3 headRestRotation;
    public Vector3 bodyRestRotation;
    public Vector3 headAimRotation;
    public Vector3 bodyAimRotation;

    void Update()
    {
        // Usa la variable del sistema de input para saber si el personaje está apuntando
        if (inputScript.aim)
        {
            // Apuntando
            SetAimState(true);
        }
        else
        {
            // No está apuntando
            SetAimState(false);
        }
    }

    // Función para establecer el estado de apuntado o reposo
    void SetAimState(bool isAiming)
    {
        if (isAiming)
        {
            // Apuntando: aplicar pesos y rotaciones de apuntado
            headConstraint.weight = headAimWeight;
            bodyConstraint.weight = bodyAimWeight;
            armConstraint.weight = armAimWeight;

            // Ajustar rotaciones específicas si es necesario
            SetConstraintRotation(headConstraint, headAimRotation);
            SetConstraintRotation(bodyConstraint, bodyAimRotation);
        }
        else
        {
            // No está apuntando: aplicar pesos y rotaciones de reposo
            headConstraint.weight = headRestWeight;
            bodyConstraint.weight = bodyRestWeight;
            armConstraint.weight = armRestWeight;

            // Ajustar rotaciones específicas si es necesario
            SetConstraintRotation(headConstraint, headRestRotation);
            SetConstraintRotation(bodyConstraint, bodyRestRotation);
        }
    }

    // Función para aplicar la rotación de un constraint
    void SetConstraintRotation(MultiAimConstraint constraint, Vector3 rotation)
    {
        // Asumiendo que puedes ajustar la rotación de los constraints de esta forma
        var constraintData = constraint.data;
        constraintData.offset = rotation;  // Si "offset" es el parámetro que ajusta la rotación
        constraint.data = constraintData;
    }
}




