using System.Linq;
using Assets.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Skins : MonoBehaviour
{
    [SerializeField] private GameObject chosenMark;
    [SerializeField] private Button[] buyButtons;
    [SerializeField] private TextMeshProUGUI[] costTexts;

    private void OnEnable()
    {
        LoadSkinCosts();
        LoadChosenMarkPosition();
        UpdateInteractableForButtons();
    }

    private void UpdateInteractableForButtons()
    {
        for (var i = 0; i < buyButtons.Length; i++)
        {
            if (int.Parse(costTexts[i].text) <= SavesManager.Coins)
            {
                buyButtons[i].interactable = true;
            }

            else
            {
                buyButtons[i].interactable = false;
            }
        }
    }

    public void HandleBuyButtonClick(int index)
    {
        var cost = int.Parse(costTexts[index].text);
        SavesManager.Coins -= cost;
        SavesManager.Skin = buyButtons[index].GetComponent<Image>().sprite;
        /*YandexGame.savesData.coins = SavesManager.Coins;
        YandexGame.savesData.skin = SavesManager.Skin;
        YandexGame.SaveProgress();*/

        costTexts[index].text = "0";
        chosenMark.transform.position = buyButtons[index].transform.position + Vector3.up * 3;
        UpdateInteractableForButtons();
        
        SaveSkinCosts();
    }

    public void LoadSkinCosts()
    {
        if (SavesManager.SkinCosts == null || SavesManager.SkinCosts.Length < 1)
            return;

        for (var i = 0; i < costTexts.Length; i++)
        {
            costTexts[i].text = SavesManager.SkinCosts[i];
        }
    }

    public void LoadChosenMarkPosition()
    {
        if (SavesManager.Skin == null)
        {
            chosenMark.transform.position = buyButtons[0].transform.position + Vector3.up * 3;
            return;
        }
        
        chosenMark.transform.position =
            buyButtons.First((b) => b.GetComponent<Image>().sprite.Equals(SavesManager.Skin)).transform.position +
            Vector3.up * 3;
    }

    public void SaveSkinCosts()
    {
        SavesManager.SkinCosts = costTexts.Select((t) => t.text).ToArray();
        //YandexGame.savesData.skinCosts = costTexts.Select((t) => t.text).ToArray();
    }
}
