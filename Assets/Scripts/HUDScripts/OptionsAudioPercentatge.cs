using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsAudioPercentatge : MonoBehaviour
{
    [Header("UI References")]
    public Slider audioSlider; // Asignar en el inspector
    public TextMeshProUGUI audioPercentatgeTxt;

    void Start()
    {
        if (audioSlider != null)
        {
            // Configurar listener para cambios en el slider
            audioSlider.onValueChanged.AddListener(OnSliderValueChanged);

            // Inicializar el texto con el valor actual
            UpdateAudioText(audioSlider.value);
            UpdateAllAudioSourcesVolume(audioSlider.value);
        }
        else
        {
            Debug.LogError("Audio Slider no asignado en OptionsAudioPercentatge!");
        }
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateAudioText(value);
        UpdateAllAudioSourcesVolume(value);
    }

    private void UpdateAudioText(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100);
        if (audioPercentatgeTxt != null)
            audioPercentatgeTxt.text = percentage + "%";
    }

    private void UpdateAllAudioSourcesVolume(float value)
    {
        AudioSource[] sources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var audio in sources)
        {
            if (audio != null)
                audio.volume = value;
        }

        AudioListener.volume = value;
    }
}
