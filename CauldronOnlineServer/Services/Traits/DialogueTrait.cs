using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class DialogueTrait : WorldTrait
    {
        public override bool Instant => true;

        private DialogueParameter _parameter = new DialogueParameter();

        public DialogueTrait(DialogueParameter parameter)
        {
            _parameter = parameter;
        }

        public DialogueTrait(WorldTraitData data) : base(data)
        {
            if (data is DialogueTraitData dialogue)
            {
                _parameter.Dialogue = dialogue.Dialogue;
                _parameter.Hitbox = dialogue.Hitbox;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}