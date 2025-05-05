using Record;
using UnityEngine;
using Spell.Model.Core;

namespace Spell.Dev.UI
{
    public class DevSpellUIBootstrapper : MonoBehaviour
    {
        [SerializeField] private DevSpellUIView view;

        private void Awake()
        {
            var spellDataController = new SpellDataController();
            var recordController = new RecordController();

            var presenter = new DevSpellUIPresenter(view, spellDataController, recordController);
            view.Initialize(presenter);
        }
    }
}
