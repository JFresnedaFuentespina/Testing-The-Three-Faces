using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;

public class ChangeCharacter : MonoBehaviour
{
    public GameObject ghost;
    public GameObject esqueleto;
    public bool showingGhost = false;
    public List<string> actions = new List<string>();
    public GameObject explosionVFX;

    private GameObject monedaOriginal;
    private RotateCoin rotateCoin;
    private GameObject cursorManagerGO;
    private CursorManager cursorManager;

    public float switchCooldown = 2f;
    private float lastSwitchTime = -Mathf.Infinity;


    public delegate void OnChangePlayerIcon();
    public static event OnChangePlayerIcon OnChangePlayerIconEvent;

    void OnDestroy()
    {
        // PickupItem.OnNewChangeCharacterActionEvent -= AddAction;
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent -= AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent -= AddAction;
    }

    void Start()
    {
        cursorManagerGO = GameObject.Find("CursorManagerGO");
        if (cursorManagerGO != null)
        {
            cursorManager = cursorManagerGO.GetComponent<CursorManager>();
        }
        esqueleto.SetActive(true);
        ghost.SetActive(false);
        monedaOriginal = GameObject.Find("MonedaOriginal").gameObject;
        if (monedaOriginal != null)
            rotateCoin = monedaOriginal.GetComponent<RotateCoin>();
        SubscribeToPickupItemsEvents();
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            if (playerData.actions != null)
                actions = playerData.actions;
            else
                actions = new List<string>();
        }
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("ChangeCharacter"))
            && Time.time >= lastSwitchTime + switchCooldown)
        {
            SwitchCharacter();
            lastSwitchTime = Time.time;
        }
    }

    public void SubscribeToPickupItemsEvents()
    {
        // PickupItem.OnNewChangeCharacterActionEvent += AddAction;
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
    }

    void SwitchCharacter()
    {
        if (OnChangePlayerIconEvent != null)
        {
            OnChangePlayerIconEvent();
        }
        showingGhost = !showingGhost;

        if (actions.Contains("Hourglass"))
        {
            Debug.Log("FREEZE TIME!");
            FreezeEnemies();

        }
        if (actions.Contains("Bomb"))
        {
            GameObject bomb = Instantiate(explosionVFX, showingGhost ? esqueleto.transform.position : ghost.transform.position, Quaternion.identity, gameObject.transform);
            Destroy(bomb, 2f);
        }

        if (showingGhost)
        {
            if (cursorManager != null)
                cursorManager.ChangeCursorToCross();
            ghost.transform.position = esqueleto.transform.position;
            ghost.SetActive(true);
            esqueleto.SetActive(false);
        }
        else
        {
            if (cursorManager != null)
                cursorManager.ChangeCursorToSword();
            esqueleto.transform.position = ghost.transform.position;
            esqueleto.SetActive(true);
            ghost.SetActive(false);
        }

        if (rotateCoin != null)
        {
            rotateCoin.rotate = true;                // gira la moneda
            rotateCoin.StartCooldown(switchCooldown); // sincroniza la barra
        }
    }
    public void RemoveAction(string action)
    {
        actions.Remove(action);
    }

    public void AddAction(string action)
    {
        actions.Add(action);
    }

    public List<string> GetUnlockedActions()
    {
        return actions;
    }

    private void FreezeEnemies()
    {
        // StartCoroutine(FreezeEnemyMoves());
        StartCoroutine(FreezeBasicEnemyAI());
        StartCoroutine(FreezeBossCara());
        StartCoroutine(FreezeBossCruz());
        StartCoroutine(FreezeBossCanto());
    }

    // IEnumerator FreezeEnemyMoves()
    // {
    //     // Obtener todos los enemigos usando el wrapper que tienes
    //     EnemyMove[] enemiesArray = FindObjectsByType<EnemyMove>(FindObjectsSortMode.None);
    //     List<EnemyMove> enemies = new List<EnemyMove>(enemiesArray);

    //     if (enemies.Count == 0)
    //     {
    //         yield return null;
    //     }

    //     Dictionary<EnemyMove, float> originalSpeeds = new Dictionary<EnemyMove, float>();
    //     foreach (var enemy in enemies)
    //     {
    //         originalSpeeds[enemy] = enemy.velocity;
    //         enemy.velocity = 0f;
    //         Animator anim = enemy.GetComponent<Animator>();
    //         if (anim != null)
    //             anim.speed = 0f;
    //     }

    //     yield return new WaitForSecondsRealtime(2f);

    //     foreach (var enemy in enemies)
    //     {
    //         enemy.velocity = originalSpeeds[enemy];
    //         Animator anim = enemy.GetComponent<Animator>();
    //         if (anim != null)
    //             anim.speed = 1f;
    //     }
    // }

    IEnumerator FreezeBasicEnemyAI()
    {
        // Obtener todos los enemigos usando el wrapper que tienes
        ZombieFSM[] enemiesArray = FindObjectsByType<ZombieFSM>(FindObjectsSortMode.None);
        List<ZombieFSM> enemies = new List<ZombieFSM>(enemiesArray);

        if (enemies.Count == 0)
        {
            yield return null;
        }

        foreach (var enemy in enemies)
        {
            enemy.StopAgent();
        }

        yield return new WaitForSecondsRealtime(2f);

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ResetAgent();
            }
        }
    }

    IEnumerator FreezeBossCara()
    {
        // Obtener todos los enemigos usando el wrapper que tienes
        CaraFSM[] enemiesArray = FindObjectsByType<CaraFSM>(FindObjectsSortMode.None);
        List<CaraFSM> enemies = new List<CaraFSM>(enemiesArray);

        if (enemies.Count == 0)
        {
            yield return null;
        }

        foreach (var enemy in enemies)
        {
            enemy.Freeze();
        }

        yield return new WaitForSecondsRealtime(2f);

        foreach (var enemy in enemies)
        {
            enemy.UnFreeze();
        }
    }

    IEnumerator FreezeBossCruz()
    {
        // Obtener todos los enemigos usando el wrapper que tienes
        CruzFSM[] enemiesArray = FindObjectsByType<CruzFSM>(FindObjectsSortMode.None);
        List<CruzFSM> enemies = new List<CruzFSM>(enemiesArray);

        if (enemies.Count == 0)
        {
            yield return null;
        }

        foreach (var enemy in enemies)
        {
            enemy.Freeze();
        }

        yield return new WaitForSecondsRealtime(2f);

        foreach (var enemy in enemies)
        {
            enemy.UnFreeze();
        }
    }

    IEnumerator FreezeBossCanto()
    {
        CantoFSM[] enemiesArray = FindObjectsByType<CantoFSM>(FindObjectsSortMode.None);
        List<CantoFSM> enemies = new List<CantoFSM>(enemiesArray);

        if (enemies.Count == 0)
        {
            yield return null;
        }

        foreach (var enemy in enemies)
        {
            enemy.Freeze();
        }

        yield return new WaitForSecondsRealtime(2f);

        foreach (var enemy in enemies)
        {
            enemy.UnFreeze();
        }
    }
}
