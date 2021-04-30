using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AK74 : Gun
{
    private void Update()
    {
        if (photonView.IsMine)
        {
            Aim();
            fireSpeedTimer -= Time.deltaTime;
            if (Input.GetMouseButton(0) && fireSpeedTimer < 0)
            {
                fireSpeedTimer = fireSpeed;
                Fire();
            }
        }
    }
    public new void Fire()
    {
        localPlayer.photonView.RPC("SetFireTrigger", RpcTarget.All, localPlayer.photonView.ViewID);
        RaycastHit distHit;
        if (Physics.Raycast(myCam.ScreenPointToRay(new Vector3(Screen.width / 2 + Random.Range(-1f, 1f) * bulletSpread, Screen.height / 2 + Random.Range(-1f, 1f) * bulletSpread, 0)), out distHit, Range, ~ignoredMask))
        {
            //Debug.Log(distHit.transform.name);
            shootTarget = distHit.point;
            if (distHit.transform.tag == "OtherTeam" && !GameManager.instance.gameOver)
            {
                StartCoroutine(localPlayer.ShowHitMarker(fireSpeed));
                distHit.transform.GetComponent<Health>().TakeDamage(gunDamage, distHit.transform.gameObject.GetPhotonView().ViewID, localPlayer.teamNum);
            }
        }
        else
            shootTarget = myCam.ScreenToWorldPoint(new Vector3(Screen.width / 2 + Random.Range(-1f, 1f) * bulletSpread, Screen.height / 2 + Random.Range(-1f, 1f) * bulletSpread, Range));
        //Debug.DrawLine(transform.position, shootTarget, Color.magenta, 999);
        localPlayer.CreateBulletTrailLine(shootTarget);
    }
    public new void Aim()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bulletSpread /= bulletSpreadReduction;
            crossHair.sizeDelta = new Vector2(crossHair.sizeDelta.x * zoomedCrossHairSize, crossHair.sizeDelta.y * zoomedCrossHairSize);
        }
        if (Input.GetMouseButton(1))
        {
            myCam.fieldOfView = localPlayer.zoomedFOV;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            bulletSpread *= bulletSpreadReduction;
            crossHair.sizeDelta = new Vector2(crossHair.sizeDelta.x / zoomedCrossHairSize, crossHair.sizeDelta.y / zoomedCrossHairSize);
            if (localPlayer.anim.GetBool("Sprinting"))
                myCam.fieldOfView = localPlayer.sprintFOV;
            else
                myCam.fieldOfView = localPlayer.normalFOV;
        }
    }
}
