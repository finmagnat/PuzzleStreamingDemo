using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private void Update() => transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
}
