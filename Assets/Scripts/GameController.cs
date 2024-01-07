using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_treasuresA;
    public GameObject m_treasuresB;

    public GameObject m_sonSpritePrefab;
    public GameObject m_teenagerSpritePrefab;
    public GameObject m_fatherSpritePrefab;
    public GameObject m_grandfatherSpritePrefab;

    public GameObject m_sonSprite;
    public GameObject m_teenagerSprite;
    public GameObject m_fatherSprite;
    public GameObject m_grandfatherSprite;

    //public PlayerController m_player;
    public SpriteController_Child m_childController;
    public SpriteController_Father m_fatherController;
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
    public bool m_sonAtStartArea = false;
    public bool m_sonAtFatherArea = false;
    public bool m_fatherAtPyramid = false;
    public bool m_fatherAtStartArea = false;
    public bool m_grandfatherAtStartArea = false;

    public void Start()
    {
        //m_player.m_gameControllerReportYoungAdult = PlayerReachesYoungAdult;
        //m_player.m_gameControllerReportFullAdult = PlayerReachesFullAdult;
        //m_player.m_gameControllerReportPlayerAtPyramid = SonIsAtPyramid;
        //m_player.m_gameControllerReportPlayerAtStartingArea = SonIsAtStartArea;

        if (m_grandfatherAtStartArea)
            InitGrandfather(false);

        InitFather();
        InitChild();
    }


    public void InitChild(bool initAsPlayer = false)
    {
        m_sonSprite = Instantiate(m_sonSpritePrefab, Vector3.zero, Quaternion.identity, m_world.transform);
        m_childBornParticles.GetComponent<ParticleSystem>().Play();
        if (initAsPlayer)
        {
            m_sonSprite.GetComponent<SpriteDriver_Child>().enabled = false;
            m_sonSprite.GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            m_sonSprite.GetComponent<SpriteDriver_Child>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
            m_childController = m_sonSprite.GetComponent<SpriteDriver_Child>().m_spriteController as SpriteController_Child;
        }

        m_childController.GetComponent<SpriteController_Child>().m_reportDamage = ChildTookDamage;
        m_childController.GetComponent<SpriteController_Child>().m_reportAtStart = SonIsAtStartArea;
        m_childController.GetComponent<SpriteController_Child>().m_reportAtPyramid = SonIsAtPyramid;
        m_childController.GetComponent<SpriteController_Child>().m_reportAgedOut = SpriteReportsAgedOut;
    }
    public void InitTeen(bool initAsPlayer = false, SpriteController_Child spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_teenagerSprite = Instantiate(m_teenagerSpritePrefab, Vector3.zero, rotation, m_world.transform);
        if (initAsPlayer)
        {
            m_teenagerSprite.GetComponent<SpriteDriver_Teenager>().enabled = false;
            m_teenagerSprite.GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            m_teenagerSprite.GetComponent<SpriteDriver_Teenager>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }
    }

    public void InitFather(bool initAsPlayer = false)
    {
        m_fatherSprite = Instantiate(m_fatherSpritePrefab, Vector3.zero, Quaternion.identity, m_world.transform);
        if (initAsPlayer)
        {
            m_fatherSprite.GetComponent<SpriteDriver_Father>().enabled = false;
            m_fatherSprite.GetComponent<PlayerController>().enabled = true;
            //m_fatherController = m_fatherSprite.GetComponent<PlayerController>().m_spriteController as SpriteController_Father;
        }
        else
            m_fatherController = m_fatherSprite.GetComponent<SpriteDriver_Father>().m_spriteController as SpriteController_Father;

        m_fatherController.m_reportFatherHasPlayerInArea = FatherReportsPlayerEnteredArea;
        m_fatherController.m_reportFatherReachedStartArea = FatherReportsReachesPyramid;
        m_fatherController.m_reportFatherReachedStartArea = FatherReachesStartArea;
    }
    public void InitGrandfather(bool initAsPlayer = false)
    {
        m_grandfatherSprite = Instantiate(m_fatherSpritePrefab, Vector3.zero, Quaternion.identity, m_world.transform);
        m_grandfather.GetComponent<Grandfather>().m_reportGrowOldMoveToTargetDone = FatherAsGrandfatherMovesToPlayerDone;
        if (initAsPlayer)
        {
            m_grandfatherSprite.GetComponent<SpriteDriver_Father>().enabled = false;
            m_grandfatherSprite.GetComponent<PlayerController>().enabled = true;
        }
    }

    public void SpriteReportsAgedOut(I_SpriteController i_spriteController)
    {
        switch (i_spriteController)
        {
            case SpriteController_Child child:
                Debug.Log($"It's a child!");
                InitTeen(((SpriteController_Child)i_spriteController).m_spriteHandler.gameObject.GetComponent<PlayerController>().enabled, (SpriteController_Child)i_spriteController);
                i_spriteController.DestroySelf();
                break;
            case SpriteController_Teenager teenager:
                Debug.Log($"It's a teenager!");
                break;
            case SpriteController_Father adult:
                Debug.Log($"It's an adult!");
                break;
        }
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
            //m_player.GetComponent<I_Hurtable>().Heal();
            StopCoroutine("HealAfterSeconds");
        }
        yield return null;
    }

    public void FatherReportsReachesPyramid()
    {
        m_fatherAtPyramid = true;
        Debug.Log("Father at pyramid");
        if (m_playerAtPyramid)
            PlayFatherBuriesGrandfatherAnim();
    }
    public void SonIsAtPyramid(bool b)
    {
        Debug.Log("Son at pyramid: "+b);
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
        m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
    }

    public void FatherPyramidAnimDone()
    {
        m_fatherController.RestoreHandlerSpeed();
        m_fatherController.GetComponent<Animator>().Play("Walk");
        m_treasuresB.SetActive(true);
        m_fatherHeart.SetActive(true);
        m_fatherAtPyramid = false;
    }
    public void FatherReachesStartArea()
    {
        m_fatherAtStartArea = true;
        Debug.Log("Father at StartArea");
        if (m_sonAtStartArea)
            PlayFatherGrowsOldAnim();
    }
    public void SonIsAtStartArea(bool b)
    {
        Debug.Log("Player at StartArea: "+b);
        m_sonAtStartArea = b;
        if(m_sonAtStartArea && m_fatherAtStartArea)
            PlayFatherGrowsOldAnim();
    }
    public void SonIsAtFatherArea(bool b)
    {
        Debug.Log("Player at FatherArea: "+b);
        m_sonAtStartArea = b;
        if(m_sonAtStartArea && m_fatherAtStartArea)
            PlayFatherGrowsOldAnim();
    }

    public void PlayFatherGrowsOldAnim()
    {
        m_grandfather.transform.SetParent(m_world.transform);
        Debug.Log("Father and Player at StartArea");
        m_fatherController.GetComponent<Animator>().Play("FatherGrowOld");
        EnterCinematicMode();
    }

    public void FatherGrowOldAnimDone()
    {
        m_grandfather.SetActive(true);
        m_grandfatherEye.GetComponent<Animator>().Play("Idle");
        m_grandfather.transform.position = m_fatherController.transform.position;
        m_fatherController.gameObject.SetActive(false);
        m_treasuresA.SetActive(true);
        m_fatherAtStartArea = false;
        //m_player.BecomeFather();
        //m_grandfatherAtStartArea = true;
        //m_player.GetOlder();
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
        EnterCinematicMode();
    }

    public void PlayerBuriesFatherPyramidAnimDone()
    {
        //m_player.GetComponent<Animator>().Play("Walk");
        m_treasuresB.SetActive(true);
        m_fatherHeart.SetActive(true);
        m_fatherAtPyramid = false;
    }

    public void FatherAsGrandfatherMovesToPlayer()
    {
        Debug.Log("Grandfather and Player at StartArea");
        //m_grandfather.GetComponent<Grandfather>().ActivateMoveToTarget(m_player.gameObject);
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

    public void EnterCinematicMode()
    {
        //m_player.GetComponent<PlayerController>().DisablePlayerControls();
    }
    public void ExitCinematicMode()
    {
        //m_player.GetComponent<PlayerController>().EnablePlayerControls();
    }

    public void ChildTookDamage()
    {

    }
}
