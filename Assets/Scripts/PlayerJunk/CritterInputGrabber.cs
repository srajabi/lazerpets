﻿using UnityEngine;

public class CritterInputGrabber
{
    readonly float mouseSensitivity;

    CritterInputPacket lastTickFrame;
    CritterInputPacket thisTickFrame;

    float yawDegrees;
    float pitchDegrees;

    public CritterInputGrabber(float mouseSensitivity = 4f)
    {
        this.mouseSensitivity = mouseSensitivity;

        thisTickFrame = new CritterInputPacket {
            headOrientation = Quaternion.identity
        };

        lastTickFrame = thisTickFrame;
    }

    Quaternion updateOrientation()
    {
        yawDegrees += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitchDegrees -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitchDegrees = Mathf.Clamp(pitchDegrees, -89f, 89f);
        return Quaternion.Euler(pitchDegrees, yawDegrees, 0f);
    }

    static bool immediateInputToTick(bool thisTick, bool lastTick, bool thisImmediate)
    {
        if (lastTick != thisTick) return thisTick;
        if (thisImmediate != lastTick) return thisImmediate;
        return thisTick;
    }

    public CritterInputPacket UpdateImmediate()
    {
        var thisImmediateFrame = new CritterInputPacket {
            headOrientation = updateOrientation(),
            forward   = Input.GetKey(KeyCode.W),
            backward  = Input.GetKey(KeyCode.S),
            leftward  = Input.GetKey(KeyCode.A),
            rightward = Input.GetKey(KeyCode.D),
            jump      = Input.GetKey(KeyCode.Space),
        };

        thisTickFrame.forward   = immediateInputToTick(thisTickFrame.forward,   lastTickFrame.forward,   thisImmediateFrame.forward);
        thisTickFrame.backward  = immediateInputToTick(thisTickFrame.backward,  lastTickFrame.backward,  thisImmediateFrame.backward);
        thisTickFrame.leftward  = immediateInputToTick(thisTickFrame.leftward,  lastTickFrame.leftward,  thisImmediateFrame.leftward);
        thisTickFrame.rightward = immediateInputToTick(thisTickFrame.rightward, lastTickFrame.rightward, thisImmediateFrame.rightward);
        thisTickFrame.jump      = immediateInputToTick(thisTickFrame.jump,      lastTickFrame.jump,      thisImmediateFrame.jump);

        return thisImmediateFrame;
    }

    public CritterInputPacket UpdateTick()
    {
        var result = thisTickFrame;
        lastTickFrame = thisTickFrame;
        return result;
    }
}