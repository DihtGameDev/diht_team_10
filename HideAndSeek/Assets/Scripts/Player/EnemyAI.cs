using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


public class EnemyAI : MonoBehaviour {

    [Header("Set in inspector")]
    [SerializeField]
    private NavMeshAgent _agent;
    public EnemyAIData enemyAIData;

    private float _angleDiff = 0f;
    private Vector3 _prevAngle = Vector3.zero;

    private Vector3 _targetLastEulerAngles = Vector3.zero;
    private List<Transform> _visibleTargets = new List<Transform>(); // optimization

    public float checkVisibleTargetsDelay = 0.2f;
    public float EPS = 0.01f;

    public bool hasGoal = false;

    protected void Start() {
        tag = Constants.ENEMY_SEEKER_AI_TAG;
        
        if (_agent == null) {
            _agent = GetComponent<NavMeshAgent>();
        }

        StartCoroutine(Misc.WaitWhile(
            () => { return (_agent == null && enemyAIData == null); },
            () => { StartCoroutine(Misc.LoopWithDelay(checkVisibleTargetsDelay, TargetSetting)); },
            0.5f
        ));
        
    }

    protected void Update() {
        AIMovement();
    }

    public void SetEnemyAIData(EnemyAIData data) {
        enemyAIData = data;
    }

    private void TargetSetting() {
        FindAndSetVisibleTargets();

        if (_visibleTargets.Count > 0) {
            hasGoal = true;
            Transform targetTransform = ChooseNearestVisibleTarget(_visibleTargets);
            _agent.SetDestination(targetTransform.position);
            _targetLastEulerAngles = targetTransform.eulerAngles;
            _angleDiff = transform.eulerAngles.y - _prevAngle.y;
            _prevAngle = transform.eulerAngles;
        }
    }

    private void AIMovement() {
        if (CameToDestination()) {
            if (hasGoal) {
                float endAngle = (_targetLastEulerAngles.y + enemyAIData.addingAngleWhenCame * Mathf.Sign(_angleDiff)) % 360f;
                float angleBefore = transform.eulerAngles.y;
                transform.eulerAngles =
                            new Vector3(
                                0,
                                Mathf.LerpAngle(transform.eulerAngles.y, endAngle, Time.deltaTime * enemyAIData.lerpAngleCoeff),
                                0
                            );

                if (Mathf.Abs(transform.eulerAngles.y - angleBefore) < EPS) {
                    hasGoal = false;
                }
            } else {
                _agent.SetDestination(GenerateRandomDestination());
            }
        }
    }

    private Vector3 GenerateRandomDestination() {
        Vector3 randomDirection =
            Random.insideUnitSphere * Random.Range(enemyAIData.minPathfinderRadius, enemyAIData.maxPathfinderRadius);

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(transform.position + randomDirection, out hit, enemyAIData.viewRadius, 1)) {  // find closest point to navmesh
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    private List<Transform> FindAndSetVisibleTargets() {
        _visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, enemyAIData.viewRadius, enemyAIData.targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++) {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < enemyAIData.viewAngle / 2) {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, enemyAIData.obstacleMask)) {
                    Debug.DrawRay(transform.position, dirToTarget * enemyAIData.viewRadius);
                    _visibleTargets.Add(target);
                }
            }
        }

        return _visibleTargets;
    }

    private Transform ChooseNearestVisibleTarget(List<Transform> visibleTargets) {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < visibleTargets.Count; ++i) {
            float dstToTarget = Vector3.Distance(transform.position, visibleTargets[i].position);
            if (minDistance > dstToTarget) {
                minDistance = dstToTarget;
                nearestTarget = visibleTargets[i];
            }
        }

        return nearestTarget;
    }

    private bool CameToDestination() {
        return Vector3.Distance(transform.position, _agent.destination) < EPS;
    }

    protected void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) {
            case Constants.HIDEMAN_TAG: {
                PhotonView pv = other.GetComponent<PhotonView>();  // send message on this photonview hit.gameObject object, i.e. same photon view
                pv.RPC("KillHideman", RpcTarget.All, PhotonNetwork.NickName + "_AI", pv.Owner.NickName); // we send all users that user with this nickname killed by this user
                break;
            }
        }
    }
}
