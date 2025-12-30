using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Updater
{
    public abstract UpdaterType GetUpdaterType();
    public abstract void Init();
    public abstract void OnEveryFrame();
    public abstract void LimitedUpdate();
}
public enum UpdaterType
{
    User,
    AI,
    Other
}