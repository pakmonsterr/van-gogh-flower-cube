using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Interaction.Input;
using UnityEngine.Assertions;

namespace Oculus.Interaction.Samples
{
    public class palm_menu : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHmd))]
        private UnityEngine.Object _hmd;
        private IHmd Hmd { get; set; }

        [SerializeField]
        private ActiveStateSelector[] _poses;

        [SerializeField]
        private Material[] _onSelectIcons;

        [SerializeField]
        private GameObject _poseActiveVisualPrefab;

        private GameObject[] _poseActiveVisuals;

        protected virtual void Awake()
        {
            Hmd = _hmd as IHmd;
        }

        protected virtual void Start()
        {
            this.AssertField(Hmd, nameof(Hmd));
            this.AssertField(_poseActiveVisualPrefab, nameof(_poseActiveVisualPrefab));

            _poseActiveVisuals = new GameObject[_poses.Length];
            _poseActiveVisuals[0] = Instantiate(_poseActiveVisualPrefab);
            _poseActiveVisuals[0].GetComponentInChildren<TextMeshPro>().text = _poses[0].name;
            _poseActiveVisuals[0].GetComponentInChildren<ParticleSystemRenderer>().material = _onSelectIcons[0];
            _poseActiveVisuals[0].SetActive(false);

            _poses[0].WhenSelected += () => ShowVisuals();
            _poses[0].WhenUnselected += () => HideVisuals();
        }
        private void ShowVisuals()
        {
            if (!Hmd.TryGetRootPose(out Pose hmdPose))
            {
                return;
            }

            Vector3 spawnSpot = hmdPose.position + hmdPose.forward;
            _poseActiveVisuals[0].transform.position = spawnSpot;
            _poseActiveVisuals[0].transform.LookAt(2 * _poseActiveVisuals[0].transform.position - hmdPose.position);

            var hands = _poses[0].GetComponents<HandRef>();
            Vector3 visualsPos = Vector3.zero;
            foreach (var hand in hands)
            {
                hand.GetRootPose(out Pose wristPose);
                Vector3 forward = hand.Handedness == Handedness.Left ? wristPose.right : -wristPose.right;
                visualsPos += wristPose.position + forward * .15f + Vector3.up * .02f;
            }
            _poseActiveVisuals[0].transform.position = visualsPos / hands.Length;
            _poseActiveVisuals[0].gameObject.SetActive(true);
        }

        private void HideVisuals()
        {
            _poseActiveVisuals[0].gameObject.SetActive(false);
        }
    }
}
