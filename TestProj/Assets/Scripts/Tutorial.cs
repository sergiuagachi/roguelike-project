using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    
    public GameObject objectToDraw1;
    public GameObject objectToDraw2;
    public GameObject objectToDraw3;
    
    public Text description;
    public Text endTutorial;

    private SpriteRenderer _spr1;
    private SpriteRenderer _spr2;
    private SpriteRenderer _spr3;

    public Sprite playerSprite;
    
    public Sprite enemy1;
    public Sprite enemy2;
    public Sprite enemy3;

    public Sprite sword;
    public Sprite pickaxe;
    public Sprite shovel;

    public Sprite food1;
    public Sprite food2;

    public Sprite stairsUp;
    public Sprite stairsDown;
    
    public Sprite dirtWall;
    public Sprite stoneWall;
    
    public Sprite outerWallSprite;
    public Sprite interiorWallSprite;
    
    public Sprite chest;
    public Sprite key;
    
    public Sprite checkpoint;

    public Sprite sign;

    public Sprite exit;
    
    private int _objectToShow;
    private bool _isReady = true;
    
    private void Start() {
        _spr1 = objectToDraw1.GetComponent<SpriteRenderer>();
        _spr2 = objectToDraw2.GetComponent<SpriteRenderer>();
        _spr3 = objectToDraw3.GetComponent<SpriteRenderer>();
        
        _spr1.sprite = null;
        _spr2.sprite = playerSprite;
        _spr3.sprite = null;
        description.text = "This is you"; 
    }

    private void Update() {
        if(_isReady && Input.anyKey) {
            _isReady = false;
            _objectToShow++;
            StartCoroutine(NextItem());
        }
    }

    private IEnumerator NextItem() {
        yield return new WaitForSeconds(0.2f);
        
        switch (_objectToShow) {
            case 1:
                _spr1.sprite = enemy1;
                _spr2.sprite = enemy2;
                _spr3.sprite = enemy3;
                description.text = "They are your enemies";
                break;
            case 2:
                _spr1.sprite = pickaxe;
                _spr2.sprite = sword;
                _spr3.sprite = shovel;
                description.text = "These are used to pass obstacles";
                break;
            case 3:
                _spr1.sprite = food1;
                _spr2.sprite = null;
                _spr3.sprite = food2;
                description.text = "This is food, it heals you";
                break;
            case 4:
                _spr1.sprite = stairsUp;
                _spr2.sprite = null;
                _spr3.sprite = stairsDown;
                description.text = "You will use this to go up and down the tower";
                break;
            case 5:
                _spr1.sprite = stoneWall ;
                _spr2.sprite = null;
                _spr3.sprite = dirtWall;
                description.text = "These block your movement and can be destroyed with a tool";
                break;
            case 6:
                _spr1.sprite = outerWallSprite;
                _spr2.sprite = null;
                _spr3.sprite = interiorWallSprite;
                description.text = "These block your movement and can't be destroyed";
                break;
            case 7:
                _spr1.sprite = chest;
                _spr2.sprite = null;
                _spr3.sprite = key;
                description.text = "You will find about these once you start the game";
                break;
            case 8:
                _spr1.sprite = null;
                _spr2.sprite = checkpoint;
                _spr3.sprite = null;
                description.text = "This is a checkpoint, you respawn here when you die";
                break;
            case 9:
                _spr1.sprite = null;
                _spr2.sprite = sign;
                _spr3.sprite = null;
                description.text = "You get your info from these";
                break;
            case 10:
                _spr1.sprite = null;
                _spr2.sprite = exit;
                _spr3.sprite = null;
                description.text = "You exit from here";
                break;
            case 11:
                _spr1.sprite = null;
                _spr2.sprite = null;
                _spr3.sprite = null;
                description.text = "";
                endTutorial.text = "Press any key to start the intro";
                break;
            default:
                SceneManager.LoadScene("Intro", LoadSceneMode.Single);
                break;
        }

        _isReady = true;
    }
}
