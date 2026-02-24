using UnityEngine;

public class ControlCamara : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        GameObject suelo = GameObject.Find("Suelo");

        if (suelo != null)
        {
            SpriteRenderer sr = suelo.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Forzamos el tamaño a 100x100 si el Size se rompió
                float alto = sr.size.y > 1 ? sr.size.y : 100f;

                transform.position = new Vector3(suelo.transform.position.x, suelo.transform.position.y, -10f);
                cam.orthographic = true;
                cam.orthographicSize = (alto / 2f) + 5f;

                Debug.Log($"<color=green>📸 CÁMARA: Ajustada a tamaño {alto}. Posición: {transform.position}</color>");
            }
            else { Debug.LogError("❌ El objeto Suelo no tiene SpriteRenderer"); }
        }
        else { Debug.LogError("❌ NO ENCONTRÉ el objeto llamado 'Suelo' en la jerarquía"); }
    }
}
