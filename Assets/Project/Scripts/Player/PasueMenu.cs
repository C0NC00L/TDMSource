using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PasueMenu : MonoBehaviour
{
    [Header("Tab Objects")]
    public GameObject generalTab;
    public GameObject audioTab;
    public GameObject controlsTab;

    [Header("Audio")]
    public Slider masterVol;

    [Header("Controls")]
    public Slider sensSlider;

    [Header("Objects")]
    public CameraController myCamControl;
    public GameObject ribControlObj;
    RibTargetMover ribControl;

    private void Start()
    {
        ribControl = ribControlObj.GetComponent<RibTargetMover>();
        //audio
        AudioListener.volume = PlayerPrefs.GetFloat("masterVolume", .5f);
        masterVol.value = PlayerPrefs.GetFloat("masterVolume", .5f);

       //controls
       myCamControl.sensx = PlayerPrefs.GetFloat("camSens", .5f) * 10;
       myCamControl.sensy = PlayerPrefs.GetFloat("camSens", .5f) * 10;
       ribControl.sensy = PlayerPrefs.GetFloat("camSens", .5f) * 10;

       sensSlider.value = PlayerPrefs.GetFloat("camSens", .5f);
    }

    ///////Button Tabs
    public void SwitchTab(GameObject tabToDisplay)
    {
        generalTab.SetActive(false);
        audioTab.SetActive(false);
        controlsTab.SetActive(false);
        tabToDisplay.SetActive(true);
    }


    ///////General Tab
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");
    }
    public void QuiteGame()
    {
        Application.Quit();
    }

    ///////Audio Stuff
    public void OnVolChange()
    {
        AudioListener.volume = masterVol.value;
        PlayerPrefs.SetFloat("masterVolume", masterVol.value);
        PlayerPrefs.Save();
    }

    ///////Controls
    public void OnSensChange()
    {
        ribControl.sensy = sensSlider.value * 10;
        myCamControl.sensx = sensSlider.value * 10;
        myCamControl.sensy = sensSlider.value * 10;
        myCamControl.sensy = sensSlider.value * 10;
        PlayerPrefs.SetFloat("camSens", sensSlider.value);
        PlayerPrefs.Save();
    }
}
