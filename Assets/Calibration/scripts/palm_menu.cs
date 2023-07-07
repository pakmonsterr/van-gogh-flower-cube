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
        public TMP_Text debug_text;
        
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

            _poses[0].WhenSelected += () => palmUp();
            _poses[0].WhenUnselected += () => palmDown();
        }
        
        private void palmUp()
        {
            debug_text.text = "palm up";
        }

        private void palmDown()
        {
            debug_text.text = "palm down";
        }
    }
}
