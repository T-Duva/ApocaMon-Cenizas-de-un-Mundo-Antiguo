using UnityEngine;

/// <summary>
/// Cámara libre: WASD desplazamiento, Q/E subir-bajar, clic derecho mantenido para rotar vista.
/// </summary>
[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{
    [Header("Velocidad")]
    [SerializeField] private float flySpeed = 10f;

    [Header("Rotación (clic derecho)")]
    [SerializeField] private float sensibilidadMouse = 2f;

    private float rotacionY;
    private float rotacionX;

    void Start()
    {
        Vector3 euler = transform.eulerAngles;
        rotacionY = euler.y;
        rotacionX = euler.x;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            rotacionY += Input.GetAxis("Mouse X") * sensibilidadMouse;
            rotacionX -= Input.GetAxis("Mouse Y") * sensibilidadMouse;
            rotacionX = Mathf.Clamp(rotacionX, -89f, 89f);
            transform.rotation = Quaternion.Euler(rotacionX, rotacionY, 0f);
        }

        Vector3 movimiento = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movimiento += transform.forward;
        if (Input.GetKey(KeyCode.S)) movimiento -= transform.forward;
        if (Input.GetKey(KeyCode.D)) movimiento += transform.right;
        if (Input.GetKey(KeyCode.A)) movimiento -= transform.right;
        if (Input.GetKey(KeyCode.E)) movimiento += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) movimiento -= Vector3.up;

        if (movimiento.sqrMagnitude > 0.01f)
        {
            movimiento.Normalize();
            transform.position += movimiento * (flySpeed * dt);
        }
    }
}
