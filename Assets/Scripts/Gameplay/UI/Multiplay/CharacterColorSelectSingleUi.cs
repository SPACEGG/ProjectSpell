using Multiplay;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Multiplay
{
    public class CharacterColorSelectSingleUi : MonoBehaviour
    {
        [SerializeField] private int colorId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;

        private ProjectSpellGameMultiplayer ProjectSpellGameMultiplayer => ProjectSpellGameMultiplayer.Singleton;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { ProjectSpellGameMultiplayer.ChangePlayerColor(colorId); });
        }

        private void Start()
        {
            ProjectSpellGameMultiplayer.OnPlayerInfosChanged += ProjectSpellGameMultiplayer_OnPlayerInfosChanged;
            image.color = ProjectSpellGameMultiplayer.GetPlayerColor(colorId);
            UpdateIsSelected();
        }

        private void ProjectSpellGameMultiplayer_OnPlayerInfosChanged(object sender, System.EventArgs e)
        {
            UpdateIsSelected();
        }

        private void UpdateIsSelected()
        {
            var isSelected = ProjectSpellGameMultiplayer.GetPlayerInfo().ColorId == colorId;
            selectedGameObject.SetActive(isSelected);
        }

        private void OnDestroy()
        {
            ProjectSpellGameMultiplayer.OnPlayerInfosChanged -= ProjectSpellGameMultiplayer_OnPlayerInfosChanged;
        }
    }
}