using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/FlareAbility", fileName = "FlareAbiliy")]
public class FlareAbility : AbstractAbility {
    public float cooldown = 8;
    public float flareSpeed = 10f;
    public float flareRadius = 20f;
    public float flareDuration = 4f;

    public static string FLARE_TYPE_OP = "FlareAbility";

    [SerializeField]
    private GameObject _flarePrefab;

    private FieldOfView _flareFov;

    protected void OnEnable() {
    }

    override public void UseAbility() {
        Debugger.Log("UseAbility");

        // flare instantiation
        GameObject flareObj = GameManager.instance.objectsPool.Get(FLARE_TYPE_OP);
        if (flareObj == null) {
            flareObj = Instantiate(
                _flarePrefab,
                GameManager.instance.mainPlayer.transform.position,
                Quaternion.Euler(GameManager.instance.mainPlayer.transform.eulerAngles)
            );
        } else {
            // otherwise flare start moving from previous position and rotation
            flareObj.transform.position = GameManager.instance.mainPlayer.transform.position;
            flareObj.transform.eulerAngles = GameManager.instance.mainPlayer.transform.eulerAngles;
        }

        if (_flareFov == null) {
            _flareFov = flareObj.GetComponentInChildren<FieldOfView>();
        }

        _flareFov.viewRadius = flareRadius;

        GameManager.instance.StartCoroutine(FlareMovement(flareObj));
    }

    public IEnumerator FlareMovement(GameObject flareObject) {
        Debugger.Log("FlareMovement");

        float startTime = Time.time;

        GameManager.instance.cameraController.SetChasingObject(flareObject);

        while (Time.time - startTime < flareDuration) {
            flareObject.transform.localPosition +=
                flareObject.transform.TransformDirection(Vector3.forward) * Time.deltaTime * flareSpeed;  // move toward local z-axis
            yield return null;
        }

        GameManager.instance.cameraController.SetChasingObject(GameManager.instance.mainPlayer);

        DestroyFlare(flareObject);
    }

    private void DestroyFlare(GameObject flareObject) {
        GameManager.instance.objectsPool.Add(FLARE_TYPE_OP, flareObject);
    }

    override public float Cooldown() {
        return cooldown;
    }
}
