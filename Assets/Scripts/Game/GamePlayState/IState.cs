public interface IState
{
    StateMachine Fsm { get; set; }
    void Enter(Pc.Owner owner);
    void Exit(Pc.Owner owner);
}