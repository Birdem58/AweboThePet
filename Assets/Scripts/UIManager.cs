using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Popup_Panel;

    public GameObject foodOptions;
    public GameObject sleepOptions;
    public GameObject funOptions;
    public GameObject healthOptions;
    public GameObject hygieneOptions;
    public GameObject weightOptions;
    public GameObject happinessOptions;
    public GameObject disciplineOptions;

    private void Start()
    {
        Popup_Panel.SetActive(false);
    }

    // Ana UI Butonlar�
    public void OnFoodButtonClicked()
    {
        OpenPopup(foodOptions);
    }

    public void OnSleepButtonClicked()
    {
        OpenPopup(sleepOptions);
    }

    public void OnFunButtonClicked()
    {
        OpenPopup(funOptions);
    }

    public void OnHealthButtonClicked()
    {
        OpenPopup(healthOptions);
    }

    public void OnHygieneButtonClicked()
    {
        OpenPopup(hygieneOptions);
    }

    public void OnWeightButtonClicked()
    {
        OpenPopup(weightOptions);
    }

    public void OnHappinessButtonClicked()
    {
        OpenPopup(happinessOptions);
    }

    public void OnDisciplineButtonClicked()
    {
        OpenPopup(disciplineOptions);
    }


    // Genel Popup A��c�
    private void OpenPopup(GameObject optionsGroup)
    {
        Popup_Panel.SetActive(true);

        // B�t�n alt gruplar� kapat
        foodOptions.SetActive(false);
        sleepOptions.SetActive(false);
        funOptions.SetActive(false);
        healthOptions.SetActive(false);
        hygieneOptions.SetActive(false);
        weightOptions.SetActive(false);
        happinessOptions.SetActive(false);
        disciplineOptions.SetActive(false);

        // Sadece se�ileni a�
        optionsGroup.SetActive(true);
    }

    // �u anda Popup paneli i�indeki �zel butonlara ba�lanacak �rnek fonksiyonlar:

    public void OnMealButtonClicked()
    {
        Debug.Log("Meal se�ildi, hunger azal�yor.");
        // Hunger'� azalt
        Popup_Panel.SetActive(false);
    }

    public void OnSnackButtonClicked()
    {
        Debug.Log("Snack se�ildi, hunger art�yor ama weight de art�yor!");
        // Hunger'� azalt ama weight art�r
        Popup_Panel.SetActive(false);
    }

    public void OnLightOnButtonClicked()
    {
        Debug.Log("I��k a��ld�.");
        // I���� a�
        Popup_Panel.SetActive(false);
    }

    public void OnLightOffButtonClicked()
    {
        Debug.Log("I��k kapand�.");
        // I���� kapat
        Popup_Panel.SetActive(false);
    }








}
