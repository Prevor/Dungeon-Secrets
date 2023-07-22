using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            Destroy(pauseMenu.gameObject);
            Destroy(GameObject.Find("SpawnPoint"));
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneLoaded += LoadState;
    }

    // Ресури
    public List<GameObject> characterPrefabs;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    // Посилання
    [HideInInspector] public Player player;
    [HideInInspector] public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public Slider hitpointBarSlider;
    public Animator deathMenuAnim;
    public GameObject hud;
    public GameObject menu;
    public PauseMenu pauseMenu;
    public CharacterMenu characterMenu;

    // Logic
    public int gold;
    public int experience;

    // Floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    // Оновлення зброї
    public bool TryUpgradeWeapone()
    {
        if (weaponPrices.Count <= weapon.weaponLevel)
            return false;

        if (gold >= weaponPrices[weapon.weaponLevel])
        {
            gold -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }
        return false;
    }

    public void SetMaxHelth(int health)
    {
        hitpointBarSlider.maxValue = health;
        hitpointBarSlider.value = health;
    }

    public void SetHealth(int health)
    {
        hitpointBarSlider.value = health;
    }

    // Система досвіду
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) // Макс. рівень
                return r;
        }
        return r;
    }
    public int GetXpLevel(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }

        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel())
            OnLevelUp();
    }

    // Підвищення рівня
    public void OnLevelUp()
    {
        Debug.Log("Level Up!");
        player.OnLevelUp();
        SetHealth(player.maxHitPoint);
    }

    // Завантаження сцени з поточними параметрами
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pauseMenu.Resume();

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            int rndPlayerIndex = Random.Range(0, characterPrefabs.Count);

            // Створення гравця з випадковим префабом
            GameObject playerObject = Instantiate(characterPrefabs[rndPlayerIndex], GameObject.Find("SpawnPoint").transform.position, Quaternion.identity);
            player = playerObject.GetComponent<Player>();
            weapon = playerObject.GetComponentInChildren<Weapon>();

            characterMenu.currentCharacterSelection = rndPlayerIndex;
            characterMenu.characterSelectionSprite.sprite = playerObject.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            if (player != null && GameObject.Find("SpawnPoint"))
            {
               player.transform.position = GameObject.Find("SpawnPoint").transform.position;

            }
        }
    }

    // Меню смерті і перезавантаження
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        SceneManager.LoadScene("Outside");
        player.RespawnPlayer();
    }

    // Збереження статистики
    public void SaveState()
    {
        string s = $"0|{gold}|{experience}|{weapon.weaponLevel}";
        PlayerPrefs.SetString("SaveState", s);
    }

    // Завантаження статистики
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        // Золото
        gold = int.Parse(data[1]);

        // Досвід
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());

        // Зброя
        weapon.SetWeaponLevel(int.Parse(data[3]));
    }
}
