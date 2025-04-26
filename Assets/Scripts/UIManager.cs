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

    // Ana UI Butonlarý
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


    // Genel Popup Açýcý
    private void OpenPopup(GameObject optionsGroup)
    {
        Popup_Panel.SetActive(true);

        // Bütün alt gruplarý kapat
        foodOptions.SetActive(false);
        sleepOptions.SetActive(false);
        funOptions.SetActive(false);
        healthOptions.SetActive(false);
        hygieneOptions.SetActive(false);
        weightOptions.SetActive(false);
        happinessOptions.SetActive(false);
        disciplineOptions.SetActive(false);

        // Sadece seçileni aç
        optionsGroup.SetActive(true);
    }

    // Þu anda Popup paneli içindeki özel butonlara baðlanacak örnek fonksiyonlar:

    public void OnMealButtonClicked()
    {
        Debug.Log("Meal seçildi, hunger azalýyor.");
        // Hunger'ý azalt
        Popup_Panel.SetActive(false);
    }

    public void OnSnackButtonClicked()
    {
        Debug.Log("Snack seçildi, hunger artýyor ama weight de artýyor!");
        // Hunger'ý azalt ama weight artýr
        Popup_Panel.SetActive(false);
    }

    public void OnLightOnButtonClicked()
    {
        Debug.Log("Iþýk açýldý.");
        // Iþýðý aç
        Popup_Panel.SetActive(false);
    }

    public void OnLightOffButtonClicked()
    {
        Debug.Log("Iþýk kapandý.");
        // Iþýðý kapat
        Popup_Panel.SetActive(false);
    }








}
