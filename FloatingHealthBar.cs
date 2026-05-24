using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;   // Slider instead of Image
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI labelTextState;
    [SerializeField] private Transform target;      // Bot this health bar follows
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>(); // optional
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;

            // Rotate toward camera
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    // Update the slider + label
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = maxValue;
        healthSlider.value = currentValue;

        if (labelText != null)
            labelText.text = $"{Mathf.CeilToInt(currentValue)} / {Mathf.CeilToInt(maxValue)}";
    }

    public void UpdateState(string stateName)
    {
        if (labelText != null)
            labelTextState.text = $"State: {stateName}";
    }
}
