public interface IState
{
    StateMachine Fsm { get; set; }
    void Enter(Piece.Owner owner);
    void Exit(Piece.Owner owner);
}