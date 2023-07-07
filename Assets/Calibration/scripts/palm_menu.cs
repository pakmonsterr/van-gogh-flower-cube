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
            for (int i = 0; i < _poses.Length; i++)
            {
                _poseActiveVisuals[i] = Instantiate(_poseActiveVisualPrefab);
                _poseActiveVisuals[i].GetComponentInChildren<TextMeshPro>().text = _poses[i].name;
                _poseActiveVisuals[i].GetComponentInChildren<ParticleSystemRenderer>().material = _onSelectIcons[i];
                _poseActiveVisuals[i].SetActive(false);

                int poseNumber = i;
                _poses[i].WhenSelected += () => ShowVisuals(poseNumber);
                _poses[i].WhenUnselected += () => HideVisuals(poseNumber);
            }
        }
        private void ShowVisuals(int poseNumber)
        {
            if (!Hmd.TryGetRootPose(out Pose hmdPose))
            {
                return;
            }

            Vector3 spawnSpot = hmdPose.position + hmdPose.forward;
            _poseActiveVisuals[poseNumber].transform.position = spawnSpot;
            _poseActiveVisuals[poseNumber].transform.LookAt(2 * _poseActiveVisuals[poseNumber].transform.position - hmdPose.position);

            var hands = _poses[poseNumber].GetComponents<HandRef>();
            Vector3 visualsPos = Vector3.zero;
            foreach (var hand in hands)
            {
                hand.GetRootPose(out Pose wristPose);
                Vector3 forward = hand.Handedness == Handedness.Left ? wristPose.right : -wristPose.right;
                visualsPos += wristPose.position + forward * .15f + Vector3.up * .02f;
            }
            _poseActiveVisuals[poseNumber].transform.position = visualsPos / hands.Length;
            _poseActiveVisuals[poseNumber].gameObject.SetActive(true);
        }

        private void HideVisuals(int poseNumber)
        {
            _poseActiveVisuals[poseNumber].gameObject.SetActive(false);
        }
    }
}
