using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Vector3 playerSpawnPosition;
    public static GameManager GM;

    void Awake()
    {
        MakeThisTheOnlyGameManager();
        playerSpawnPosition = new Vector3(0, 0, -1);
    }

    private void Start()
    {
        
    }

    void MakeThisTheOnlyGameManager()
    {
        if (GM == null)
        {
            DontDestroyOnLoad(gameObject);
            GM = this;
        }
        else
        {
            if (GM != this)
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnUpdateSpawn(Component sender, object data)
    {
        playerSpawnPosition = (Vector3)data;
    }

    public void OnTouchedDeath(Component sender, object data)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}