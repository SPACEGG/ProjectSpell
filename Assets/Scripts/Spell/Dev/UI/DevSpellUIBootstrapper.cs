using Record;
using UnityEngine;

namespace Spell.Dev.UI
{
    public class DevSpellUIBootstrapper : MonoBehaviour
    {
        [SerializeField] private DevSpellUIView view;

        private void Awake()
        {
            var spellController = new SpellDataController();
            var recordController = new RecordController();

            var presenter = new DevSpellUIPresenter(view, spellController, recordController);
            view.Initialize(presenter);
        }
    }
}
