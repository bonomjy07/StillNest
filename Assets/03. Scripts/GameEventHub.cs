using System;
using UniRx;
using UnityEngine;

public class GameEventHub : Singleton<GameEventHub>
{
    private readonly Subject<Unit> _onHeroSpawnButtonClick = new();

    public IObservable<Unit> OnHeroSpawnButtonClick => _onHeroSpawnButtonClick;

    public void RaiseHeroSpawnClick()
    {
        _onHeroSpawnButtonClick.OnNext(Unit.Default);
    }
}
