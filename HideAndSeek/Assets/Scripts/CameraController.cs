using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private GameObject _chasingObj;

    [SerializeField]
    private CameraData cameraData;

    private Renderer _fadedRenderer = null;
    private GameObject _fadedGameObject = null; // optimization

    private void Start() {
        transform.eulerAngles = cameraData.eulerAngles;
        
        StartCoroutine("FadeOverlappingObjects");
    }
    
    private void LateUpdate() {
        if (_chasingObj != null) {
            transform.position = Vector3.Lerp(transform.position, _chasingObj.transform.position + cameraData.deltaChasingObjPos, cameraData.cameraChasingLerp);
        }
    }

    public void SetChasingObject(GameObject chasingObj) {
        _chasingObj = chasingObj;
    }
    
    private IEnumerator FadeOverlappingObjects() {
        while (_chasingObj == null) {
            yield return new WaitForSeconds(.4f);
        }

        int layerMask = LayerMask.GetMask("Obstacle"); // = 1 << 9;

        yield return new WaitForSeconds(2f); // for setting delta from chasing... to camera(otherwise it doesn't have time)
        Vector3 dir = _chasingObj.transform.position - transform.position;

        while (true) {
            Renderer r = _CheckOverlapping(dir, layerMask);

            if (r != _fadedRenderer) {
                if (_fadedRenderer != null) {
                    _fadedRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                
                if (r != null) {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }

                _fadedRenderer = r;
            }

            yield return new WaitForSeconds(.2f);
        }
    }

    private Renderer _CheckOverlapping(Vector3 dir, int layerMask) {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, layerMask)) {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
            if (hit.collider.gameObject == _fadedGameObject) {
                return _fadedRenderer;
            } else {
                _fadedGameObject = hit.collider.gameObject;
                return _fadedGameObject.GetComponentInChildren<Renderer>();
            }
        } else {
            _fadedGameObject = null;
        //    Debug.DrawRay(transform.position, dir * 10000, Color.white);
            return null;
        }
    }
}
