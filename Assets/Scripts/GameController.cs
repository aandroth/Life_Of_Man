using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_treasuresA;
    public GameObject m_treasuresB;

    public PlayerController m_player;
    public Father m_father;
    public GameObject m_fatherHead;
    public GameObject m_fatherHeart;
    public GameObject m_grandfather;
    public GameObject m_grandfatherEye;
    public GameObject m_world;
    public GameObject m_childBornParticles;

    // Milestones
    public bool m_grandfatherDies = false;
    public bool m_fatherBecomesGrandfather = false;
    public bool m_fatherNowGrandfatherDies = false;
    public bool m_playerBecomesFather = false;
    public bool m_playerBecomesGrandfather = false;
    public bool m_playerDies = false;

    public bool m_playerIsYoungAdult = false;
    public bool m_playerIsFullAdult = false;
    public bool m_playerAtPyramid = false;
    public bool m_playerAtStartArea = false;
    public bool m_playerAtFatherArea = false;
    public bool m_fatherAtPyramid = false;
    public bool m_fatherAtStartArea = false;
    public bool m_grandfatherAtStartArea = false;

    public void Start()
    {
        m_player.m_gameControllerReportYoungAdult = PlayerReachesYoungAdult;
        m_player.m_gameControllerReportFullAdult = PlayerReachesFullAdult;
        m_player.m_gameControllerReportPlayerAtPyramid = PlayerIsAtPyramid;
        m_player.m_gameControllerReportPlayerAtStartingArea = PlayerIsAtStartArea;
        m_player.m_gameControllerReportPlayerAtStartingArea = PlayerIsAtStartArea;
        m_father.m_reportFatherReachedPyramid = FatherReachesPyramid;
        m_father.m_reportFatherReachedStartArea = FatherReachesStartArea;
        //m_father.m_reportFatherHasPlayerInArea = PlayerIsAtFatherArea(bool b);
        m_father.m_reportPyramidAnimDone = FatherPyramidAnimDone;
        m_father.m_reportGrowOldAnimDone = FatherGrowOldAnimDone;
        m_grandfather.GetComponent<Grandfather>().m_reportGrowOldMoveToTargetDone = FatherAsGrandfatherMovesToPlayerDone;

    }

    public void FatherReportsPlayerEnteredArea()
    {
        if(m_grandfather != null)
        {
            m_grandfather.GetComponent<Grandfather>().EnactHealing();
            StartCoroutine(HealAfterSeconds(3f));
        }
    }

    public IEnumerator HealAfterSeconds(float _seconds)
    {
        _seconds -= Time.deltaTime;
        if (_seconds <= 0)
        {
            m_player.GetComponent<I_Hurtable>().Heal();
            StopCoroutine("HealAfterSeconds");
        }
        yield return null;

    }

    public void FatherReachesPyramid()
    {
        m_fatherAtPyramid = true;
        Debug.Log("Father at pyramid");
        if (m_playerAtPyramid)
            PlayFatherBuriesGrandfatherAnim();
    }
    public void PlayerIsAtPyramid(bool b)
    {
        Debug.Log("Player at pyramid: "+b);
        m_playerAtPyramid = b;
        if (m_playerIsYoungAdult && m_playerAtPyramid && m_fatherAtPyramid)
            PlayFatherBuriesGrandfatherAnim();
        else if(m_playerIsFullAdult && m_playerAtPyramid)
            PlayPlayerBuriesFatherAnim();
    }

    public void PlayFatherBuriesGrandfatherAnim()
    {
        Debug.Log("Called PlayFatherBuriesGrandfatherAnim");
        m_grandfather.SetActive(false);
        m_father.GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
    }

    public void FatherPyramidAnimDone()
    {
        m_father.RestoreHandlerSpeed();
        m_father.GetComponent<Animator>().Play("Walk");
        m_treasuresB.SetActive(true);
        m_fatherHeart.SetActive(true);
        m_fatherAtPyramid = false;
    }
    public void FatherReachesStartArea()
    {
        m_fatherAtStartArea = true;
        Debug.Log("Father at StartArea");
        if (m_playerAtStartArea)
            PlayFatherGrowsOldAnim();
    }
    public void PlayerIsAtStartArea(bool b)
    {
        Debug.Log("Player at StartArea: "+b);
        m_playerAtStartArea = b;
        if(m_playerAtStartArea && m_fatherAtStartArea)
            PlayFatherGrowsOldAnim();
    }
    public void PlayerIsAtFatherArea(bool b)
    {
        Debug.Log("Player at FatherArea: "+b);
        m_playerAtStartArea = b;
        if(m_playerAtStartArea && m_fatherAtStartArea)
            PlayFatherGrowsOldAnim();
    }

    public void PlayFatherGrowsOldAnim()
    {
        m_grandfather.transform.SetParent(m_world.transform);
        Debug.Log("Father and Player at StartArea");
        m_father.GetComponent<Animator>().Play("FatherGrowOld");
        EnterCinematicMode();
    }

    public void FatherGrowOldAnimDone()
    {
        m_grandfather.SetActive(true);
        m_grandfatherEye.GetComponent<Animator>().Play("Idle");
        m_grandfather.transform.position = m_father.transform.position;
        m_father.gameObject.SetActive(false);
        m_treasuresA.SetActive(true);
        m_fatherAtStartArea = false;
        m_player.BecomeFather();
        m_grandfatherAtStartArea = true;
        m_player.GetOlder();
        m_playerIsFullAdult = true;
        m_playerIsYoungAdult = false;

        //if (m_playerAtStartArea)
        //{
            FatherAsGrandfatherMovesToPlayer();
        //}
    }

    public void PlayPlayerBuriesFatherAnim()
    {
        Debug.Log("Father as Grandfather, and Player at pyramid");
        m_player.GetComponent<PlayerController>().BuryFather();
        EnterCinematicMode();
    }

    public void PlayerBuriesFatherPyramidAnimDone()
    {
        m_player.GetComponent<Animator>().Play("Walk");
        m_treasuresB.SetActive(true);
        m_fatherHeart.SetActive(true);
        m_fatherAtPyramid = false;
    }

    public void FatherAsGrandfatherMovesToPlayer()
    {
        Debug.Log("Grandfather and Player at StartArea");
        m_grandfather.GetComponent<Grandfather>().ActivateMoveToTarget(m_player.gameObject);
    }
    public void FatherAsGrandfatherMovesToPlayerDone()
    {
        Debug.Log("GrandfatherMovesToPlayerDone");
        m_fatherBecomesGrandfather = true;
        ExitCinematicMode();
    }

    public void PlayerReachesYoungAdult()
    {
        Debug.Log("Player becomes Young Adult");
        m_treasuresA.SetActive(false);
        m_fatherHead.transform.Rotate(Vector3.forward, 22);
        m_grandfatherEye.GetComponent<Animator>().Play("CloseEye");
        m_playerIsYoungAdult = true;
    }

    public void PlayerReachesFullAdult()
    {
        m_treasuresB.SetActive(false);
        m_playerBecomesFather = true;
    }

    public void ChildIsBorn()
    {
        m_childBornParticles.GetComponent<ParticleSystem>().Play();
    }

    public void EnterCinematicMode()
    {
        m_player.GetComponent<PlayerController>().DisablePlayerControls();
    }
    public void ExitCinematicMode()
    {
        m_player.GetComponent<PlayerController>().EnablePlayerControls();
    }
}
