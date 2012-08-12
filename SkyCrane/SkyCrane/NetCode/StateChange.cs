using System;
using System.Collections.Generic;
using System.IO;

namespace SkyCrane.NetCode
{
    public enum StateChangeType { MOVED, CREATE_PLAYER_CHARACTER, SET_PLAYER, CREATE_ENTITY, DELETE_ENTITY, CHANGE_SPRITE}
    public enum StateProperties { ENTITY_ID, POSITION_X, POSITION_Y, SPRITE_NAME, ANIMATION_NAME, DRAW_PRIORITY, FRAME_WIDTH }

    public class StateChange : Marshable
    {
        public StateChangeType type;
        public Dictionary<StateProperties, int> intProperties = new Dictionary<StateProperties, int>();
        public Dictionary<StateProperties, String> stringProperties = new Dictionary<StateProperties, String>();

        public StateChange() { }

        public byte[] getPacketData()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bb = new BinaryWriter(ms);
            bb.Write((byte)type);

            bb.Write((byte)intProperties.Count);
            foreach (KeyValuePair<StateProperties, int> kvp in intProperties)
            {
                bb.Write((byte)kvp.Key);
                bb.Write((int)kvp.Value);
            }

            bb.Write((byte)stringProperties.Count);
            foreach (KeyValuePair<StateProperties, String> kvp in stringProperties)
            {
                bb.Write((byte)kvp.Key);
                bb.Write(kvp.Value);
            }

            return ms.ToArray();
        }

        public StateChange(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            BinaryReader br = new BinaryReader(ms);

            this.type = (StateChangeType)br.ReadByte();

            byte nums = br.ReadByte();
            for (int i = 0; i < nums; i++)
            {
                this.intProperties[(StateProperties)br.ReadByte()] = br.ReadInt32();
            }

            nums = br.ReadByte();
            for (int i = 0; i < nums; i++)
            {
                this.stringProperties[(StateProperties)br.ReadByte()] = br.ReadString();
            }
        }

        public static StateChange getStateData(byte[] b)
        {
            StateChange ret = new StateChange(b);

            return ret;
        }
    }
}
