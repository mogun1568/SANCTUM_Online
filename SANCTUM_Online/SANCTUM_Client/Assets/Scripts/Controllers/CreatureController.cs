using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public int Id { get; set; }

    PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
            {
                return;
            }

            Pos = new Vector3Int(value.PosX, 0, value.PosZ);
        }
    }

    public Vector3Int Pos
    {
        get
        {
            return new Vector3Int(PosInfo.PosX, 0, PosInfo.PosZ);
        }

        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosZ == value.z)
            {
                return;
            }

            PosInfo.PosX = value.x;
            PosInfo.PosZ = value.z;
        }
    }
}
