using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HygineBar : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null) Debug.LogError("Slider componenti bulunamadý!");
    }

    void Update()
    {
        if (PetManager.Instance != null)
        {
            slider.value = PetManager.Instance.hygiene;
        }
    }
}
