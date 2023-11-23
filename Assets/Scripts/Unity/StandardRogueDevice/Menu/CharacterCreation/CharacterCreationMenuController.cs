using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class CharacterCreationMenuController : MonoBehaviour
    {
        private readonly CharacterCreationDatabase database;
        private readonly ScrollModelsMenuView scrollMenuView;
        private readonly ModelsMenuView commandMenuView;

        private List<object> models = new List<object>();

        public CharacterCreationMenuController(CharacterCreationDatabase database, ScrollModelsMenuView scrollMenuView, ModelsMenuView commandMenuView)
        {
            this.database = database;
            this.scrollMenuView = scrollMenuView;
            this.commandMenuView = commandMenuView;
        }

        public void Open(IModelsMenuRoot root, Spanning<RogueObj> lobbyMembers)
        {
            //scrollMenuView.OpenView(null, database.Presets, root, null, null, RogueMethodArgument.Identity);
        }
    }
}
