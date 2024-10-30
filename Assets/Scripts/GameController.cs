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
    public Coroutine m_showResetOptionButtonsCoroutine;
    public Coroutine m_reloadLevelAfterDelayCoroutine;
    public Coroutine m_playerDiedOrSonDiedCoroutine;
    public Coroutine m_gameOverCoroutine;
    public float m_reloadLevelDelay = 11f;

    public Pyramid m_pyramid;
    public GameObject[] m_treasuresA;
    public GameObject[] m_treasuresB;
    public GameObject m_treasurePointer;
    public GameObject m_tricksPointer;
    public GameObject m_enemySpawnerHandler;
    public GameObject m_growOldAreaTrigger, m_growOldAreaTriggerNextPosition;
    public GameObject m_grandfatherNpcDiesStone;
    public GameObject m_ObjectBeyondStartArea;

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

    public GameObject m_debugButtons;
    public CountdownTimer m_gameTimer;
    public CameraController m_camera;
    public GameObject m_world;
    public AmbientMusicController m_ambientMusicController;

    // Milestones
    public bool[] m_nextLevelUnlocked = new bool[2];
    public bool m_playerBecomesFather = false;
    public bool m_playerBecomesGrandfather = false;
    public bool m_playerDiesAsGrandfather = false;
    public bool m_PlayersFatherDiedAlone = false;

    public bool m_sonIsReadyToBeTeenager = false;
    public bool m_sonIsTeenager = false;
    public bool m_sonIsReadyToBeFather = false;
    public bool m_sonAtPyramid = false;
    public bool m_sonAtStartArea = false;
    public bool m_sonMovingToFatherArea = false;
    public bool m_sonAtFatherArea = false;
    public bool m_sonDiedAsChild = false;
    public bool m_sonDiedAsTeenager = false;

    public bool m_fatherAtPyramid = false;
    public bool m_fatherAtStartArea = false;
    public bool m_fatherAtGrowOldArea = false;
    public bool m_fatherIsReadyToBeGrandfather = false;
    public bool m_fatherCarryingGrandfather = false;
    public bool m_fatherBecomingGrandfather = false;
    public bool m_fatherNowGrandfatherDies = false;

    public bool m_grandfatherPresentFromStart = false;
    public bool m_grandfatherAtTeenagerArea = false;
    public bool m_grandfatherAtFatherArea = false;
    public bool m_grandfatherReadyToBeCarried = false;
    public bool m_grandfatherHadSilverShield = false;
    public bool m_grandfatherDies = false;

    public bool m_speedThroughAnims = false;
    public float m_speedThroughModifier = 30;

    public static GameController m_instance = null;

    public void Start()
    {
        LoadLevel();
        m_gameTimer.GetComponent<CountdownTimer>().m_reportRanOutOfTime = TimerRanOut;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
            m_childController.TakeDamage(m_childController.gameObject, 0);

        if (Input.GetKeyUp(KeyCode.P))
            m_debugButtons.SetActive(!m_debugButtons.activeSelf);
    }

    public void LoadLevel()
    {
        SetAllTreasureIndicatorsToGray();
        switch (m_levelState)
        {
            case LEVEL_STATE.LEVEL_1:
                LoadLevel_1();
                break;
            case LEVEL_STATE.LEVEL_2:
                LoadLevel_2();
                break;
            default:
                LoadLevel_3();
                break;
        }
    }

    public void LoadLevel_1()
    {
        m_camera.FadeOutBlankCard_Duration_Delay(1, 0);
        // Standard
        InitFather(false);
        m_fatherController.FatherGainsSilverShield();

        InitGrandfather(false, m_fatherController);
        m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
        m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        m_grandfatherController.UpgradeHeart();
        m_fatherCarryingGrandfather = true;
        m_grandfatherPresentFromStart = true;
        m_grandfatherHadSilverShield = true;
        InitChild(true);
    }
    public void LoadLevel_2()
    {
        InitFather(false);
        InitChild(true);
    }
    public void LoadLevel_3()
    {
        InitChild(true);
    }

    public void InitChild(bool initAsPlayer = false, SpriteController_Father spriteController = null)
    {
        if (m_childHandler != null || m_teenagerHandler != null)
            return;

        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_childHandler = Instantiate(m_childSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_childController = m_childHandler.GetComponent<SpriteDriver_Child>().m_spriteController;
        m_sonAtFatherArea = true;
        m_sonDiedAsChild = false;
        m_sonIsReadyToBeTeenager = false;
        if (initAsPlayer)
        {
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().m_leadingAmount = 5;
            m_childHandler.GetComponent<SpriteDriver_Child>().enabled = false;
            m_childHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_childController.gameObject;
            m_childHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
            m_camera.ZoomTo(0);
            m_ambientMusicController.StartAmbientMusic(0);
        }
        else
        {
            m_childHandler.GetComponent<SpriteDriver_Child>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }

        m_childController.GetComponent<HurtableCollider>().m_reportHealthChangedAndIsNow = SonHealthChangedAndIsNow;
        m_childController.m_reportAtPyramid = ChildReportsReachesPyramid;
        m_childController.m_reportGotOlder = ChildReportsGotOlder;
        m_childController.m_reportGotTreasure = SonGotTreasure;
        m_childController.m_reportDies = ChildDies;
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().SetTargetHandler_AndTarget_AndActivateSpawner(m_childHandler, m_childController.gameObject);
        if (m_speedThroughAnims)
        {
            m_childController.m_speed = m_speedThroughModifier;
        }
        ActivateGameTimerWithSeconds();
        SonHealthChangedAndIsNow(3);
    }
    public void InitTeenager(bool initAsPlayer = false, SpriteController_Child spriteController = null, float? placement = null)
    {
        if ((spriteController == null && m_childHandler != null) || m_teenagerHandler != null)
            return;

        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        if (placement != null)
            rotation.eulerAngles = new Vector3(0, 0, (float)placement);
        m_teenagerHandler = Instantiate(m_teenagerSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_teenagerController = m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_spriteController;
        if (initAsPlayer)
        {
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().m_leadingAmount = 20;
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled = false;
            m_teenagerHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_teenagerController.gameObject;
            m_teenagerHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
            m_camera.ZoomTo(4);
            m_ambientMusicController.StartAmbientMusic(1);
        }
        else
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_target = m_fatherController.GetComponent<SpriteController_Father>().m_visionToWorldHitPoint;
        }

        m_sonIsReadyToBeFather = false;
        m_sonIsReadyToBeTeenager = false;
        m_sonIsTeenager = true;
        m_sonDiedAsTeenager = false;
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
        ActivateGameTimerWithSeconds();
        SonHealthChangedAndIsNow(3);
    }

    public void InitFather(bool initAsPlayer = false, SpriteController_Teenager spriteController = null, float? placement = null)
    {
        if (m_fatherHandler != null)
            return;

        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        if (placement != null)
            rotation.eulerAngles = new Vector3(0, 0, (float)placement);
        m_fatherHandler = Instantiate(m_fatherSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_fatherController = m_fatherHandler.GetComponent<SpriteDriver_Father>().m_spriteController;
        if (initAsPlayer)
        {
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().m_leadingAmount = 50;
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().SetSpawnFrequency(5, 10);
            m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled = false;
            m_fatherHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_fatherController.gameObject;
            m_camera.PlayerBecomesAdult();
            m_fatherHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
            m_camera.ZoomTo(8);
            m_camera.m_lookAhead = 7;
            m_ambientMusicController.StartAmbientMusic(2);
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
        if (m_grandfatherHandler != null)
            return;

        Quaternion rotation = spriteController == null ? Quaternion.identity : spriteController.m_spriteHandler.transform.rotation;
        m_grandfatherHandler = Instantiate(m_grandfatherSpritePrefab, Vector3.zero, rotation, m_world.transform);
        m_grandfatherController = m_grandfatherHandler.GetComponent<SpriteDriver_Grandfather>().m_spriteController;
        if (initAsPlayer)
        {
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().m_leadingAmount = 50;
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().SetSpawnFrequency(5, 10);
            m_grandfatherHandler.GetComponent<SpriteDriver_Grandfather>().enabled = false;
            m_grandfatherHandler.GetComponent<PlayerController>().enabled = true;
            m_camera.m_playerTarget = m_grandfatherController.gameObject;
            m_grandfatherHandler.GetComponent<PlayerController>().m_cameraController = m_camera;
            if (m_fatherHandler != null)
                m_fatherHandler.GetComponent<SpriteDriver_Father>().m_avoidEnemies = false;
            m_camera.ZoomTo(9);
            m_camera.m_lookAhead = 7;
            m_ambientMusicController.StartAmbientMusic(3);
        }
        m_grandfatherController.m_reportGrowOldMoveToTargetDone = GrandfatherMovesToAdultSonDone;    
        m_grandfatherController.m_reportRevealPyramid = GrandfatherReportsRevealPyramidAnim;
        m_grandfatherController.m_reportRevealPyramidDone = GrandfatherReportsRevealPyramidAnimDone;
        m_grandfatherController.m_reportGrandfatherDiesAloneDone = GrandfatherDiesAlone_AsPlayerDone;
    }

    public void ChildReportsReachesPyramid(bool b)
    {
        m_sonAtPyramid = b;
        if (m_sonIsReadyToBeTeenager && m_sonAtPyramid && m_fatherController == null)
        {
            ChildBecomesTeenager();
            SetTreasureArray_ToActiveOrInactive(m_treasuresB, true);
        }
    }

    public void ChildReportsGotOlder(I_SpriteController i_spriteController)
    {
        if (m_childHandler.GetComponent<PlayerController>().enabled)
            m_camera.ZoomOut();
        if (m_childController.m_growthCount == 3)
        {
            if (m_gameTimer.enabled)
            {
                m_gameTimer.PauseTimer();
                m_gameTimer.StartFadeTimer();
            }
            m_sonIsReadyToBeTeenager = true;
            SetTreasureArray_ToActiveOrInactive(m_treasuresA, false);

        }
    }
    public void ChildDies()
    {
        if (m_childHandler.GetComponent<PlayerController>().enabled)
        {
            m_camera.EnterCinematicEndingMode();
            m_playerDiedOrSonDiedCoroutine = StartCoroutine(PlayerDiedOrSonDied());
        }
        else
        {
            if (m_levelState == LEVEL_STATE.LEVEL_1 || m_levelState == LEVEL_STATE.LEVEL_3)
            {
                m_showResetOptionButtonsCoroutine = StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(false, 1, 10));
                m_playerDiedOrSonDiedCoroutine = StartCoroutine(PlayerDiedOrSonDied());
            }
            else
                m_showResetOptionButtonsCoroutine = StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(true, 1, 10));
            m_sonDiedAsChild = true;
        }
        if (m_gameTimer.enabled) m_gameTimer.StartFadeTimer();
        m_childController.DestroySelf();
    }

    public void ChildBecomesTeenager()
    {
        if (m_childController != null)
        {
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
            InitTeenager(m_childHandler.GetComponent<PlayerController>().enabled, m_childController);
            m_childController.DestroySelf();
            SetAllTreasureIndicatorsToGray();
            if(m_teenagerHandler.GetComponent<PlayerController>().enabled) 
                m_checkpointState = CHECKPOINT_STATE.PLAYER_AS_TEENAGER_REACHES_PYRAMID;
        }
    }

    public void TeenagerBecomesAdult()
    {
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
        m_tricksPointer.SetActive(false);
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
        if (g == null) // Cancels a move
        {
            m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().m_target = g;
            if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
            {
                m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled = false;
                m_teenagerHandler.GetComponent<PlayerController>().EnablePlayerControls();
            }
            m_sonMovingToFatherArea = false;
            m_teenagerController.Idle();
            return;
        }

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
            if (m_gameTimer.enabled)
            {
                m_gameTimer.PauseTimer();
                m_gameTimer.StartFadeTimer();
            }

            SetTreasureArray_ToActiveOrInactive(m_treasuresB, false);
            if(m_grandfatherHandler != null) // Father has already grown old
            {
                m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
                m_tricksPointer.SetActive(false);
                m_sonMovingToFatherArea = true;
                MoveTeenagerToGameObject(m_grandfatherController.gameObject);
            }
            else if(m_fatherHandler == null) // No father or grandfather
            {
                m_sonIsReadyToBeFather = true;
            }
        }
    }
    
    public void TeenagerReportsReachesStartArea(bool b)
    {
        m_sonAtStartArea = b;

        if (m_sonAtStartArea && m_sonIsReadyToBeFather)
        {
            // Teenager left grandfather to die
            if (m_grandfatherHandler != null)
            {
                if (m_grandfatherHandler.GetComponent<PlayerController>().enabled)
                    GrandfatherDiesAlone_AsPlayer();
                else
                    GrandfatherDies_AsNpc();
            }
            if(m_sonIsReadyToBeFather && m_grandfatherHandler == null && !m_fatherAtStartArea)
            {
                TeenagerBecomesAdult();
                m_fatherAtStartArea = true;
            }
        }

    }

    public void TeenagerReportsGrandfatherEnteredArea(bool grandfatherIsAtTeenager)
    {
        m_grandfatherAtTeenagerArea = grandfatherIsAtTeenager;

        if(m_grandfatherAtTeenagerArea)
        {
            if (m_sonMovingToFatherArea)
                MoveTeenagerToGameObject(null);

            m_sonIsReadyToBeFather = true;

            if (m_grandfatherReadyToBeCarried && m_teenagerHandler.GetComponent<SpriteDriver_Teenager>().enabled)
            {
                if (m_teenagerController.m_growthCount == 3 && m_sonIsReadyToBeFather && !m_teenagerHandler.GetComponent<PlayerController>().enabled)
                    m_teenagerController.Action(); // Teenager picks up father
                else if(!m_teenagerHandler.GetComponent<PlayerController>().enabled)
                    MoveTeenagerToGameObject(m_ObjectBeyondStartArea); // Teenager leaves father to die
            }
        }
    }
    private IEnumerator TeenagerExecutesActionAfterDelay()
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
        if (m_gameTimer.enabled)
        {
            m_gameTimer.PauseTimer();
            m_gameTimer.StartFadeTimer();
        }
        if (m_teenagerHandler.GetComponent<PlayerController>().enabled)
        {
            m_camera.EnterCinematicEndingMode();
            m_playerDiedOrSonDiedCoroutine = StartCoroutine(PlayerDiedOrSonDied());
        }
        else
        {
            if (m_levelState == LEVEL_STATE.LEVEL_1 || m_levelState == LEVEL_STATE.LEVEL_3)
            {
                m_showResetOptionButtonsCoroutine = StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(false, 1, 10));
                m_playerDiedOrSonDiedCoroutine = StartCoroutine(PlayerDiedOrSonDied());
            }
            else
                m_showResetOptionButtonsCoroutine = StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(true, 1, 10));
            m_sonDiedAsTeenager = true;
        }
        if (m_gameTimer.enabled) m_gameTimer.StartFadeTimer();
        m_teenagerController.DestroySelf();
    }


    public void SonGotTreasure(int treasureCount)
    {
        if (m_fatherHandler != null && m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
        {
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        }
        SetTreasureIndicatorToYellow(treasureCount - 1);
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

        if (m_fatherHandler != null && m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled && health == 1)
            m_fatherHandler.GetComponent<SpriteDriver_Father>().PullBackSon();
    }

    public void FatherReportsSonAtArea(bool b)
    {
        m_sonAtFatherArea = b;

        if (m_sonAtFatherArea)
        {
            if (m_sonMovingToFatherArea)
            {
                MoveTeenagerToGameObject(null);
                return;
            }
            // Father at pyramid buries Grandfather and Child becomes Teenager
            if (m_sonIsReadyToBeTeenager && m_fatherCarryingGrandfather && m_fatherAtPyramid && m_grandfatherHandler != null)
            {
                m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
                if (m_grandfatherHandler != null && m_fatherCarryingGrandfather)
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
            if (m_fatherCarryingGrandfather && m_grandfatherController!= null && !m_grandfatherController.m_spriteHandler.GetComponent<PlayerController>().enabled)
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

        if (m_fatherAtPyramid)
        {
            PutFatherIntoCinematicMode();

            if ((m_sonIsReadyToBeTeenager) ||
                (m_sonDiedAsChild && m_levelState == LEVEL_STATE.LEVEL_2))
            {
                if (m_fatherCarryingGrandfather)
                    m_grandfatherController.BeginRevealPyramidAnim();
                else
                    PlayFatherAtPyramidSequence();
            }
            if (m_teenagerController != null && m_teenagerHandler.GetComponent<PlayerController>().enabled)
                m_checkpointState = CHECKPOINT_STATE.PLAYER_AS_TEENAGER_REACHES_PYRAMID;
            else if (m_fatherHandler.GetComponent<PlayerController>().enabled)
                m_checkpointState = CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_PYRAMID;
        }
        else
        {
            m_pyramid.HidePyramid();
            foreach (GameObject g in m_treasuresA)
            {
                g.SetActive(true);
            }
            m_fatherIsReadyToBeGrandfather = true;
        }
    }
    public void FatherReportsReachesStartArea(bool b)
    {
        m_fatherAtStartArea = b;
        if (!m_fatherAtStartArea && m_childController == null && m_teenagerController == null && !m_fatherBecomingGrandfather)
        {
            InitChild(false, m_fatherController);
            SetTreasureArray_ToActiveOrInactive(m_treasuresA, true);
            m_tricksPointer.SetActive(true);
        }
        if (m_fatherAtStartArea)
        {
            if (m_fatherHandler.GetComponent<PlayerController>().enabled)
                m_checkpointState = CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_START;
            else if (m_fatherCarryingGrandfather && m_grandfatherController != null && m_grandfatherController.m_spriteHandler.GetComponent<PlayerController>().enabled)
                m_checkpointState = CHECKPOINT_STATE.PLAYER_AS_GRANDFATHER_REACHES_START;
        }
    }

    public void FatherReportsReachesGrowOldArea(bool fatherAtGrowOldArea)
    {
        m_fatherAtGrowOldArea = fatherAtGrowOldArea;

        if (fatherAtGrowOldArea && m_fatherIsReadyToBeGrandfather && !m_fatherBecomingGrandfather)
        {
            m_fatherController.Idle();
            PlayFatherGrowsOldSequence();
            if (m_sonAtFatherArea)
            {
                if (m_teenagerHandler != null && m_sonAtFatherArea && m_sonIsReadyToBeFather)
                    m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
            }
        }
    }

    public void PlayFatherAtPyramidSequence()
    {
        //Debug.Log("Called PlayFatherBuriesGrandfatherAnim");
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
        if (m_speedThroughAnims)
            m_fatherController.GetComponent<Animator>().speed = m_speedThroughModifier;
        if (m_fatherHandler.GetComponent<SpriteDriver_Father>().enabled)
            m_fatherHandler.GetComponent<SpriteDriver_Father>().EnterLookDownState();
        if (m_fatherCarryingGrandfather)
        {
            m_grandfatherAtFatherArea = false;
            m_grandfatherHandler.SetActive(false);
            if (m_pyramid.m_powerState == Pyramid.POWER_STATE.GOLD)
            {
                if (m_grandfatherHandler.GetComponent<PlayerController>().enabled)
                {
                    m_playerDiesAsGrandfather = true;
                    m_camera.EnterCinematicMode(m_pyramid.GetNextBlock());
                    m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfatherHeartAndMind");
                }
                else
                {
                    m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
                }
            }
            else // if (m_pyramid.m_powerState == Pyramid.POWER_STATE.BRONZE)
            {
                if(m_pyramid.m_blockCount == 3)
                {
                    if (m_grandfatherHandler.GetComponent<PlayerController>().enabled)
                    {
                        m_playerDiesAsGrandfather = true;
                        m_camera.EnterCinematicMode(m_pyramid.GetNextBlock());
                    }

                    m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfatherBronzeHeart");
                }
                else if(m_pyramid.m_blockCount == 4)
                {
                    m_playerDiesAsGrandfather = true;
                    m_camera.EnterCinematicMode(m_pyramid.GetNextBlock());
                    m_fatherController.GetComponent<Animator>().Play("FatherPlacesDeadGrandfatherSilver");
                }
            }
            m_grandfatherController.DestroySelf();
        }
        else
        {
            m_fatherController.GetComponent<Animator>().Play("FatherAtEmptyPyramidNoGrandfather");
        }
        if (m_childController != null)
        {
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
    }

    public void FatherReportsPlacedGrandfatherAnimDone()
    {
        m_pyramid.AddBlock();
    }

    public void FatherAtPyramidAnimDone()
    {
        m_fatherController.ContinueWalking();
        m_fatherIsReadyToBeGrandfather = true;
        if (m_playerDiesAsGrandfather)
        {
            m_camera.EnterCinematicEndingMode(m_pyramid.gameObject);
            m_playerDiedOrSonDiedCoroutine = StartCoroutine(PlayerDiedOrSonDied());
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
        InitGrandfather(m_fatherHandler.GetComponent<PlayerController>().enabled, m_fatherController);
        if (m_grandfatherHadSilverShield)
        {
            m_grandfatherController.m_heart.UpgradeHeartToGold();
        }
        m_fatherController.DestroySelf();
        m_fatherBecomingGrandfather = false;
    }

    public void PlayFatherGrowsOldSequence()
    {
        Debug.Log($"PlayFatherGrowsOldSequence");
        m_fatherBecomingGrandfather = true;
        //Debug.Log("Called PlayFatherGrowsOldSequence");
        m_fatherController.GetComponent<Animator>().Play("FatherGrowOld");
        m_ambientMusicController.StartAmbientMusicMisc(0);
    }

    public void FatherGrowOldAnimDone()
    {
        m_grandfatherReadyToBeCarried = true;
        //m_fatherController.gameObject.SetActive(false);
        FatherBecomesGrandfather();
        if(m_sonDiedAsChild || m_sonDiedAsTeenager)
        {
            GrandfatherDiesAlone_AsPlayer();
        }
        else if (!m_sonIsReadyToBeFather)
        {
            m_tricksPointer.SetActive(false);
            m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner();
            m_sonMovingToFatherArea = true;
            MoveTeenagerToGameObject(m_grandfatherController.gameObject);
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
        m_fatherController.ContinueWalking();
        m_fatherCarryingGrandfather = true;
        if (m_levelState == LEVEL_STATE.LEVEL_1 && !m_PlayersFatherDiedAlone) 
        {
            m_grandfatherController.UpgradeHeart();
            m_fatherController.m_shield.gameObject.SetActive(true);
            m_fatherController.m_shield.UpgradeToSilver();
        }
        if (m_grandfatherHandler.GetComponent<PlayerController>().enabled) m_checkpointState = CHECKPOINT_STATE.PLAYER_AS_GRANDFATHER_REACHES_START;
    }

    public void GrandfatherDiesAlone_AsPlayer()
    {
        // Have son target starting area
        if (m_teenagerHandler != null)
        {
            MoveTeenagerToGameObject(m_ObjectBeyondStartArea); // Make the son walk away
            m_gameTimer.StartFadeTimer();
        }

        // Play the Grandfather die animation and then the player dies
        m_playerDiesAsGrandfather = true;
        m_grandfatherController.PlayGrandfatherDiesAloneAnim();
        m_ambientMusicController.StartAmbientMusicEnd_EndIsHappy(false);
    }

    public void GrandfatherDiesAlone_AsPlayerDone()
    {
        // Put the camera into cenimaic mode
        m_camera.EnterCinematicEndingMode(m_grandfatherController.gameObject);
        m_playerDiedOrSonDiedCoroutine = StartCoroutine(PlayerDiedOrSonDied());
    }

    public void GrandfatherDies_AsNpc()
    {
        // Replace Grandfather with stone
        m_grandfatherController.DestroySelf();
        m_grandfatherNpcDiesStone.SetActive(true);
        m_PlayersFatherDiedAlone = true;
        m_grandfatherHadSilverShield = false;
        if (m_pyramid.m_powerState != Pyramid.POWER_STATE.BRONZE) 
        { 
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
            m_pyramid.m_blockCount = 3;
        }

        // Move the Grow Old Area Trigger
        m_growOldAreaTrigger.transform.position = m_growOldAreaTriggerNextPosition.transform.position;
    }

    public void EnterCinematicMode()
    {
        if (m_fatherHandler != null)
        {
            PutFatherIntoCinematicMode();
        }
    }

    public void UnlockNextLevel()
    {
        // load game data
        DataManager dM = GameObject.FindObjectOfType<DataManager>();

        // Save the data to unlock the next level
        if (m_levelState == LEVEL_STATE.LEVEL_1 && !dM.LevelUnlockedCheck(2))
        {
            dM.LevelUnlock(2);

            return;
        }
        // Save the data to unlock the next level
        if (m_levelState == LEVEL_STATE.LEVEL_2 && !dM.LevelUnlockedCheck(3))
        {
            dM.LevelUnlock(3);
            return;
        }
    }

    public void HidePyramid()
    {
        m_pyramid.gameObject.SetActive(false);
    }

    public void SetTreasureIndicatorToYellow(int i)
    {
        if (i < 3)
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
        {
            g.SetActive(setActiveToTrue);
        }
    }

    private IEnumerator PlayerDiedOrSonDied()
    {
        //yield return new WaitForSeconds(sequenceDelay);
        float duration = 10;
        float delay = 1;
        if (m_playerDiesAsGrandfather)
        {
            if (!m_fatherAtPyramid)
            {
                if (m_levelState == LEVEL_STATE.LEVEL_2 && (m_sonDiedAsChild || m_sonDiedAsTeenager))
                    StartCoroutine(LevelEnd());
                else
                {
                    // Give reload options
                    m_showResetOptionButtonsCoroutine = StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(false, delay, duration));
                    // If no choice made -> Call Game Over
                    yield return new WaitForSeconds(delay + duration + 2);
                    m_gameOverCoroutine = StartCoroutine(GameOver()); // Reloads StartScreen
                }
            }
            else
            {
                // Show level ending card and reload StartScreen
                StartCoroutine(LevelEnd());
                m_ambientMusicController.StartAmbientMusicEnd_EndIsHappy(false);
            }
        }
        else // Player is a child/teenager OR is father with son
        {
            // Give reload options
            m_showResetOptionButtonsCoroutine = StartCoroutine(ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(false, delay, duration));
            // If no choice made -> Call Game Over
            yield return new WaitForSeconds(delay + duration + 2);
            m_gameOverCoroutine = StartCoroutine(GameOver()); // Reloads StartScreen
        }
    }

    private IEnumerator GameOver()
    {
        Debug.Log("Recall");
        // Fade in Game Over card
        m_camera.FadeInGameOverCard_Duration_Delay(5, 0);
        yield return new WaitForSeconds(7);
        // Call ReloadStartScreen
        ReloadStartScreenBeginCoroutine();
    }

    private IEnumerator LevelEnd()
    {
        float duration = 14;
        float delay = 0;
        // Fade in Level End card
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            if (!m_nextLevelUnlocked[0] && m_PlayersFatherDiedAlone)
            {
                UnlockNextLevel();
                m_nextLevelUnlocked[0] = true;
            }
            if (m_nextLevelUnlocked[0])
                m_camera.FadeInLevel3CompleteCard_Duration_Delay(duration, delay);
            else
                m_camera.FadeInLevel1CompleteCard_Duration_Delay(duration, delay);
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_2 && !m_nextLevelUnlocked[1])
        {
            if (m_sonDiedAsChild || m_sonDiedAsTeenager)
            {
                UnlockNextLevel();
                m_nextLevelUnlocked[1] = true;
            }
            if (m_nextLevelUnlocked[1])
                m_camera.FadeInLevel3CompleteCard_Duration_Delay(duration, delay);
            else
                m_camera.FadeInLevel2CompleteCard_Duration_Delay(duration, delay);
        }
        else
        {
            m_camera.FadeInLevel3CompleteCard_Duration_Delay(duration, delay);
        }
        yield return new WaitForSeconds(duration + 2);
        // Call ReloadStartScreen
        ReloadStartScreenBeginCoroutine();
    }

    public void ReloadStartScreenBeginCoroutine()
    {
        m_reloadLevelAfterDelayCoroutine = StartCoroutine(ReloadStartScreen());
    }

    private IEnumerator ReloadStartScreen()
    {
        // Fade to black, and then reload the StartScreen
        m_camera.FadeInBlankCard_Duration_Delay(3, 0);
        yield return new WaitForSeconds(5);
        SceneManager.LoadSceneAsync("StartScreen");
    }

    private IEnumerator ShowResetButtons_AndContinueButton_AfterDelaySeconds_ForSeconds(bool showContinueButton = false, float delay = 2, float secondsToShowButtons = 10)
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
        StopCoroutine(m_showResetOptionButtonsCoroutine);
        m_resetOptionsButtons.SetActive(false);
        m_continueButton.SetActive(false);
    }
    public void LoadLastCheckpoint()
    {
        if (m_showResetOptionButtonsCoroutine != null) StopCoroutine(m_showResetOptionButtonsCoroutine);
        if(m_gameOverCoroutine != null)                StopCoroutine(m_gameOverCoroutine);
        if(m_playerDiedOrSonDiedCoroutine != null)     StopCoroutine(m_playerDiedOrSonDiedCoroutine);
        m_resetOptionsButtons.SetActive(false);
        m_continueButton.SetActive(false);
        StartCoroutine(LoadLastCheckpointCoroutine());
    }
    public void LoadSpecificCheckpoint(int i)
    {
        if (m_showResetOptionButtonsCoroutine != null) StopCoroutine(m_showResetOptionButtonsCoroutine);
        if(m_gameOverCoroutine != null)                StopCoroutine(m_gameOverCoroutine);
        if(m_playerDiedOrSonDiedCoroutine != null)     StopCoroutine(m_playerDiedOrSonDiedCoroutine);
        m_resetOptionsButtons.SetActive(false);
        m_continueButton.SetActive(false);
        StartCoroutine(LoadSpecificCheckpointCoroutine(i));
    }

    private IEnumerator LoadLastCheckpointCoroutine()
    {
        m_camera.m_state = CameraController.CAMERA_STATE.FOLLOW_TARGET_RIGHT;
        m_camera.FadeInBlankCard_Duration_Delay(2, 0);
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner(true);
        yield return new WaitForSeconds(4);
        DestroyActiveCharactersAndMarkers();
        if (m_gameTimer.gameObject.activeSelf) m_gameTimer.StartFadeTimer(1);
        SetTreasureArray_ToActiveOrInactive(m_treasuresA, false);
        SetTreasureArray_ToActiveOrInactive(m_treasuresB, false);

        yield return new WaitForSeconds(2);
        switch (m_checkpointState)
        {
            case CHECKPOINT_STATE.START:
                LoadPlayerAsChildAtStartCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_TEENAGER_REACHES_PYRAMID:
                LoadPlayerAsTeenagerReachesPyramidCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_START:
                LoadPlayerAsFatherReachesStartCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_PYRAMID:
                LoadPlayerAsFatherReachesPyramidCheckpoint();
                break;
            case CHECKPOINT_STATE.PLAYER_AS_GRANDFATHER_REACHES_START:
                LoadPlayerAsGrandfatherReachesStartCheckpoint();
                break;
        }
        SonHealthChangedAndIsNow(3);
        SetAllTreasureIndicatorsToGray();
        m_camera.FadeOutBlankCard_Duration_Delay(2, 0);
        yield return new WaitForSeconds(2);
    }

    private IEnumerator LoadSpecificCheckpointCoroutine(int i)
    {
        m_camera.m_state = CameraController.CAMERA_STATE.FOLLOW_TARGET_RIGHT;
        m_camera.FadeInBlankCard_Duration_Delay(2, 0);
        m_enemySpawnerHandler.GetComponent<EnemySpawnerHandler>().DespawnAllEnemiesAndDeactivateSpawner(true);
        yield return new WaitForSeconds(4);
        DestroyActiveCharactersAndMarkers();
        if (m_gameTimer.gameObject.activeSelf) m_gameTimer.StartFadeTimer(1);
        SetTreasureArray_ToActiveOrInactive(m_treasuresA, false);
        SetTreasureArray_ToActiveOrInactive(m_treasuresB, false);

        yield return new WaitForSeconds(2);
        switch (i)
        {
            case (int)CHECKPOINT_STATE.START:
                LoadPlayerAsChildAtStartCheckpoint();
                break;
            case (int)CHECKPOINT_STATE.PLAYER_AS_TEENAGER_REACHES_PYRAMID:
                LoadPlayerAsTeenagerReachesPyramidCheckpoint();
                break;
            case (int)CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_START:
                LoadPlayerAsFatherReachesStartCheckpoint();
                break;
            case (int)CHECKPOINT_STATE.PLAYER_AS_FATHER_REACHES_PYRAMID:
                LoadPlayerAsFatherReachesPyramidCheckpoint();
                break;
            case (int)CHECKPOINT_STATE.PLAYER_AS_GRANDFATHER_REACHES_START:
                LoadPlayerAsGrandfatherReachesStartCheckpoint();
                break;
        }
        SonHealthChangedAndIsNow(3);
        SetAllTreasureIndicatorsToGray();
        m_camera.FadeOutBlankCard_Duration_Delay(2, 0);
        yield return new WaitForSeconds(2);
    }

    public void DestroyActiveCharactersAndMarkers()
    {
        if (m_childHandler != null)
            m_childController.DestroySelf();
        if (m_teenagerHandler != null)
            m_teenagerController.DestroySelf();
        if (m_fatherHandler != null)
        {
            m_fatherController.DestroySelf();
            if(m_fatherHandler.GetComponent<SpriteDriver_Father>().m_treasurePointer != null)
                m_fatherHandler.GetComponent<SpriteDriver_Father>().m_treasurePointer.SetActive(false);
        }
        if (m_grandfatherHandler != null)
            m_grandfatherController.DestroySelf();
        var markers = GameObject.FindGameObjectsWithTag("Marker");
        foreach (var m in markers)
            Destroy(m);
        m_pyramid.HidePyramid();
    }

    public void LoadPlayerAsChildAtStartCheckpoint()
    {
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            m_pyramid.m_blockCount = 0;
            InitFather(false, null);
            InitGrandfather(false, m_fatherController);
            m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
            m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
            m_grandfatherController.UpgradeHeart();
            m_fatherCarryingGrandfather = true;
            m_fatherController.FatherGainsSilverShield();
            m_pyramid.m_powerState = Pyramid.POWER_STATE.GOLD;
            m_PlayersFatherDiedAlone = false;
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_2)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
            InitFather(false, null);
            m_PlayersFatherDiedAlone = false;
        }
        else if(m_levelState == LEVEL_STATE.LEVEL_3)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
            m_PlayersFatherDiedAlone = true;
        }
        InitChild(true, m_fatherController);
        m_tricksPointer.SetActive(true);
        SetTreasureArray_ToActiveOrInactive(m_treasuresA, true);
        m_sonDiedAsChild = false;
        m_sonDiedAsTeenager = false;
        m_sonIsReadyToBeFather = false;
    }

    public void LoadPlayerAsTeenagerReachesPyramidCheckpoint()
    {
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            m_pyramid.m_blockCount = 1;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.GOLD;
            InitFather(false, null, -200);
            m_fatherIsReadyToBeGrandfather = true;
            m_fatherCarryingGrandfather = false;
            m_grandfatherDies = true;
            m_PlayersFatherDiedAlone = false;
            m_fatherController.FatherGainsSilverShield();
            m_fatherController.FatherGainsHeart();
            m_fatherController.FatherGainsGoldHeart();
        }
        else if(m_levelState == LEVEL_STATE.LEVEL_2)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
            InitFather(false, null, -200);
            m_fatherController.FatherGainsHeart();
            m_fatherIsReadyToBeGrandfather = true;
        }
        else if(m_levelState == LEVEL_STATE.LEVEL_3)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        InitTeenager(true, null, -200);
        m_tricksPointer.SetActive(true);
        SetTreasureArray_ToActiveOrInactive(m_treasuresB, true);
    }

    public void LoadPlayerAsFatherReachesStartCheckpoint()
    {
        InitFather(true);
        m_fatherIsReadyToBeGrandfather = true;
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            if (!m_PlayersFatherDiedAlone)
            {
                InitGrandfather(false, m_fatherController);
                m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
                m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
                m_grandfatherController.UpgradeHeart();
                m_fatherController.FatherGainsSilverShield();
                m_pyramid.m_blockCount = 1;
                m_pyramid.m_powerState = Pyramid.POWER_STATE.GOLD;
            }
            else
            {
                m_pyramid.m_blockCount = 3;
                m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
            }
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_2)
        {
            if (!m_PlayersFatherDiedAlone)
            {
                InitGrandfather(false, m_fatherController);
                m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
                m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
            }
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_3)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        m_tricksPointer.SetActive(true);
        SetTreasureArray_ToActiveOrInactive(m_treasuresA, true);
    }

    public void LoadPlayerAsFatherReachesPyramidCheckpoint()
    {
        InitFather(true, null, -200);
        m_fatherIsReadyToBeGrandfather = true;
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            if (!m_PlayersFatherDiedAlone)
            {
                m_fatherController.FatherGainsHeart();
                m_fatherController.FatherGainsGoldHeart();
                m_fatherController.FatherGainsSilverShield();
                m_pyramid.m_blockCount = 2;
                m_pyramid.m_powerState = Pyramid.POWER_STATE.GOLD;
            }
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_2)
        {
            m_fatherController.FatherGainsHeart();
            m_pyramid.m_blockCount = 4;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_3)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        if (!m_sonDiedAsChild)
        {
            InitTeenager(false, null, -200);
        }
        m_tricksPointer.SetActive(true);
        SetTreasureArray_ToActiveOrInactive(m_treasuresB, true);
    }

    public void LoadPlayerAsGrandfatherReachesStartCheckpoint()
    {
        InitFather(false);
        InitChild(false);
        InitGrandfather(true, m_fatherController);
        m_grandfatherController.transform.SetParent(m_fatherController.gameObject.transform);
        m_grandfatherController.transform.localPosition = m_grandfatherController.m_carriedByFatherOffset;
        m_fatherCarryingGrandfather = true;
        if (m_levelState == LEVEL_STATE.LEVEL_1)
        {
            if (!m_PlayersFatherDiedAlone)
            {
                m_grandfatherController.UpgradeHeart();
                m_fatherController.FatherGainsSilverShield();
                m_pyramid.m_blockCount = 2;
                m_pyramid.m_powerState = Pyramid.POWER_STATE.GOLD;
            }
            else
            {
                m_pyramid.m_blockCount = 3;
                m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
            }
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_2)
        {
            if (!m_PlayersFatherDiedAlone)
            {
                m_pyramid.m_blockCount = 4;
            }
            else
            {
                m_pyramid.m_blockCount = 3;
            }
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        else if (m_levelState == LEVEL_STATE.LEVEL_3)
        {
            m_pyramid.m_blockCount = 3;
            m_pyramid.m_powerState = Pyramid.POWER_STATE.BRONZE;
        }
        m_tricksPointer.SetActive(true);
        SetTreasureArray_ToActiveOrInactive(m_treasuresA, true);
    }

    public void TimerRanOut()
    {
        Debug.Log("Time ran out");
        if (m_childController != null) ChildDies();
        if (m_teenagerController != null) TeenagerDies();
    }
    public void ActivateGameTimerWithSeconds(int seconds = 180)
    {
        if(!m_gameTimer.gameObject.activeSelf)
            m_gameTimer.gameObject.SetActive(true);
        m_gameTimer.StartTimer(seconds);
    }

    public void KillSon()
    {
        if (m_childHandler != null) ChildDies();
        if (m_teenagerHandler != null) TeenagerDies();
    }
}
