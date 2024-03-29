using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> m_hearts;

    public Pyramid m_pyramid;
    public GameObject[] m_treasuresA;
    public GameObject[] m_treasuresB;
    public GameObject m_treasurePointer;

    public GameObject m_childSpritePrefab;
    public GameObject m_teenagerSpritePrefab;
    public GameObject m_fatherSpritePrefab;
    public GameObject m_grandfatherSpritePrefab;

    public GameObject m_childHandler;
    public GameObject m_teenagerHandler;
    public GameObject m_fatherHandler;
    public GameObject m_grandfatherHandler;

    
    public SpriteController_Child m_childController;
    public SpriteController_Teenager m_teenagerController;
    public SpriteController_Father m_fatherController;
    public SpriteController_Grandfather m_grandfatherController;
    public GameObject m_world;
    public CameraController m_camera;

    // Milestones
    public bool m_grandfatherDies = false;
    public bool m_fatherBecomingGrandfather = false;
    public bool m_fatherNowGrandfatherDies = false;
    public bool m_playerBecomesFather = false;
    public bool m_playerBecomesGrandfather = false;
    public bool m_fatherCarryingGrandfather = false;
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

    public bool m_speedThroughAnims = false;
    public float m_speedThroughModifier = 30;

    public void Start()
    {
        InitFather();
        //InitFather(true);
        if (m_grandfatherAtStartArea)
        {
            InitGrandfather(false, m_fatherController);
            //InitGrandfather(true, m_fatherController);
            m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
            m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
            m_fatherCarryingGrandfather = true;
        }
        InitChild(true, m_fatherController);
        //m_pyramid.m_blockCount = 2;
        //m_sonIsReadyToBeTeenager = true;
        //m_camera.m_zoomDistancesIndex = m_camera.m_zoomDistances.Length - 2;
        //m_camera.ZoomOut();
    }

    public void InitChild(bool initAsPlayer = false, SpriteController_Father spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_childHandler = Instantiate(m_childSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_childController = m_childHandler.GetComponent<SpriteDriver_Child>().m_spriteController;
        m_sonAtFatherArea = true;
        if (initAsPlayer)
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().enabled = false;
            m_childHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_childController.gameObject;
            m_childHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
        }
        else
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }

        m_childController.GetComponent<HurtableCollider>().m_reportHealthChangedAndIsNow = SonHealthChangedAndIsNow;
        m_childController.GetComponent<SpriteController_Child>().m_reportAtStart = SonReportsReachesStartArea;
        m_childController.GetComponent<SpriteController_Child>().m_reportAtPyramid = SonReportsReachesPyramid;
        m_childController.GetComponent<SpriteController_Child>().m_reportGotOlder = SpriteReportsGotOlder;
        m_childController.GetComponent<SpriteController_Child>().m_reportGotTreasure = SonGotTreasure;
        if (m_speedThroughAnims)
        {
            m_childController.m_speed = m_speedThroughModifier;
        }
    }
    public void InitTeenager(bool initAsPlayer = false, SpriteController_Child spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_teenagerHandler = Instantiate(m_teenagerSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_teenagerController = m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_spriteController;
        if (initAsPlayer)
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled = false;
            m_teenagerHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_teenagerController.gameObject;
            m_teenagerHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
            m_camera.m_zoomDistancesIndex = 4;
            m_camera.ZoomOut();
        }
        else
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }

        m_sonIsReadyToBeTeenager = false;
        m_sonIsTeenager = true;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_innerHandler.GetComponent<HurtableCollider>().m_reportHealthChangedAndIsNow = SonHealthChangedAndIsNow;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportAtStart = SonReportsReachesStartArea;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportAtPyramid = SonReportsReachesPyramid;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportGotOlder = SpriteReportsGotOlder;
        m_teenagerController.GetComponent<SpriteController_Teenager>().m_reportGotTreasure = SonGotTreasure;
        if (m_speedThroughAnims)
        {
            m_teenagerController.m_speed = m_speedThroughModifier;
        }
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
            m_camera.m_playerTarget = m_fatherController.gameObject;
            m_camera.PlayerBecomesAdult();
            m_fatherHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
            m_camera.m_zoomDistancesIndex = 7;
            m_camera.ZoomOut();
            m_camera.m_lookAhead = 7;
        }
        else
        {
            m_fatherHandler.GetComponent<SpriteDriver_Father>().m_treasurePointer = m_treasurePointer;
        }

        m_sonIsReadyToBeAdult = false;
        m_sonIsFullAdult = true;
        m_sonIsTeenager = false;
        m_fatherController.m_reportFatherHasSonInArea = FatherReportsSonEnteredArea;
        m_fatherController.m_reportFatherHasGrandfatherInArea = FatherReportsGrandfatherEnteredArea;
        m_fatherController.m_reportFatherReachedPyramid = FatherReportsReachesPyramid;
        m_fatherController.m_reportPlacedGrandfatherdAnimDone = FatherReportsPlacedGrandfatherAnimDone;
        m_fatherController.m_reportFatherReachedStartArea = FatherReportsReachesStartArea;
        m_fatherController.m_reportPyramidAnimDone = FatherAtPyramidAnimDone;
        m_fatherController.m_reportGrowOldAnimDone = FatherGrowOldAnimDone;
        //Debug.Log($"InitFather finished");

        if (m_speedThroughAnims)
        {
            m_fatherController.m_walkSpeed = m_speedThroughModifier;
            m_fatherController.ContinueWalking();
        }
    }

    public void InitGrandfather(bool initAsPlayer = false, SpriteController_Father spriteController = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_grandfatherHandler = Instantiate(m_grandfatherSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_grandfatherController = m_grandfatherHandler.GetComponent<SpriteDriver_Grandfather>().m_spriteController;
        if (initAsPlayer)
        {
            m_grandfatherHandler.GetComponent<SpriteDriver_Grandfather>().enabled = false;
            m_grandfatherHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_grandfatherController.gameObject;
            m_grandfatherHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
        }
        m_grandfatherController.GetComponent<SpriteController_Grandfather>().m_reportGrowOldMoveToTargetDone = GrandfatherMovesToAdultSonDone;
        m_grandfatherController.GetComponent<SpriteController_Grandfather>().m_reportRevealPyramid = GrandfatherReportsRevealPyramidAnim;
        m_grandfatherController.GetComponent<SpriteController_Grandfather>().m_reportRevealPyramidDone = GrandfatherReportsRevealPyramidAnimDone;
    }

    public void SpriteReportsGotOlder(I_SpriteController i_spriteController)
    {
        switch (i_spriteController)
        {
            case SpriteController_Child child:
                if (m_childHandler.GetComponent<PlayerController>().enabled)
                    m_camera.ZoomOut();
                if (m_childController.m_growthCount == 3)
                    m_sonIsReadyToBeTeenager = true;
                break;
            case SpriteController_Teenager teenager:
                if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
                    m_camera.ZoomOut();
                if (m_teenagerController.m_growthCount == 3)
                    m_sonIsReadyToBeAdult = true;
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
        m_fatherAtStartArea = true;
        m_teenagerController.DestroySelf();
    }

    public void AdultBecomesOld()
    {
        InitGrandfather((m_fatherController).m_spriteHandler.gameObject.GetComponent<PlayerController>().enabled, m_fatherController);
        m_fatherController.DestroySelf();
        m_fatherBecomingGrandfather = false;
    }

    public void FatherReportsSonEnteredArea(bool b)
    {
        m_sonAtFatherArea = b;

        // Father at pyramid buries Grandfather and Child becomes Teenager
        if (m_sonIsReadyToBeTeenager && m_fatherCarryingGrandfather)
        {
            if(m_fatherAtPyramid && m_grandfatherHandler != null)
                m_grandfatherController.BeginRevealPyramidAnim();
        }
        if(m_sonAtFatherArea && m_fatherCarryingGrandfather && !m_grandfatherController.m_spriteHandler.GetComponent<PlayerController>().enabled)
        {
            m_grandfatherController.Action();
        }
    }

    public void FatherReportsGrandfatherEnteredArea(bool grandfatherIsAtFather)
    {
        m_grandfatherAtFatherArea = grandfatherIsAtFather;

        // Father at Grandfather picks him up
        if (m_grandfatherAtFatherArea && m_grandfatherHandler != null)
        {
            GrandfatherMovesToAdultSon();
        }
    }

    public void FatherReportsReachesPyramid(bool fatherAtPyramid)
    {
        m_fatherAtPyramid = fatherAtPyramid;

        if(m_fatherAtPyramid)
        {
            PutFatherIntoCinematicMode();
        }
        else
        {
            m_pyramid.HidePyramid();
            foreach(GameObject g in m_treasuresA)
            {
                g.SetActive(true);
            }
        }
        if (m_fatherCarryingGrandfather && m_sonIsReadyToBeTeenager && m_fatherAtPyramid && m_grandfatherHandler != null)
            m_grandfatherController.BeginRevealPyramidAnim();
    }

    public void FatherReportsPlacedGrandfatherAnimDone()
    {
        m_pyramid.AddBlock();
    }

    public void GrandfatherReportsRevealPyramidAnim()
    {
        m_pyramid.gameObject.SetActive(true);
        m_pyramid.RevealPyramid();
    }
    public void GrandfatherReportsRevealPyramidAnimDone()
    {
        PlayFatherBuriesGrandfatherSequence();
    }
    public void HidePyramid()
    {
        m_pyramid.gameObject.SetActive(false);
    }
    public void SonReportsReachesPyramid(bool b)
    {
        m_sonAtPyramid = b;
        if (m_sonIsReadyToBeTeenager && m_fatherAtPyramid && m_sonAtPyramid && m_grandfatherHandler != null && m_fatherCarryingGrandfather)
            m_grandfatherController.BeginRevealPyramidAnim();
    }

    public void PlayFatherBuriesGrandfatherSequence()
    {
        Debug.Log("Called PlayFatherBuriesGrandfatherAnim");
        if(m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        m_grandfatherAtFatherArea = false;
        m_grandfatherHandler.SetActive(false);
        if(m_speedThroughAnims)
            m_fatherController.GetComponent<Animator>().speed = m_speedThroughModifier;
        if(m_fatherCarryingGrandfather)
        {
            if(m_pyramid.m_powerState == Pyramid.POWER_STATE.GOLD && m_grandfatherHandler.GetComponent<PlayerController>().enabled)
            {
                m_camera.EnterCinematicMode(m_pyramid.GetNextBlock());
                m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfatherHeartAndMind");
                m_playerDies = true;
            }
            else
                m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
            m_grandfatherController.DestroySelf();
        }
        else
        {
            m_fatherController.GetComponent<Animator>().Play("FatherAtEmptyPyramidNoGrandfather");
        }
        m_childController.Idle();
        if (m_childHandler.GetComponent<SpriteDriver_Child>().enabled)
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().DisableControls();
        }
        else
        {
            m_childHandler.GetComponent<PlayerController>().DisablePlayerControls();
        }
    }

    public void FatherAtPyramidAnimDone()
    {
        m_fatherController.ContinueWalking(); 
        if (m_playerDies)
        {
            m_camera.EnterCinematicEndingMode();
        }
        else
        {
            m_pyramid.HidePyramid();
            m_camera.ExitCinematicMode();
            foreach (GameObject g in m_treasuresB)
            {
                g.SetActive(true);
            }
        }
        ChildBecomesTeenager();
        if (m_fatherCarryingGrandfather)
            m_fatherCarryingGrandfather = false;
    }
    public void FatherReportsReachesStartArea(bool b)
    {
        m_fatherAtStartArea = b;
        if (m_fatherAtStartArea && m_sonIsReadyToBeAdult)
        {
            m_fatherController.Idle();

            if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
                m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        }
        if(!m_fatherAtStartArea && m_childController == null && !m_fatherBecomingGrandfather)
        {
            InitChild(false, m_fatherController);
            m_sonIsFullAdult = false;
        }
        //Debug.Log("Father at StartArea");
        if (m_sonAtStartArea && m_fatherAtStartArea && m_sonIsReadyToBeAdult)
            PlayFatherGrowsOldSequence();
    }

    public void SonReportsReachesStartArea(bool b)
    {
        //Debug.Log("Son at StartArea: " + b);
        m_sonAtStartArea = b;
        if (m_sonAtStartArea && m_fatherAtStartArea && m_sonIsReadyToBeAdult)
            PlayFatherGrowsOldSequence();
    }

    public void PlayFatherGrowsOldSequence()
    {
        m_fatherBecomingGrandfather = true;
        //Debug.Log("Called PlayFatherGrowsOldSequence");
        m_fatherController.GetComponent<Animator>().Play("FatherGrowOld");
        if (m_teenagerHandler != null)
        {
            if (m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled)
            {
                m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().DisableControls();
            }
            else
            {
                m_teenagerHandler.GetComponent<PlayerController>().DisablePlayerControls();
            }
        }
    }

    public void FatherGrowOldAnimDone()
    {
        m_fatherController.gameObject.SetActive(false);
        AdultBecomesOld();
        TeenagerBecomesAdult();

        if (m_fatherController != null && m_grandfatherAtFatherArea)
        {
            GrandfatherMovesToAdultSon();
        }
    }

    public void GrandfatherMovesToAdultSon()
    {
        m_fatherController.Idle();
        //Debug.Log("Grandfather and Adult Son at StartArea");
        m_grandfatherController.BeginMoveToTarget(m_fatherController.gameObject);
        m_sonAtFatherArea = false;
    }
    public void GrandfatherMovesToAdultSonDone()
    {
        //Debug.Log("GrandfatherMovesToPlayerDone");
        
        ExitCinematicMode();
        m_fatherController.ContinueWalking();
        m_fatherCarryingGrandfather = true;
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
        //Debug.Log("PutFatherIntoCinematicMode");
        if(m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            //Debug.Log("Driver detected");
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        }
        m_fatherController.Idle();
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

    public void SonGotTreasure()
    {
        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        }
    }

    public void SonHealthChangedAndIsNow(int health)
    {
        if (health == 3)
        {
            for (int ii = 0; ii < 3; ++ii)
            {
                m_hearts[ii].SetActive(true);
            }
        }
        else
            m_hearts[health].SetActive(false);
    }
}
