using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviourPun
{
    [Header("Components")]
    public Rigidbody rig;
    public Animator anim;
    public GameObject hitMarker;
    public SkinnedMeshRenderer myModel;
    public GameObject myHipObj;
    [HideInInspector]public TeamManager myTeam;
    public AudioManager mySoundManager;

    [Header("Gun Stuff")]
    public Transform startPoint;
    public Transform endPoint;
    public GameObject gunContainer;
    GameObject curGun;

    [Header("Speeds")]
    public float walkSpeed;
    public float sprintSpeed;
    float moveSpeed;
    public float slideSpeedBoost;
    public float jumpForce;

    [Header("Camera Stats")]
    public Camera myCam;
    public float normalFOV;
    public float zoomedFOV;
    public float sprintFOV;

    [Header("UI")]
    public RectTransform crossHair;
    public GameObject myCanvas;
    public GameObject hitMarkerUIImage;
    public GameObject WinDisplay;
    public TextMeshProUGUI WinText;
    public Slider blueSlider;
    public Slider redSlider;
    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI redScoreText;
    public GameObject pauseMenu;

    [Header("Animation Rig")]
    public Rig LeftArmRig;
    public Rig ribsRig;

    [Header("Team Stuff")]
    public int teamNum;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            myCanvas.SetActive(false);
            transform.Find("3rd Person Over The Shoulder").gameObject.SetActive(false);
            GetComponent<PlayerScript>().enabled = false;
            return;
        }
        else
        {
            moveSpeed = walkSpeed;
            GameManager.instance.localPlayer = this;
            GameManager.instance.UpdateScoreUI(GameManager.instance.bluePoints, GameManager.instance.redPoints);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (GameManager.instance.gameOver)
                DisplayeWinText(GameManager.instance.wonTeam);

            myTeam = TeamManager.instance;
            if (myTeam.teamNum == 0)
                myHipObj.layer = 10;
            else
                myHipObj.layer = 11;
            SpawnGun(GameManager.instance.curPlayerGun.name);
            photonView.RPC("Initialize", RpcTarget.All, photonView.ViewID, myTeam.teamNum);
            Invoke("LateStart", 1);
        }
    }
    [PunRPC]
    void Initialize(int id, int teamNum)
    {
        PlayerScript player = PhotonView.Find(id).GetComponent<PlayerScript>();
        player.myModel.material = TeamManager.instance.materials[teamNum];
        player.teamNum = teamNum;
        GameManager.instance.playerObjects.Add(PhotonView.Find(id).gameObject);
    }
    void LateStart()
    {
        foreach(GameObject obj in GameManager.instance.playerObjects)
            if (obj.GetComponent<PlayerScript>().teamNum == teamNum)
                obj.tag = "MyTeam";
    }

    //Gun Stuff
    public void SpawnGun(string name)
    {//need to line up the gun start and end pos with our hand IKs
        if(curGun != null)
            PhotonNetwork.Destroy(curGun);
        GameObject obj = PhotonNetwork.Instantiate(name, gunContainer.transform.position, Quaternion.identity);
        curGun = obj;
        obj.name = obj.name.Replace("(Clone)", "");
        StartCoroutine(DelayedSetPos(obj));
        photonView.RPC("SetGunPos", RpcTarget.All, photonView.ViewID, obj.GetPhotonView().ViewID);
        GameManager.instance.curPlayerGun = obj;
    }
    IEnumerator DelayedSetPos(GameObject obj)
    {
        yield return new WaitForSeconds(.5f);
        startPoint.position = obj.GetComponent<Gun>().start.position;
        endPoint.position = obj.GetComponent<Gun>().end.position;
    }
    [PunRPC]
    void SetGunPos(int playerID, int gunID)
    {
        Transform trans = PhotonView.Find(gunID).transform;
        trans.parent = PhotonView.Find(playerID).GetComponent<PlayerScript>().gunContainer.transform;
        //trans.localPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                pauseMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            //pauseMenu.SetActive(pauseMenu.activeSelf ? false : true);
        }
    }
    //Movement Stuff
    void Move()
    {
        //get input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //calculate a direction in were we are facing
        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;
        //set that as our velocity
        rig.velocity = dir;

        if (Input.GetKeyDown(KeyCode.T))
        {
            photonView.RPC("SetDanceTrigger", RpcTarget.All, photonView.ViewID);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LeftArmRig.weight = 1;
            RaycastHit[] hits = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Vector3.down, 1);
            foreach(RaycastHit hit in hits)
            {
                if (hit.transform.gameObject != gameObject)
                {
                    rig.AddForce(Vector3.up * jumpForce);
                    photonView.RPC("SetJumpTrigger", RpcTarget.All, photonView.ViewID);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            photonView.RPC("SetSlideTrigger", RpcTarget.All, photonView.ViewID);
            StartCoroutine(SlideSpeedBoost());
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            LeftArmRig.weight = 1;
            myCam.fieldOfView = sprintFOV;
            moveSpeed = sprintSpeed;
            anim.SetBool("Sprinting", true);
            photonView.RPC("SetSprintTrigger", RpcTarget.All, photonView.ViewID);
        }
        else if (!anim.GetBool("Walking"))
        {
            myCam.fieldOfView = normalFOV;
            moveSpeed = walkSpeed;
            anim.SetBool("Sprinting", false);
        }
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (!anim.GetBool("Walking"))
            {
                anim.SetBool("Walking", true);
                photonView.RPC("SetWalkTrigger", RpcTarget.All, photonView.ViewID);
            }
        }
        else
            anim.SetBool("Walking", false);
    }
    IEnumerator SlideSpeedBoost()
    {
        if (anim.GetBool("Sprinting"))
            moveSpeed = sprintSpeed * slideSpeedBoost;
        else if (anim.GetBool("Walking"))
            moveSpeed = walkSpeed * slideSpeedBoost;
        LeftArmRig.weight = 0;

        yield return new WaitForSeconds(1);

        if (anim.GetBool("Sprinting"))
            moveSpeed = sprintSpeed;
        else if (anim.GetBool("Walking"))
            moveSpeed = walkSpeed;
        LeftArmRig.weight = 1;
    }

    //Animations
    [PunRPC]
    public void SetJumpTrigger(int id)
    {
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().anim.SetTrigger("JumpTrigger");
    }
    [PunRPC]
    public void SetWalkTrigger(int id)
    {
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().anim.SetTrigger("StartWalking");
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().LeftArmRig.weight = 1;
    }
    [PunRPC]
    public void SetSprintTrigger(int id)
    {
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().anim.SetTrigger("StartSprint");
    }
    [PunRPC]
    public void SetSlideTrigger(int id)
    {
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().anim.SetTrigger("StartSlide");
    }
    [PunRPC]
    public void SetDanceTrigger(int id)
    {
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().anim.SetTrigger("StartDance");
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().LeftArmRig.weight = 0;
    }
    [PunRPC]
    public void SetFireTrigger(int id)
    {
        PhotonView.Find(id).gameObject.GetComponent<PlayerScript>().anim.SetTrigger("ShootMoveing");
    }

    //UI Stuff
    public void DisplayeWinText(int teamNum)
    {
        crossHair.gameObject.SetActive(false);
        if(teamNum == 0)
        {
            WinText.color = Color.blue;
            WinText.text = "Blue Team Wins!";
        }
        else
        {
            WinText.color = Color.red;
            WinText.text = "Red Team Wins!";
        }
        WinDisplay.SetActive(true);
    }

    //Fire Stuff
    public void CreateBulletTrailLine(Vector3 target)
    {
        GameObject obj = PhotonNetwork.Instantiate("LineVisual", endPoint.position, Quaternion.identity);
        photonView.RPC("ChangeLinePos", RpcTarget.All, obj.GetPhotonView().ViewID, photonView.ViewID, endPoint.position, target, curGun.name);
    }
    [PunRPC]
    public void ChangeLinePos(int id, int playerID, Vector3 start, Vector3 end, string name)
    {
        PhotonView.Find(playerID).GetComponent<PlayerScript>().mySoundManager.Play(name);
        PhotonView.Find(id).GetComponent<LineRenderer>().SetPosition(1, start);
        PhotonView.Find(id).GetComponent<LineRenderer>().SetPosition(0, end);
    }
    public IEnumerator ShowHitMarker(float fireSpeed)
    {
        hitMarkerUIImage.SetActive(true);
        fireSpeed = Mathf.Clamp(fireSpeed, .1f, 2);
        yield return new WaitForSeconds(fireSpeed * .8f);
        hitMarkerUIImage.SetActive(false);
    }
}
