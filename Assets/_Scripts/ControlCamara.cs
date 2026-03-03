using UnityEngine;

/// <summary>
/// Legacy 2D: solo actúa si existe "Suelo" con SpriteRenderer. En escenas 3D (SUELO_DEFINITIVO) no hace nada; usar ControlCamaraRPG.
/// </summary>
public class ControlCamara : MonoBehaviour
{
    void Start()
    {
        if (GameObject.Find("SUELO_DEFINITIVO") != null)
            return;

        Camera cam = GetComponent<Camera>();
        GameObject suelo = GameObject.Find("Suelo");
        if (suelo == null) return;

        SpriteRenderer sr = suelo.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float alto = sr.size.y > 1 ? sr.size.y : 100f;
        transform.position = new Vector3(suelo.transform.position.x, suelo.transform.position.y, -10f);
        cam.orthographic = true;
        cam.orthographicSize = (alto / 2f) + 5f;
    }
}
