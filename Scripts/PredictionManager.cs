using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PredictionManager : MonoBehaviour
{
    public int maxIterations;

    private Scene _currentScene;
    private Scene _predictionScene;

    private PhysicsScene _currentPhysicsScene;
    private PhysicsScene _predictionPhysicsScene;

    public List<GameObject> dummyObstacles = new List<GameObject>();

    private LineRenderer _lineRenderer;
    private GameObject _dummy;

    public static PredictionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start(){
        Physics.autoSimulation = false;

        _currentScene = SceneManager.GetActiveScene();
        _currentPhysicsScene = _currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        _predictionScene = SceneManager.CreateScene("Prediction", parameters);
        _predictionPhysicsScene = _predictionScene.GetPhysicsScene();

        _lineRenderer = GetComponent<LineRenderer>();
        
        CopyAllObstacles();
    }

    private void FixedUpdate()
    {
        if (_currentPhysicsScene.IsValid()){
            _currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    public void CopyAllObstacles()
    {
        GameObject[] array = dummyObstacles.ToArray();
        
        foreach(GameObject everyObstacle in array)
        {
            if(everyObstacle.gameObject.GetComponent<Collider>() != null)
            {
                GameObject fakeT = Instantiate(everyObstacle.gameObject);
                fakeT.transform.position = everyObstacle.transform.position;
                fakeT.transform.rotation = everyObstacle.transform.rotation;
                Renderer fakeR = fakeT.GetComponent<Renderer>();
                
                if(fakeR)
                {
                    fakeR.enabled = false;
                }
                
                SceneManager.MoveGameObjectToScene(fakeT, _predictionScene);
                dummyObstacles.Add(fakeT);
            }
        }
    }

    void KillAllObstacles()
    {
        foreach(var o in dummyObstacles){
            Destroy(o);
        }
        
        dummyObstacles.Clear();
    }

    public void Predict(GameObject subject, Vector3 currentPosition, Vector3 force)
    {
        if (_currentPhysicsScene.IsValid() && _predictionPhysicsScene.IsValid())
        {
            if(_dummy == null)
            {
                _dummy = Instantiate(subject);
                SceneManager.MoveGameObjectToScene(_dummy, _predictionScene);
            }

            _dummy.transform.position = currentPosition;
            _dummy.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            _lineRenderer.positionCount = 0;
            _lineRenderer.positionCount = maxIterations;


            for (int i = 0; i < maxIterations; i++){
                _predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                _lineRenderer.SetPosition(i, _dummy.transform.position);
            }

            Destroy(_dummy);
        }
    }

    private void OnDestroy()
    {
        KillAllObstacles();
    }
}
