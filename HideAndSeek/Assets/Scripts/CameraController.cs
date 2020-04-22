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
        // first variant
        // transform.eulerAngles = new Vector3(33.09f, -15f, 0f);
        // _deltaChasingObjPos = new Vector3(2.0f, 7.9f, -6f);

        // second variant
        //   transform.eulerAngles = new Vector3(55.14f, -15f, 0f);
        //    _deltaChasingObjPos = new Vector3(2.0f, 12.6f, -6f);

        // third variant
      //   transform.eulerAngles = new Vector3(46.2f, -15f, 0f);
      //   _deltaChasingObjPos = new Vector3(4.31f, 30f, -18.3f);

        // fourth variant
         transform.eulerAngles = new Vector3(46.2f, -15f, 0f);
         _deltaChasingObjPos = new Vector3(3.89f, 18.6f, -13f);

        // _deltaChasingObjPos = transform.position - _chasingObj.transform.position; 


        StartCoroutine("FadeOverlappingObjects");
    }

    private void Update() {
        if (_chasingObj != null) {
            transform.position = Vector3.Lerp(transform.position, _chasingObj.transform.position + _deltaChasingObjPos, _cameraChasingLerp);
        } /*else {
            GameObject player = GameObject.Find("MyPlayer");
            if (player != null)
                SetChasingObject(player);
        }*/
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
                return fadedGameObject.GetComponentInChildren<Renderer>();
            }
        } else {
            fadedGameObject = null;
        //    Debug.DrawRay(transform.position, dir * 10000, Color.white);
            return null;
        }
    }
}
