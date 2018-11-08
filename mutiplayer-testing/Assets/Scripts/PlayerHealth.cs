using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] int maxHealth = 3;


    // When health changes it will syncronized with all versions of this object
    // if player 1's health is reduced all player1's will have there health reduced.
    [SyncVar(hook = "OnHealthChanged")] int health; 
    //when the server changes the healkth it also calls the OnHealthChanged function which updated the local plyaer UI
    // only the server can change a syncVar

    Player player;


    void Awake()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback]
    void OnEnable()
    {
        // this sets max health for players upon respawning
        health = maxHealth;
    }

    [ServerCallback]
    void Start()
    {
        // this lets the server know to set the local player who is also the server to max health
        health = maxHealth;
    }

    [Server]
    public bool TakeDamage()
    {
        bool died = false;

        if (health <= 0)
            return died;

        health--;
        died = health <= 0;

        RpcTakeDamage(died);

        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (isLocalPlayer)
            // this will cause the take damage img to flash over the players screen
            PlayerCanvas.canvas.FlashDamageEffect();

        if (died)
            player.Die();
    }


    void OnHealthChanged(int value)
    {
        health = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetHealth(value);
        // sets the players health
    }
}