using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> m_hearts;

    public Pyramid m_pyramid0;
    public Pyramid m_pyramid1;
    public Pyramid m_pyramid2;
    public GameObject m_treasuresA;
    public GameObject m_treasuresB;

    public GameObject m_childSpritePrefab;
    public GameObject m_teenagerSpritePrefab;
    public GameObject m_fatherSpritePrefab;
    public GameObject m_grandfatherSpritePrefab;

    public GameObject m_childHandler;
    public GameObject m_teenagerHandler;
    public GameObject m_fatherHandler;
    public GameObject m_grandfatherSprite;

    
    public SpriteController_Child m_childController;
    public SpriteController_Teenager m_teenagerController;
    public SpriteController_Father m_fatherController;
    public SpriteController_Grandfather m_grandfatherController;
    public GameObject m_fatherHead;
    public GameObject m_fatherHeart;
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

    public bool m_sonIsReadyToBeTeenager = false;
    public bool m_sonIsTeenager = false;
    public bool m_sonIsReadyToBeAdult = false;
    public bool m_sonIsFullAdult = false;
    public bool m_sonAtPyramid = false;
    public bool m_sonAtStartArea = false;
    public bool m_sonAtFatherArea = false;
    public bool m_fatherAtPyramid = false;
    public bool m_fatherAtStartArea = false;
    public bool m_grandfatherAtStartArea = false;
    public bool m_grandfatherAtFatherArea = false;

    public void Start()
    {
        InitFather();
        if (m_grandfatherAtStartArea)
        {
            InitGrandfather(false, m_fatherController);
            m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
            m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        }
        //InitChild();
        //m_childController.GetOlder();
        //m_childController.GetOlder();
        //m_childController.GetOlder();

        //InitTeenager();
        //m_teenagerController.GetOlder();
        //m_teenagerController.GetOlder();
        //m_teenagerController.GetOlder();
    }


    public void InitChild(bool initAsPlayer = false)
    {
        m_childHandler = Instantiate(m_childSpritePrefab, Vector3.zero, Quaternion.identity, m_world.transform);
        m_childBornParticles.GetComponent<ParticleSystem>().Play();
        m_childController = m_childHandler.GetComponent<SpriteDriver_Child>().m_spriteController;
        m_sonAtFatherArea = true;
        if (initAsPlayer)
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().enabled = false;
            m_childHandler.GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }

        m_childController.GetComponent<SpriteController_Child>().m_reportTookDamage = SonTookDamageAndIsNow;
        m_childController.GetComponent<SpriteController_Child>().m_reportAtStart = SonReportsReachesStartArea;
        m_childController.GetComponent<SpriteController_Child>().m_reportAtPyramid = SonReportsReachesPyramid;
        m_childController.GetComponent<SpriteController_Child>().m_reportAgedOut = SpriteReportsAgedOut;
    }
    public void InitTeenager(bool initAsPlayer = false, SpriteController_Child spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_teenagerHandler = Instantiate(m_teenagerSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_teenagerController = m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_spriteController;
        //if (initAsPlayer)
        //{
        //    m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled = false;
        //    m_teenagerHandler.GetComponent<PlayerController>().enabled = true;
        //}
        //else
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }

        m_sonIsReadyToBeTeenager = false;
        m_sonIsTeenager = true;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportTookDamage = SonTookDamageAndIsNow;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportAtStart = SonReportsReachesStartArea;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportAtPyramid = SonReportsReachesPyramid;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportAgedOut = SpriteReportsAgedOut;
    }

    public void InitFather(bool initAsPlayer = false, SpriteController_Teenager spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_fatherHandler = Instantiate(m_fatherSpritePrefab, Vector3.zero, rotation, m_world.transform); 
        m_fatherController = m_fatherHandler.GetComponent<SpriteDriver_Father>().m_spriteController;
        if (initAsPlayer)
        {
            m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled = false;
            m_fatherHandler.GetComponent<PlayerController>().enabled = true;
        }

        m_fatherController.m_reportFatherHasSonInArea = FatherReportsSonEnteredArea;
        m_fatherController.m_reportFatherHasGrandfatherInArea = FatherReportsSonEnteredArea;
        m_fatherController.m_reportFatherReachedPyramid = FatherReportsReachesPyramid;
        m_fatherController.m_reportPlacedGrandfatherdAnimDone = FatherReportsPlacedGrandfatherAnimDone;
        m_fatherController.m_reportFatherReachedStartArea = FatherReportsReachesStartArea;
        m_fatherController.m_reportPyramidAnimDone = FatherBuriesGrandfatherAnimDone;
        m_fatherController.m_reportGrowOldAnimDone = FatherGrowOldAnimDone;
    }

    public void InitGrandfather(bool initAsPlayer = false, SpriteController_Father spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_grandfatherSprite = Instantiate(m_grandfatherSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_grandfatherSprite.transform.localPosition = new Vector3(0, 83.8f, 0);
        m_grandfatherController = m_grandfatherSprite.GetComponent<SpriteDriver_Grandfather>().m_spriteController;

        m_grandfatherController.GetComponent<SpriteController_Grandfather>().m_reportGrowOldMoveToTargetDone = GrandfatherMovesToAdultSonDone;
        if (initAsPlayer)
        {
            m_grandfatherSprite.GetComponent<SpriteDriver_Grandfather>().enabled = false;
            m_grandfatherSprite.GetComponent<PlayerController>().enabled = true;
        }
    }

    public void SpriteReportsAgedOut(I_SpriteController i_spriteController)
    {
        switch (i_spriteController)
        {
            case SpriteController_Child child:
                Debug.Log("Child aged into teenager");
                m_sonIsReadyToBeTeenager = true;
                break;
            case SpriteController_Teenager teenager:
                m_sonIsReadyToBeAdult = true;
                break;
            case SpriteController_Father adult:

                break;
        }
    }

    public void ChildBecomesTeenager()
    {
        InitTeenager((m_childController).m_spriteHandler.gameObject.GetComponent<PlayerController>().enabled, m_childController);
        m_childController.DestroySelf();
    }

    public void TeenagerBecomesAdult()
    {
        InitFather((m_teenagerController).m_spriteHandler.gameObject.GetComponent<PlayerController>().enabled, m_teenagerController);
        m_teenagerController.DestroySelf();
    }

    public void AdultBecomesOld()
    {
        InitGrandfather((m_fatherController).m_spriteHandler.gameObject.GetComponent<PlayerController>().enabled, m_fatherController);
        m_fatherController.DestroySelf();
    }

    public void FatherReportsSonEnteredArea(bool b)
    {
        m_sonAtFatherArea = b;

        // Father at pyramid buries Grandfather and Child becomes Teenager
        if (m_sonIsReadyToBeTeenager && m_sonAtPyramid && m_fatherAtPyramid && m_grandfatherSprite != null)
        {
            PlayFatherBuriesGrandfatherSequence();
        }
    }

    public void FatherReportsGrandfatherEnteredArea(bool b)
    {
        m_grandfatherAtFatherArea = b;

        // Father at Grandfather picks him up
        if (m_grandfatherAtFatherArea && m_grandfatherSprite != null)
        {
            m_grandfatherController.BeginMoveToTarget(m_fatherController.gameObject);
        }
    }

    public void FatherReportsReachesPyramid(bool b)
    {
        m_fatherAtPyramid = b;

        if(m_fatherAtPyramid)
        {
            PutFatherIntoCinematicMode();
        }
        if (m_sonIsReadyToBeTeenager && m_fatherAtPyramid && m_sonAtFatherArea && m_grandfatherSprite != null)
            PlayFatherBuriesGrandfatherSequence();
    }

    public void FatherReportsPlacedGrandfatherAnimDone()
    {
        Debug.Log("Called FatherReportsPlacedGrandfatherAnimDone");
        m_pyramid0.AddBlock();
        m_pyramid1.AddBlock();
        m_pyramid2.AddBlock();
    }
    public void SonReportsReachesPyramid(bool b)
    {
        m_sonAtPyramid = b;
        if (m_sonIsReadyToBeTeenager && m_fatherAtPyramid && m_sonAtPyramid && m_grandfatherSprite != null)
            PlayFatherBuriesGrandfatherSequence();
    }

    public void PlayFatherBuriesGrandfatherSequence()
    {
        Debug.Log("Called PlayFatherBuriesGrandfatherAnim");
        m_grandfatherSprite.SetActive(false);
        m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
        if (m_childHandler.GetComponent<SpriteDriver_Child>().enabled)
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().DisableControls();
        }
        else
        {
            m_childHandler.GetComponent<PlayerController>().DisablePlayerControls();
        }
    }

    public void FatherBuriesGrandfatherAnimDone()
    {
        m_fatherController.RestoreHandlerSpeed();
        m_treasuresB.SetActive(true);
        m_fatherHeart.SetActive(true);
        ChildBecomesTeenager();
    }
    public void FatherReportsReachesStartArea(bool b)
    {
        //m_fatherAtStartArea = b;
        //if (m_fatherAtStartArea)
        //{
        //    m_fatherController.Idle();
        //}
        //Debug.Log("Father at StartArea");
        //if (m_sonAtStartArea && m_fatherAtStartArea && m_sonIsReadyToBeAdult)
        //    PlayFatherGrowsOldSequence();

        AdultBecomesOld();
    }

    public void SonReportsReachesStartArea(bool b)
    {
        Debug.Log("Son at StartArea: " + b);
        m_sonAtStartArea = b;
        if (m_sonAtStartArea && m_fatherAtStartArea && m_sonIsReadyToBeAdult)
            PlayFatherGrowsOldSequence();
    }

    public void PlayFatherGrowsOldSequence()
    {
        Debug.Log("Called PlayFatherGrowsOldSequence");
        m_fatherController.GetComponent<Animator>().Play("FatherGrowOld");
        if (m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled)
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().DisableControls();
        }
        else
        {
            m_teenagerHandler.GetComponent<PlayerController>().DisablePlayerControls();
        }
    }
    //public void PlayFatherGrowsOldAnim()
    //{
    //    m_grandfatherSprite.transform.SetParent(m_world.transform);
    //    Debug.Log("Father and Player at StartArea");
    //    m_fatherController.GetComponent<Animator>().Play("FatherGrowOld");
    //    EnterCinematicMode();
    //}

    public void FatherGrowOldAnimDone()
    {
        m_fatherController.gameObject.SetActive(false);
        AdultBecomesOld();
        //InitFather(m_teenagerController);
        m_treasuresA.SetActive(true);
        m_sonIsTeenager = false;

        if (m_sonAtFatherArea)
        {
            GrandfatherMovesToAdultSon();
            m_sonAtFatherArea = false;
        }
    }

    public void GrandfatherMovesToAdultSon()
    {
        Debug.Log("Grandfather and Player at StartArea");
        m_grandfatherController.BeginMoveToTarget(m_fatherController.gameObject);
    }
    public void GrandfatherMovesToAdultSonDone()
    {
        Debug.Log("GrandfatherMovesToPlayerDone");
        m_fatherBecomesGrandfather = true;
        ExitCinematicMode();
    }

    public void PutChildIntoCinematicMode()
    {
        if (m_childHandler.GetComponent<SpriteDriver_Child>().enabled)
        {
            //m_fatherController.m_walkSpeed = 0;
            //m_fatherController.gameObject.GetComponent<Animator>().StopPlayback();
        }
    }

    public void PutTeenagerIntoCinematicMode()
    {
        if (m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled)
        {
            //m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
            //m_fatherController.m_walkSpeed = 0;
            //m_fatherController.gameObject.GetComponent<Animator>().StopPlayback();
        }
    }

    public void PutFatherIntoCinematicMode()
    {
        Debug.Log("PutFatherIntoCinematicMode");
        if(m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            Debug.Log("Driver detected");
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
            m_fatherController.Idle();
        }
    }

    public void PutFatherOutOfCinematicMode()
    {
        if(m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            m_fatherController.ContinueWalking();
        }
    }

    public void EnterCinematicMode()
    {
        if(m_fatherHandler != null)
        {
            PutFatherIntoCinematicMode();
        }
    }
    public void ExitCinematicMode()
    {
        //m_player.GetComponent<PlayerController>().EnablePlayerControls();
    }

    public void SonTookDamageAndIsNow(int health)
    {
        m_hearts[health].gameObject.SetActive(false);
    }
}
