using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    TMP_Text _txt;

    private void Start()
    {
        _txt = GetComponent<TMP_Text>();
    }
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; 
        float fps = 1.0f / deltaTime;
        _txt.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}
