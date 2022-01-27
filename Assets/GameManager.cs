using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAction
{
    public Vector3 playerPosition;
    public Quaternion playerQuaternion;
    public Dalle originDalle;
    public Dalle targetDalle;
    public MovingBlock movingBlock;

    public PlayerAction(Dalle originDalle, Dalle targetDalle, MovingBlock movingBlock, Vector3 playerPosition, Quaternion playerQuaternion)
    {
        this.originDalle = originDalle;
        this.targetDalle = targetDalle;
        this.movingBlock = movingBlock;
        this.playerPosition = playerPosition;
        this.playerQuaternion = playerQuaternion;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<PlayerAction> playerActions = new List<PlayerAction>();
    public GameObject player;

    [SerializeField] private List<Dalle> requiredDalles = new List<Dalle>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        foreach (Dalle dalle in requiredDalles)
        {
            dalle.mode = Dalle.Mode.Required;
        }
    }

    public void UndoAction()
    {
        if (playerActions.Count == 0)
            return;

        PlayerAction lastPlayerAction = playerActions.Last();

        if (lastPlayerAction != null)
        {
            Debug.Log(player.transform.position);
            player.transform.position = lastPlayerAction.playerPosition;
            player.transform.rotation = lastPlayerAction.playerQuaternion;

            lastPlayerAction.movingBlock.transform.position = lastPlayerAction.originDalle.cubeTransformPosition.position;
        
            playerActions.Remove(lastPlayerAction);   
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
