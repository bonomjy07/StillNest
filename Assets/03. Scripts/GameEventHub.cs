using System;
using UniRx;
using UnityEngine;

public class GameEventHub : Singleton<GameEventHub>
{
    private readonly Subject<Unit> _onHeroSpawnButtonClick = new();
    private readonly Subject<CoopPlayer> _onLocalClient = new();

    public IObservable<Unit> OnHeroSpawnButtonClick => _onHeroSpawnButtonClick;
    public IObservable<CoopPlayer> OnLocalClient => _onLocalClient;

    public void RaiseHeroSpawnClick() => _onHeroSpawnButtonClick.OnNext(Unit.Default);
    public void RaiseLocalClientOn(CoopPlayer client) => _onLocalClient.OnNext(client);
}
