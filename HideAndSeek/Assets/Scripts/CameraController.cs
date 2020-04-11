using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private GameObject _chasingObj;

    private Vector3 _deltaChasingObjPos;
    private float _cameraChasingLerp = 0.125f;

    private Renderer fadedRenderer = null;
    private GameObject fadedGameObject = null; // optimization

    private void Start() {
        if (_chasingObj == null) {
            SetChasingObject(GameObject.FindGameObjectsWithTag("Player")[0]);
        } else {
            _deltaChasingObjPos = transform.position - _chasingObj.transform.position;
        }

        StartCoroutine("FadeOverlappingObjects");
    }

    private void Update() {
        if (_chasingObj != null) {
            transform.position = Vector3.Lerp(transform.position, _chasingObj.transform.position + _deltaChasingObjPos, _cameraChasingLerp);
        }
    }

    private void SetChasingObject(GameObject chasingObj) {
        _chasingObj = chasingObj;
        _deltaChasingObjPos = transform.position - _chasingObj.transform.position;
    }
    
    private IEnumerator FadeOverlappingObjects() {
        int layerMask = LayerMask.GetMask("Obstacle"); // = 1 << 9;

        Vector3 dir = _chasingObj.transform.position - transform.position;
        dir = dir / dir.magnitude; // normalize

        while (true) {
            Renderer r = _CheckOverlapping(dir, layerMask);

            if (r != fadedRenderer) {
                if (fadedRenderer != null) {
                    fadedRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                
                if (r != null) {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }

                fadedRenderer = r;
            }

            yield return new WaitForSeconds(.2f);
        }
    }

    private Renderer _CheckOverlapping(Vector3 dir, int layerMask) {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, layerMask)) {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
            if (hit.collider.gameObject == fadedGameObject) {
                return fadedRenderer;
            } else {
                fadedGameObject = hit.collider.gameObject;
                return fadedGameObject.GetComponent<Renderer>();
            }
        } else {
            fadedGameObject = null;
            Debug.DrawRay(transform.position, dir * 1000, Color.white);
            return null;
        }
    }
}
