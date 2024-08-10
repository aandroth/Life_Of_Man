using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum LEVEL_STATE { LEVEL_1, LEVEL_2, LEVEL_3 };
    public LEVEL_STATE m_levelState = LEVEL_STATE.LEVEL_1;
    public enum CHECKPOINT_STATE { START, PLAYER_AS_TEENAGER_REACHES_PYRAMID, PLAYER_AS_FATHER_REACHES_START, PLAYER_AS_FATHER_REACHES_PYRAMID, PLAYER_AS_GRANDFATHER_REACHES_START };
    public CHECKPOINT_STATE m_checkpointState = CHECKPOINT_STATE.START;
    public List<GameObject> m_hearts;
    public List<GameObject> m_treasures;
    public GameObject m_resetOptionsButtons;
    public GameObject m_continueButton;
    public GameObject m_levelIntroCard;
    public GameObject m_levelCompleteCard;
    public IEnumerator m_reloadLevelAfterDelayCoroutine;
    public float m_reloadLevelDelay = 11f;

    public Pyramid m_pyramid;
    public GameObject[] m_treasuresA;
    public GameObject[] m_treasuresB;
    public GameObject m_treasurePointer;
    public GameObject m_tricksPointer;
    public GameObject m_enemySpawnerHandler;
    public GameObject m_growOldAreaTrigger, m_growOldAreaTriggerNextPosition;
    public GameObject m_grandfatherNpcDiesStone;

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

    public CountdownTimer m_gameTimer;
    public CameraController m_camera;
    public GameObject m_world;

    // Milestones
    public bool[] m_nextLevelUnlocked = new bool[2];
    public bool m_grandfatherDies = false;
    public bool m_fatherBecomingGrandfather = false;
    public bool m_fatherNowGrandfatherDies = false;
    public bool m_playerBecomesFather = false;
    public bool m_playerBecomesGrandfather = false;
    public bool m_fatherCarryingGrandfather = false;
    public bool m_playerDiesAsGrandfather = false;

    public bool m_sonIsReadyToBeTeenager = false;
    public bool m_sonIsTeenager = false;
    public bool m_sonIsReadyToBeFather = false;
    public bool m_sonAtPyramid = false;
    public bool m_sonAtStartArea = false;
    public bool m_sonMovingToFatherArea = false;
    public bool m_sonAtFatherArea = false;

    public bool m_fatherAtPyramid = false;
    public bool m_fatherAtStartArea = false;
    public bool m_fatherAtGrowOldArea = false;
    public bool m_fatherHasSilverShield = false;
    public bool m_fatherIsReadyToBeGrandfather = false;

    public bool m_grandfatherAtStartArea = false;
    public bool m_grandfatherAtTeenagerArea = false;
    public bool m_grandfatherAtFatherArea = false;
    public bool m_grandfatherReadyToBeCarried = false;
    public bool m_grandfatherHadSilverShield = false;

    public bool m_speedThroughAnims = false;
    public float m_speedThroughModifier = 30;

    public static GameController m_instance = null;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        if (m_instance != null)
            Destroy(this); // Class dies here, and doesn't continue
        m_instance = this;
        LoadLevel();
        m_gameTimer.GetComponent<CountdownTimer>().m_reportRanOutOfTime = TimerRanOut;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
            m_childController.TakeDamage(m_childController.gameObject, 0);
    }

    public void LoadLevel()
    {
        SetAllTreasureIndicatorsToGray();
        switch (m_levelState)
        {
            case LEVEL_STATE.LEVEL_3:
                LoadLevel_1();
                break;
            case LEVEL_STATE.LEVEL_2:
                LoadLevel_1();
                break;
            default:
                LoadLevel_1();
                break;
        }
    }

    public void LoadLevel_1()
    {
        m_camera.FadeOutBlankCard_Duration_Delay(1, 0);
        // Standard
        //InitFather(false);
        //m_fatherController.FatherGainsSilverShield();

        //InitGrandfather(false, m_fatherController);
        //m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
        //m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        //m_grandfatherController.UpgradeHeart();
        //m_fatherCarryingGrandfather = true;
        //InitChild(true);
        //m_gameTimer.StartTimer(160);


        //// Start as Teenager with Father and Grandfather
        //InitFather(false);
        ////m_fatherController.FatherGainsSilverShield();

        //InitGrandfather(false, m_fatherController);
        //m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
        //m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        //m_grandfatherController.UpgradeHeart();
        //m_fatherCarryingGrandfather = true;
        //InitTeenager(true);

        //// Start as Teenager with Father
        //InitFather(false);
        ////m_fatherController.FatherGainsSilverShield();

        //m_sonIsReadyToBeFather = true;
        //m_fatherIsReadyToBeGrandfather = true;
        //InitTeenager(true);


        //// Start as Teenager with Father
        //InitFather(false);
        ////m_fatherController.FatherGainsSilverShield();

        //m_sonIsReadyToBeFather = true;
        //m_fatherIsReadyToBeGrandfather = true;
        //InitTeenager(true);


        //// Start as Father with Child
        //InitFather(true);
        //m_fatherController.FatherGainsSilverShield();
        //InitGrandfather(false, m_fatherController);
        //m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
        //m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        //m_grandfatherController.UpgradeHeart();
        //m_fatherCarryingGrandfather = true;
        //InitChild(false);
        //m_gameTimer.StartTimer(160);

        //// Start as Father with Teenager
        //InitFather(true);
        ////m_fatherController.FatherGainsSilverShield();
        //InitTeenager(false);
        //m_sonIsReadyToBeFather = true;
        //m_fatherIsReadyToBeGrandfather = true;


        // Start as Grandfather
        InitFather(false);
        m_fatherController.FatherGainsSilverShield();

        InitGrandfather(true, m_fatherController);
        m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
        m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        m_grandfatherController.UpgradeHeart();
        m_fatherCarryingGrandfather = true;
        InitChild(false);
        m_sonIsReadyToBeTeenager = true;
    }
    public void LoadLevel_2()
    {
        InitFather(false);
        InitChild(true); 
        m_gameTimer.StartTimer(160);
    }
    public void LoadLevel_3()
    {
        InitChild(true);
        m_gameTimer.StartTimer(160);
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
        m_childController.m_reportAtStart = TeenagerReportsReachesStartArea;
        m_childController.m_reportAtPyramid = ChildReportsReachesPyramid;
        m_childController.m_reportGotOlder = ChildReportsGotOlder;
        m_childController.m_reportGotTreasure = SonGotTreasure;
        m_childController.m_reportDies = ChildDies;
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().SetTargetHandler_AndTarget_AndActivateSpawner(m_childHandler, m_childController.gameObject);
        if (m_speedThroughAnims)
        {
            m_childController.m_speed = m_speedThroughModifier;
        }
    }
    public void InitTeenager(bool initAsPlayer = false, SpriteController_Child spriteController = null, float? placement = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        if (placement != null)
            rotation.eulerAngles = new Vector3(0, 0, (float)placement);
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
        m_teenagerController.GetComponent<HurtableCollider>().m_reportHealthChangedAndIsNow = SonHealthChangedAndIsNow;
        m_teenagerController.m_reportAtStart = TeenagerReportsReachesStartArea;
        m_teenagerController.m_reportAtPyramid = ChildReportsReachesPyramid;
        m_teenagerController.m_reportGotOlder = TeenagerReportsGotOlder;
        m_teenagerController.m_reportGotTreasure = SonGotTreasure;
        m_teenagerController.m_reportReachingGrandfather = TeenagerReportsGrandfatherEnteredArea;
        m_teenagerController.m_reportPickingUpGrandfather = TeenagerPicksUpGrandfather;
        m_teenagerController.m_reportDies = TeenagerDies;
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().SetTargetHandler_AndTarget_AndActivateSpawner(m_teenagerHandler, m_teenagerController.gameObject);

        if (m_speedThroughAnims)
        {
            m_teenagerController.m_speed = m_speedThroughModifier;
        }
    }

    public void InitFather(bool initAsPlayer = false, SpriteController_Teenager spriteController = null, float? placement = null)
    {
        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        if (placement != null)
            rotation.eulerAngles = new Vector3(0, 0, (float)placement);
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
            if (m_grandfatherHandler != null && m_grandfatherHandler.GetComponent<PlayerController>().enabled)
                m_fatherHandler.GetComponent<SpriteDriver_Father>().m_avoidEnemies = false;
        }

        m_sonIsReadyToBeFather = false;
        m_sonIsTeenager = false;
        m_fatherIsReadyToBeGrandfather = false;
        m_fatherAtGrowOldArea = false;
        m_fatherController.m_reportFatherHasSonInArea = FatherReportsSonAtArea;
        m_fatherController.m_reportFatherReachedPyramid = FatherReportsReachesPyramid;
        m_fatherController.m_reportPlacedGrandfatherdAnimDone = FatherReportsPlacedGrandfatherAnimDone;
        m_fatherController.m_reportFatherReachedStartArea = FatherReportsReachesStartArea;
        m_fatherController.m_reportFatherReachesGrowOldArea = FatherReportsReachesGrowOldArea;
        m_fatherController.m_reportPyramidAnimDone = FatherAtPyramidAnimDone;
        m_fatherController.m_reportGrowOldAnimDone = FatherGrowOldAnimDone;

        if (m_speedThroughAnims)
        {
            m_fatherController.m_walkSpeed = m_speedThroughModifier;
        }
        m_fatherController.ContinueWalking();
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
            if (m_fatherHandler != null)
                m_fatherHandler.GetComponent<SpriteDriver_Father>().m_avoidEnemies = false;
        }
        m_grandfatherController.m_reportGrowOldMoveToTargetDone = GrandfatherMovesToAdultSonDone;
        m_grandfatherController.m_reportRevealPyramid = GrandfatherReportsRevealPyramidAnim;
        m_grandfatherController.m_reportRevealPyramidDone = GrandfatherReportsRevealPyramidAnimDone;
        m_grandfatherController.m_reportGrandfatherDiesAloneDone = GrandfatherDiesAlone_AsPlayerDone;
        if(spriteController != null)
            m_grandfatherHadSilverShield = spriteController.m_shield.m_state == Shield.STATE.SILVER;
    }

    public void ChildBecomesTeenager()
    {
        InitTeenager(m_childController.m_spriteHandler.GetComponent<PlayerController>().enabled, m_childController);
        m_gameTimer.StartTimer(160);
        m_childController.DestroySelf();
        SetAllTreasureIndicatorsToGray();
    }

    public void PutChildIntoCinematicMode()
    {
        if (m_childHandler.GetComponent<SpriteDriver_Child>().enabled)
        {
            //m_fatherController.m_walkSpeed = 0;
            //m_fatherController.gameObject.GetComponent<Animator>().StopPlayback();
        }
    }

    public void ChildReportsReachesPyramid(bool b)
    {
        m_sonAtPyramid = b;
        if (m_sonIsReadyToBeTeenager && m_fatherAtPyramid && m_sonAtPyramid)
        {
            if(m_fatherCarryingGrandfather)
                m_grandfatherController.BeginRevealPyramidAnim();
            else
                PlayFatherAtPyramidSequence();
        }
    }

    public void ChildReportsGotOlder(I_SpriteController i_spriteController)
    {
        if (m_childHandler.GetComponent<PlayerController>().enabled)
            m_camera.ZoomOut();
        if (m_childController.m_growthCount == 3)
        {
            m_sonIsReadyToBeTeenager = true;
            SetTreasureArray_ToActiveOrInactive(m_treasuresA, false);
        }
    }
    public void ChildDies()
    {
        m_camera.EnterCinematicEndingMode();
        if (m_childHandler.GetComponent<PlayerController>().enabled)
        {
            StartCoroutine(PlayerDiedOrSonDied());
        }
        else
        {
            if (m_levelState == LEVEL_STATE.LEVEL_2 && m_nextLevelUnlocked[1] != true)
                m_nextLevelUnlocked[1] = true;
            StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(true, 1, 10));
        }
        m_childController.DestroySelf();
    }

    public void TeenagerBecomesAdult()
    {
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().m_targetHandler = null;
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
        m_tricksPointer.SetActive(false);
        m_enemySpawnerHandler.SetActive(false);
        InitFather(m_teenagerController.m_spriteHandler.GetComponent<PlayerController>().enabled, m_teenagerController);
        m_fatherAtStartArea = true;
        m_teenagerController.DestroySelf();
        SetAllTreasureIndicatorsToGray();
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
    public void MoveTeenagerToGameObject(GameObject g)
    {
        if (g == null && m_sonMovingToFatherArea) // Cancels a move
        {
            if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
            {
                m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled = false;
                m_teenagerHandler.GetComponent<PlayerController>().EnablePlayerControls();
            }
            else
                m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_target = g;
            m_sonMovingToFatherArea = false;
            m_teenagerController.Idle();
            return;
        }

        m_sonMovingToFatherArea = true;
        //Make the teenager move to the father->grandfather
        if (m_teenagerHandler != null)
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_target = g;
            if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
            {
                m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled = true;
                m_teenagerHandler.GetComponent<PlayerController>().DisablePlayerControls();
            }
        }
    }

    public void TeenagerReportsGotOlder(I_SpriteController i_spriteController)
    {
        if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
            m_camera.ZoomOut();
        if (m_teenagerController.m_growthCount == 3)
        {
            m_sonIsReadyToBeFather = true;
            SetTreasureArray_ToActiveOrInactive(m_treasuresB, false);
            m_tricksPointer.SetActive(false);
            if (m_fatherIsReadyToBeGrandfather && m_fatherAtGrowOldArea)
            {
                if (m_sonAtFatherArea)
                    PlayFatherGrowsOldSequence();
                else
                    MoveTeenagerToGameObject(m_fatherController.gameObject);
            }
        }
    }

    public void TeenagerReportsReachesStartArea(bool b)
    {
        m_sonAtStartArea = b;

        if (m_sonAtStartArea && m_sonIsReadyToBeFather && m_fatherController == null)
        {
            InitFather(m_teenagerController.m_spriteHandler.GetComponent<PlayerController>().enabled, m_teenagerController);
            m_fatherAtStartArea = true;
            m_teenagerController.DestroySelf();
            if (m_grandfatherHandler != null)
                GrandfatherDies_AsNpc();
        }
    }

    public void TeenagerReportsGrandfatherEnteredArea(bool grandfatherIsAtTeenager)
    {
        m_grandfatherAtTeenagerArea = grandfatherIsAtTeenager;

        if (m_grandfatherReadyToBeCarried && m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled)
            m_teenagerController.Action(); //TeenagerExecutesActionAfterDelay();
        if (m_sonMovingToFatherArea)
            MoveTeenagerToGameObject(null);
    }
    public IEnumerator TeenagerExecutesActionAfterDelay()
    {
        float delay = 0.5f;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }
        m_teenagerController.Action(); // Teenager picks up the grandfather
    }
    public void TeenagerPicksUpGrandfather()
    {
        TeenagerBecomesAdult();
        GrandfatherMovesToAdultSon();
    }
    public void TeenagerDies()
    {
        m_camera.EnterCinematicEndingMode();
        if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
        {
            StartCoroutine(PlayerDiedOrSonDied());
        }
        else
            StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(true, 2, 10));
        m_teenagerController.DestroySelf();
    }
    public void SonGotTreasure(int treasureCount)
    {
        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        }
        SetTreasureIndicatorToYellow(treasureCount-1);
        if (treasureCount == 3)
            m_gameTimer.StartFadeTimer();
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

        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled && health == 1)
            m_fatherHandler.GetComponent<SpriteDriver_Father>().PullBackSon();
    }

    public void FatherReportsSonAtArea(bool b)
    {
        m_sonAtFatherArea = b;

        if (m_sonAtFatherArea)
        {
            if(m_sonMovingToFatherArea)
            {
                MoveTeenagerToGameObject(null);
                if (m_fatherIsReadyToBeGrandfather)
                {
                    PlayFatherGrowsOldSequence();
                }
                return;
            }
            // Father at pyramid buries Grandfather and Child becomes Teenager
            if (m_sonIsReadyToBeTeenager && m_fatherCarryingGrandfather && m_fatherAtPyramid && m_grandfatherHandler != null)
            { 
                if(m_grandfatherHandler != null && m_fatherCarryingGrandfather)
                {
                    m_grandfatherController.BeginRevealPyramidAnim();
                }
                else
                {
                    PlayFatherAtPyramidSequence();
                }
                return;
            }
            // GrandfatherNpc heals son
            if (m_fatherCarryingGrandfather && !m_grandfatherController.m_spriteHandler.GetComponent<PlayerController>().enabled)
            {
                m_grandfatherController.Action();
                return;
            }
            // FatherNpc heals son
            if (m_fatherController.m_hasHeart && !m_fatherController.m_spriteHandler.GetComponent<PlayerController>().enabled)
            {
                m_fatherController.Action();
                return;
            }
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
            m_fatherIsReadyToBeGrandfather = true;
        }
        if (m_sonIsReadyToBeTeenager && m_fatherAtPyramid)
        {
            if (m_fatherCarryingGrandfather)
                m_grandfatherController.BeginRevealPyramidAnim();
            else
                PlayFatherAtPyramidSequence();
        }
    }
    public void FatherReportsReachesStartArea(bool b)
    {
        m_fatherAtStartArea = b;
        if (!m_fatherAtStartArea && m_childController == null && m_teenagerController == null && !m_fatherBecomingGrandfather)
        {
            InitChild(false, m_fatherController);
            m_gameTimer.StartTimer(160);
            SetTreasureArray_ToActiveOrInactive(m_treasuresA, true);
            m_tricksPointer.SetActive(true);
        }
    }

    public void FatherReportsReachesGrowOldArea(bool fatherAtGrowOldArea)
    {

        m_fatherAtGrowOldArea = fatherAtGrowOldArea;

        if (fatherAtGrowOldArea && m_fatherIsReadyToBeGrandfather)
        {
            m_fatherController.Idle();

            if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled) // Player is the son
            {
                m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();

                if (m_sonIsReadyToBeFather)
                {
                    if (m_sonAtFatherArea)
                        PlayFatherGrowsOldSequence();
                    else
                        MoveTeenagerToGameObject(m_fatherController.gameObject);
                }
            }
            else // Player is the father
            {
                m_fatherHandler.GetComponent<PlayerController>().DisablePlayerControls();

                if (m_sonIsReadyToBeFather)
                {
                    if (m_sonAtFatherArea)
                        PlayFatherGrowsOldSequence();
                    else
                        MoveTeenagerToGameObject(m_fatherController.gameObject);
                }
                else // Son not strong enough to carry Grandfather, kill the player
                    GrandfatherDiesAlone_AsPlayer();
            }
        }
    }

    public void PlayFatherAtPyramidSequence()
    {
        //Debug.Log("Called PlayFatherBuriesGrandfatherAnim");
        if (m_speedThroughAnims)
            m_fatherController.GetComponent<Animator>().speed = m_speedThroughModifier;
        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        if (m_fatherCarryingGrandfather)
        {
            m_grandfatherAtFatherArea = false;
            m_grandfatherHandler.SetActive(false);
            if (m_pyramid.m_powerState == Pyramid.POWER_STATE.GOLD && m_grandfatherHandler.GetComponent<PlayerController>().enabled)
            {
                m_camera.EnterCinematicMode(m_pyramid.GetNextBlock());
                m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfatherHeartAndMind");
                m_playerDiesAsGrandfather = true;
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

    public void FatherReportsPlacedGrandfatherAnimDone()
    {
        m_pyramid.AddBlock();
    }

    public void FatherAtPyramidAnimDone()
    {
        m_fatherController.ContinueWalking();
        if (m_playerDiesAsGrandfather)
        {
            m_camera.EnterCinematicEndingMode(m_pyramid.gameObject);
            StartCoroutine(PlayerDiedOrSonDied());
        }
        else
        {
            m_pyramid.HidePyramid();
            m_camera.ExitCinematicMode();
            SetTreasureArray_ToActiveOrInactive(m_treasuresB, true);
        }
        ChildBecomesTeenager();
        if (m_fatherCarryingGrandfather)
            m_fatherCarryingGrandfather = false;
    }

    public void FatherBecomesGrandfather()
    {
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
        m_enemySpawnerHandler.SetActive(false);
        InitGrandfather((m_fatherController).m_spriteHandler.GetComponent<PlayerController>().enabled, m_fatherController);
        m_fatherController.DestroySelf();
        m_fatherBecomingGrandfather = false;
    }

    public void PlayFatherGrowsOldSequence()
    {

        m_fatherBecomingGrandfather = true;
        //Debug.Log("Called PlayFatherGrowsOldSequence");
        m_fatherController.GetComponent<Animator>().Play("FatherGrowOld");
    }

    public void FatherGrowOldAnimDone()
    {
        m_grandfatherReadyToBeCarried = true;
        m_fatherController.gameObject.SetActive(false);
        FatherBecomesGrandfather();

        //Make the teenager move back to the father->grandfather
        if (m_teenagerHandler != null && m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled)
        {
            MoveTeenagerToGameObject(m_grandfatherController.gameObject);
            //StartCoroutine(TeenagerExecutesActionAfterDelay());
        }
    }

    public void PutFatherIntoCinematicMode()
    {
        //Debug.Log("PutFatherIntoCinematicMode");
        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            //Debug.Log("Driver detected");
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        }
        m_fatherController.Idle();
    }

    public void PutFatherOutOfCinematicMode()
    {
        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            m_fatherController.ContinueWalking();
        }
    }

    public void GrandfatherReportsRevealPyramidAnim()
    {
        m_pyramid.gameObject.SetActive(true);
        m_pyramid.RevealPyramid();
    }
    public void GrandfatherReportsRevealPyramidAnimDone()
    {
        PlayFatherAtPyramidSequence();
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
        ExitCinematicMode();
        m_fatherController.ContinueWalking();
        m_fatherCarryingGrandfather = true;
        if (m_grandfatherHadSilverShield) { m_fatherController.m_shield.gameObject.SetActive(true); m_fatherController.m_shield.UpgradeToSilver(); }
    }

    public void GrandfatherDiesAlone_AsPlayer()
    {
        // Have son target starting area
        MoveTeenagerToGameObject(m_treasuresA[0]); // Make the son walk away

        // Play the Grandfather die animation and then the player dies
        m_grandfatherController.PlayGrandfatherDiesAloneAnim();
    }

    public void GrandfatherDiesAlone_AsPlayerDone()
    {
        // Put the camera into cenimaic mode
        m_camera.EnterCinematicEndingMode(m_grandfatherController.gameObject);
        StartCoroutine(PlayerDiedOrSonDied());
    }

    public void GrandfatherDies_AsNpc()
    {
        // Replace Grandfather with stone
        m_grandfatherController.DestroySelf();
        m_grandfatherNpcDiesStone.SetActive(true);

        // Move the Grow Old Area Trigger
        m_growOldAreaTrigger.transform.position = m_growOldAreaTriggerNextPosition.transform.position;
        if(m_levelState == LEVEL_STATE.LEVEL_1 && m_nextLevelUnlocked[0] != true)
            m_nextLevelUnlocked[0] = true;
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


    public void UnlockNextLevel()
    {
        // load game data
        DataManager.m_gameData = JsonUtility.FromJson<DataManager.GameData>("GameDataTextAsset");

        // Save the data to unlock the next level
        if (m_levelState == LEVEL_STATE.LEVEL_1 && !DataManager.m_gameData.m_level_2_Unlocked)
        {
            DataManager.m_gameData.m_level_2_Unlocked = true;
            DataManager.Save();
            return;
        }
        // Save the data to unlock the next level
        if (m_levelState == LEVEL_STATE.LEVEL_2 && !DataManager.m_gameData.m_level_3_Unlocked)
        {
            DataManager.m_gameData.m_level_3_Unlocked = true;
            DataManager.Save();
            return;
        }
    }

    public void HidePyramid()
    {
        m_pyramid.gameObject.SetActive(false);
    }

    public void SetTreasureIndicatorToYellow(int i)
    {
        if(i < 3)
        {
            m_treasures[i].GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public void SetAllTreasureIndicatorsToGray()
    {
        foreach (GameObject g in m_treasures)
            g.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public void SetTreasureArray_ToActiveOrInactive(GameObject[] treasureHandler, bool setActiveToTrue)
    {
        foreach (GameObject g in treasureHandler)
            g.SetActive(setActiveToTrue);
    }

    public IEnumerator PlayerDiedOrSonDied(/*float sequenceDelay = 0*/)
    {
        //yield return new WaitForSeconds(sequenceDelay);
        float duration = 10;
        float delay = 1;
        if (m_playerDiesAsGrandfather)
        {
            if(!m_fatherAtPyramid)
            {
                // Give reload options
                StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(false, delay, duration));
                // If no choice made -> Call Game Over
                yield return new WaitForSeconds(delay + duration + 2);
                StartCoroutine(GameOver()); // Reloads StartScreen
            }
            else
            {
                // Show level ending card and reload StartScreen
                StartCoroutine(LevelEnd());
            }
        }
        else // Player is a child/teenager OR is father with son
        {
            // Give reload options
            StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(false, delay, duration));
            // If no choice made -> Call Game Over
            yield return new WaitForSeconds(delay + duration + 2);
            StartCoroutine(GameOver()); // Reloads StartScreen
        }
    }

    public IEnumerator GameOver()
    {
        // Fade in Game Over card
        m_camera.FadeInGameOverCard_Duration_Delay(5, 0);
        yield return new WaitForSeconds(7);
        // Call ReloadStartScreen
        StartCoroutine(ReloadStartScreen());
    }

    public IEnumerator LevelEnd()
    {
        float duration = 7;
        float delay = 0;
        // Fade in Level End card
        if (m_levelState == LEVEL_STATE.LEVEL_1 && !m_nextLevelUnlocked[0])
        {
            m_camera.FadeInLevel1CompleteCard_Duration_Delay(duration, delay);
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_2 && !m_nextLevelUnlocked[1])
        {
            m_camera.FadeInLevel2CompleteCard_Duration_Delay(duration, delay);
        }
        else
        {
            m_camera.FadeInLevel3CompleteCard_Duration_Delay(duration, delay);
        }
        yield return new WaitForSeconds(duration+2);
        // Call ReloadStartScreen
        StartCoroutine(ReloadStartScreen());
    }

    public IEnumerator ReloadLevel()
    {
        // Fade to black, and then reload the level
        m_camera.FadeInBlankCard_Duration_Delay(3, 0);
        yield return new WaitForSeconds(5);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public IEnumerator ReloadStartScreen()
    {
        // Fade to black, and then reload the StartScreen
        m_camera.FadeInBlankCard_Duration_Delay(3, 0);
        yield return new WaitForSeconds(5);
        SceneManager.LoadSceneAsync("StartScreen");
    }

    // Used for level/checkpoint reloads
    // Called on GameOvers
    //public IEnumerator FadeInGameOverCardBool_ThenBlankCard_ThenReloadLevelBool_AfterDelaySeconds(bool showGameOverCard = false, bool reloadLevel = false, float delaySeconds = 0)
    //{
    //    if (showGameOverCard)
    //    {
    //        m_camera.FadeInGameOverCard_Duration_Delay();
    //        yield return new WaitForSeconds(7);
    //    }
    //    m_camera.FadeInBlankCard_Duration_Delay();
    //    yield return new WaitForSeconds(delaySeconds);
    //    if (reloadLevel)
    //        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    //    else
    //        SceneManager.LoadSceneAsync("StartScreen");
    //}

    public IEnumerator ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(bool showContinueButton = false, float delay = 2, float secondsToShowButtons = 10)
    {
        yield return new WaitForSeconds(delay);

        m_resetOptionsButtons.SetActive(true);
        if (showContinueButton) m_continueButton.SetActive(true);

        yield return new WaitForSeconds(secondsToShowButtons);

        m_resetOptionsButtons.SetActive(false);
        if (showContinueButton) m_continueButton.SetActive(false);
    }
    public void ContinueGame()
    {
        m_resetOptionsButtons.SetActive(false);
        m_continueButton.SetActive(false);
        StopCoroutine(m_reloadLevelAfterDelayCoroutine);
    }

    public void LoadLastCheckpoint()
    {
        StopCoroutine(m_reloadLevelAfterDelayCoroutine);
        switch (m_checkpointState)
        {
            case CHECKPOINT_STATE.START:
                LoadStartCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_TEENAGER_REACHES_PYRAMID:
                LoadStartCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_START:
                LoadStartCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_PYRAMID:
                LoadStartCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_GRANDFATHER_REACHES_START:
                LoadStartCheckpoint();
                break;
        }
    }

    public void LoadStartCheckpoint()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void LoadPlayerAsTeenagerReachesPyramidCheckpoint()
    {
        Destroy(m_fatherHandler);
        Destroy(m_teenagerHandler);

        InitFather(false, null, 160);
        if(m_levelState == LEVEL_STATE.LEVEL_1)
        {
            m_pyramid.m_blockCount = 1;
        }
        InitTeenager(true, null, 160);
    }

    public void LoadPlayerAsFatherReachesStartCheckpoint()
    {
        Destroy(m_fatherHandler);
        InitFather(true);
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            m_pyramid.m_blockCount = 1;
        }
        InitTeenager(true, null, 160);
    }

    public void LoadPlayerAsFatherReachesPyramidCheckpoint()
    {

    }

    public void LoadPlayerAsGrandfatherReachesPyramidCheckpoint()
    {

    }

    public void TimerRanOut()
    {
        StartCoroutine(PlayerDiedOrSonDied());
    }
}
