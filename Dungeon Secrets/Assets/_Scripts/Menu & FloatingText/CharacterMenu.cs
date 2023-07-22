using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    public Text levelText, hitpointText, goldText, upgradeCostText, xptext; // Текстові поля

    public Image characterSelectionSprite;
    public Image weaponSprite;
    public RectTransform xpBar;
    public Animator animator;
    public int currentCharacterSelection;

    //Menu
    private bool isMenuOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isMenuOpen)
            {
                OpenMenu(true);
                isMenuOpen = false;
            }
            else
            {
                OpenMenu(false);
                isMenuOpen = true;
            }
            UpdateMenu();
        }
    }

    public void OpenMenu(bool isMenuOpen)
    {
        if (isMenuOpen)
        {
            animator.SetBool("isOpen", true);
        }
        else
        {
            animator.SetBool("isOpen", false);
        }
    }

    public void OnArrowClick(bool right)
    {
        if (SceneManager.GetActiveScene().name == "Outside")
        {
            if (right)
            {
                currentCharacterSelection++;

                if (currentCharacterSelection == GameManager.instance.characterPrefabs.Count)
                    currentCharacterSelection = 0;

                OnSelectionChanged();
            }
            else
            {
                currentCharacterSelection--;

                if (currentCharacterSelection < 0)
                    currentCharacterSelection = GameManager.instance.characterPrefabs.Count - 1;

                OnSelectionChanged();
            }
        }
    }

    private void OnSelectionChanged()
    {
        characterSelectionSprite.sprite = GameManager.instance.characterPrefabs[currentCharacterSelection].GetComponent<SpriteRenderer>().sprite;
        SwapPlayer(currentCharacterSelection);
    }

    // Зміна гравця
    public void SwapPlayer(int characterSelection)
    {
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        Destroy(currentPlayer);

        GameObject selectedCharacter = GameManager.instance.characterPrefabs[characterSelection];

        GameObject newCharacter = Instantiate(selectedCharacter, currentPlayer.transform.position, Quaternion.identity);

        GameManager.instance.player = newCharacter.GetComponent<Player>();
        GameManager.instance.weapon = newCharacter.GetComponentInChildren<Weapon>();
    }

    // Оновлення зброї
    public void OnUpgradeClick()
    {
        if (GameManager.instance.TryUpgradeWeapone())
            UpdateMenu();
    }

    // Оновлення інформації в меню
    public void UpdateMenu()
    {
        // Зброя
        weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLevel];
        if (GameManager.instance.weapon.weaponLevel == GameManager.instance.weaponPrices.Count)
            upgradeCostText.text = "Max";
        else
            upgradeCostText.text = GameManager.instance.weaponPrices[GameManager.instance.weapon.weaponLevel].ToString();

        // Текстові поля
        levelText.text = GameManager.instance.GetCurrentLevel().ToString();
        hitpointText.text = GameManager.instance.player.hitpoint.ToString();
        goldText.text = GameManager.instance.gold.ToString();

        // Рядок досвіду
        int currLevel = GameManager.instance.GetCurrentLevel();
        if (currLevel == GameManager.instance.xpTable.Count)
        {
            xptext.text = GameManager.instance.experience.ToString() + "total experience points";
            xpBar.localScale = Vector3.one;
        }
        else
        {
            int prevLevelXp = GameManager.instance.GetXpLevel(currLevel - 1);
            int currLevelXp = GameManager.instance.GetXpLevel(currLevel);

            int diff = currLevelXp - prevLevelXp;
            int currXpIntoLevel = GameManager.instance.experience - prevLevelXp;

            float completionRatio = (float)currXpIntoLevel / (float)diff;
            xpBar.localScale = new Vector3(completionRatio, 1, 1);
            xptext.text = currXpIntoLevel.ToString() + " / " + diff;
        }
    }
}
