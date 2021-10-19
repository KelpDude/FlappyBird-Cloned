using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float CameraOrthoSize = 50f;
    private const float PipeWidth = 9f;
    private const float PipeHeadHeight = 3.75f;
    private const float PipeMoveSpeed = 30f;
    private const float PipeDestroyXPosition = -150f;
    private const float PipeSpawnXPosition = +150f;
    private const float GroundDestroyXPosition = -300f;
    private const float CloudDestroyXPosition = -225f;
    private const float CloudSpawnXPosition = +225f;
    private const float CloudSpawnYPosition = 25f;
    private const float BirdXPosition = 0f; 

    private static Level instance;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake()
    {
        instance = this;
        SpawnInitialGround();
        SpawnInitialClouds();
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStart += Bird_OnStart;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        //CMDebug.TextPopupMouse("Dead!");
        state = State.BirdDead;
    }

    private void Bird_OnStart(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Update()
    {
        if (state == State.Playing)
        {
            PipeMovement();
            PipeSpawning();
            HandleGround();
            HandleClouds();
        }
    }

    private void SpawnInitialClouds()
    {
        cloudList = new List<Transform>();
        Transform cloudTransform;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, CloudSpawnYPosition, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private Transform GetCloudPrefabTransform()
    {
        switch (Random.Range(0, 3))
        {
            default:
            case 0: return GameAssets.GetInstance().pfCloud1;
            case 1: return GameAssets.GetInstance().pfCloud2;
            case 2: return GameAssets.GetInstance().pfCloud3;
        }
    }

    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47.5f;
        float groundWidth = 300f;
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleClouds()
    {
        //Handles Cloud Spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0)
        {
            //Time to spawn another cloud
            float cloudSpawnTimerMax = 6f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, CloudSpawnYPosition, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }

        //Handle Cloud Moving
        for (int i = 0; i < cloudList.Count; i++)
        {
            Transform cloudTransform = cloudList[i];
            {
                //Move clouds by less speed than pipes for a Parallax effect
                cloudTransform.position += new Vector3(-1, 0, 0) * PipeMoveSpeed * Time.deltaTime * .7f;

                if (cloudTransform.position.x < CloudDestroyXPosition)
                {
                    //Cloud past destroy point, destroy self
                    Destroy(cloudTransform.gameObject);
                    cloudList.RemoveAt(i);
                    i--;
                }
            }
        }
    }


        private void HandleGround()
        {
            foreach (Transform groundTransform in groundList)
            {
                groundTransform.position += new Vector3(-1, 0, 0) * PipeMoveSpeed * Time.deltaTime;

                if (groundTransform.position.x < GroundDestroyXPosition)
                {
                    //Ground passed the left side, relocate to the right side
                    //Finds the right-most X position
                    float rightMostXPosition = -150f;
                    for (int i=0; i<groundList.Count; i++)
                    {
                        if (groundList[i].position.x > rightMostXPosition)
                        {
                            rightMostXPosition = groundList[i].position.x;
                        }
                    }
                    //Place Ground on the right-most position
                    float groundWidth = 300f;
                    //groundTransform.position = new Vector3(rightMostXPosition + groundWidth, groundTransform.y, groundTransform.z);
                }
            }
        }

    private void PipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];

            bool isToTheRightOfBird = pipe.GetXPosition() > BirdXPosition;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= BirdXPosition && pipe.IsBottom())
            {
                //pipe passes bird
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }

            if (pipe.GetXPosition() < PipeDestroyXPosition)
            {
                //Destroy Pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void PipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            //spawns another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CameraOrthoSize * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight); 
            CreateGapPipes(height, gapSize, PipeSpawnXPosition);
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
        case Difficulty.Easy:
            gapSize = 50f;
            pipeSpawnTimerMax = 1.4f;
            break;
        case Difficulty.Medium:
            gapSize = 40f;
            pipeSpawnTimerMax = 1.3f;
            break;
        case Difficulty.Hard:
            gapSize = 33f;
            pipeSpawnTimerMax = 1.1f;
            break;
        case Difficulty.Impossible:
            gapSize = 24f;
            pipeSpawnTimerMax = 1.0f;
            break;
    }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawned >= 30) return Difficulty.Impossible;
        if (pipesSpawned >= 20) return Difficulty.Hard;
        if (pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;

    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CameraOrthoSize * 2f - gapY - gapSize * .5f, xPosition, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty()); 
    }

    private void CreatePipe(float height, float xPosition, bool isBottom)
    {
        // Set up the Pipe Head
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (isBottom)
        {
            pipeHeadYPosition = -CameraOrthoSize + height - PipeHeadHeight * .5f;
        }
        else
        {
            pipeHeadYPosition = +CameraOrthoSize - height + PipeHeadHeight * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        // Set-up the Pipe Body 
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (isBottom)
        {
            pipeBodyYPosition = -CameraOrthoSize;
        }
        else
        {
            pipeBodyYPosition = +CameraOrthoSize;
            pipeBody.localScale = new Vector3(.8f, -1, 1); 
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PipeWidth, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PipeWidth, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, isBottom);
        pipeList.Add(pipe);
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount;
    }

    // represents a single Pipe 
    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PipeMoveSpeed * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PipeMoveSpeed * Time.deltaTime;
        }
        
        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
