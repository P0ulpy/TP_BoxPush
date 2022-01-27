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
        this.playerPosition = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
        this.playerQuaternion = new Quaternion(playerQuaternion.x, playerQuaternion.y, playerQuaternion.z, playerQuaternion.w);
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<PlayerAction> playerActions = new List<PlayerAction>();
    public GameObject player;

    [SerializeField] private List<Dalle> requiredDalles = new List<Dalle>();
    public int FilledDalles => _filledDalles;
    private int _filledDalles = 0;
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

    public void onDalleFilled(Dalle dalle)
    {
        if (dalle.mode == Dalle.Mode.Required)
        {
            if (++_filledDalles == requiredDalles.Count)
                Debug.Log("GG");
        }
    }

    public void onDalleEmpty(Dalle dalle)
    {
        if (dalle.mode == Dalle.Mode.Required)
        {
            _filledDalles--;
        }
    }

    public void UndoAction()
    {
        if (playerActions.Count == 0)
            return;

        PlayerAction lastPlayerAction = playerActions.Last();

        if (lastPlayerAction != null)
        {
            var characterController = player.GetComponent<CharacterController>();

            // absolument horrible a voir mais c'est le seul moyen avec mon fonctionnnement, pourrait etre remplacer par un characterController.Move() ce qui permeterais d'avoir une interpolation propre entre deux états
            characterController.enabled = false;
            player.transform.position = lastPlayerAction.playerPosition;
            characterController.enabled = true;

            player.transform.rotation = lastPlayerAction.playerQuaternion;
            
            Debug.Log(lastPlayerAction.playerPosition);
            Debug.Log(player.transform.position);

            lastPlayerAction.movingBlock.transform.position = lastPlayerAction.originDalle.cubeTransformPosition.position;
        
            lastPlayerAction.originDalle.IsFilled = true;
            lastPlayerAction.targetDalle.IsFilled = false;

            playerActions.Remove(lastPlayerAction);
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
