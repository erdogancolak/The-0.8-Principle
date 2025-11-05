using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NumberGenerator : MonoBehaviour
{
    public Transform numberParentContent;
    public GameObject numberPrefab;

    public TMP_Text selectedNumberText;
    public Button confirmButton;

    private List<Button> numberButtons = new List<Button>();
    private int selectedNumber = -1;
    private Button lastSelectedButton;

    public Color selectedButtonColor;
    public Color defaultButtonColor;
    void Start()
    {
        GenerateNumber();
        confirmButton.onClick.AddListener(OnConfirmPressed);
    }

    private void GenerateNumber()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject newNumberItem = Instantiate(numberPrefab, numberParentContent);
            TMP_Text numberText = newNumberItem.transform.GetChild(0).GetComponent<TMP_Text>();
            Button btn = newNumberItem.GetComponent<Button>(); 
            
            numberText.text = i.ToString();

            int capturedNum = i;

            btn.onClick.AddListener(() => OnNumberClicked(capturedNum, btn));
            numberButtons.Add(btn);
        }
    }

    void OnNumberClicked(int number, Button clickedButton)
    {
        foreach (Button btn in numberButtons)
        {
            btn.GetComponent<Image>().color = defaultButtonColor;
        }

        clickedButton.GetComponent<Image>().color = selectedButtonColor;

        selectedNumber = number;
        lastSelectedButton = clickedButton;
        selectedNumberText.text = number.ToString();
    }
    void OnConfirmPressed()
    {
        if (selectedNumber != -1)
        {
            Debug.Log("Onaylandý! Seçilen sayý: " + selectedNumber);
        }
        else
        {
            Debug.LogWarning("Henüz bir sayý seçilmedi!");
        }
    }
}
