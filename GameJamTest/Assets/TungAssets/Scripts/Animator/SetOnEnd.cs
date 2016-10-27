using UnityEngine;
using System.Collections;

public class SetOnEnd : StateMachineBehaviour
{
    [Header("The Type Of Edit (Only have one toggled)")]
    public bool IsBool = false;
    public bool IsInt = false;
    public bool IsFloat = false;

    [Header("The Value To Edit To (Only use the correct type)")]
    public bool BoolValue = false;
    public int IntValue = 0;
    public float FloatValue = 0;

    [Header("Name of the paremeter that is going to be edited")]
    public string ParameterName;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsBool) animator.SetBool(ParameterName, BoolValue);
        if (IsInt) animator.SetInteger(ParameterName, IntValue);
        if (IsFloat) animator.SetFloat(ParameterName, FloatValue);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
