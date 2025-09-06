using System;
using UniRx;
using UnityEngine;

public class GameEventHub : Singleton<GameEventHub>
{
    private readonly Subject<Unit> _onHeroSpawnButtonClick = new();
    private readonly Subject<CoopPlayer> _onLocalClient = new();

    private readonly Subject<Unit> _onDamageUp = new();
    private readonly Subject<Unit> _onAttackSpeedUp = new();
    
    public IObservable<Unit> OnHeroSpawnButtonClick => _onHeroSpawnButtonClick;
    public IObservable<CoopPlayer> OnLocalClient => _onLocalClient;

    public IObservable<Unit> OnDamageUp => _onDamageUp;
    public IObservable<Unit> OnAttackSpeedUp => _onAttackSpeedUp;

    public void RaiseHeroSpawnClick() => _onHeroSpawnButtonClick.OnNext(Unit.Default);
    public void RaiseLocalClientOn(CoopPlayer client) => _onLocalClient.OnNext(client);

    public void RaiseDamageUpClick() => _onDamageUp.OnNext(Unit.Default);
    public void RaiseAttackSpeedUpClick() => _onAttackSpeedUp.OnNext(Unit.Default);
}
