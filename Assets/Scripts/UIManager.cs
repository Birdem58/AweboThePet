using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PetManager;

public class UIManager : MonoBehaviour
{
    public GameObject popup_Panel;
    public GameObject lightGameObject;
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
        popup_Panel.SetActive(false);
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
        PetManager.Instance.Feed(FoodType.Meal);
        PetManager.Instance.hygiene += 50f;
        OpenPopup(healthOptions);
    }

    public void OnHygieneButtonClicked()
    {
        PetManager.Instance.Clean();
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
        popup_Panel.SetActive(true);

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



    public void OnLightOnButtonClicked()
    {
        Debug.Log("Iþýk açýldý.");
        PetManager.Instance.isLightOff = false; 
        lightGameObject.SetActive(false); 
        popup_Panel.SetActive(false);
    }

    public void OnLightOffButtonClicked()
    {
        Debug.Log("Iþýk kapandý.");
        PetManager.Instance.isLightOff = true; 
        lightGameObject.SetActive(true); 
        popup_Panel.SetActive(false);
    }

    public void OnMealButtonClicked()
    {
        PetManager.Instance.Feed(FoodType.Meal);
        Debug.Log("Ana yemek verildi. Açlýk azaltýldý.");
        popup_Panel.SetActive(false);
    }

    public void OnSnackButtonClicked()
    {
        PetManager.Instance.Feed(FoodType.Snack);
        Debug.Log("Atýþtýrmalýk verildi. Açlýk ve mutluluk arttý!");
        popup_Panel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        popup_Panel.SetActive(false);
    }



}
