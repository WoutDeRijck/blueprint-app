using System;
using System.Collections;
using System.Collections.Generic;
using GameData;

namespace AL
{
/// <summary>
///     The abstract class which defines an interface for every Adaptive Learning algorithm.
///     Creating a new algorithm means implementing this abstract interface.
/// </summary>
public abstract class AdaptiveLearning
{
    /// <summary>
    ///     ECSList stores the ECSs that are used within this game.
    /// </summary>
    protected List<Minigame> ECSList = new List<Minigame>();

    /// <summary>
    ///     Keeps track of the index within the ECSList: ECS where the current this.minigame is taken from.
    /// </summary>
    protected int minigameIndex;

    /// <summary>
    ///     minigame can be used to get information about the current minigame internally.
    /// </summary>
    protected Minigame minigame;

    /// <summary>
    ///     mask is used to mantain indexes of questions within the ECS that are selected for this minigame.
    /// </summary>
    protected List<int> mask = new List<int>();

    /// <summary>
    ///     Used to inform the game of the new obtained score.
    /// </summary>
    public Action<int, float> OnScoreUpdated;

    /// <summary>
    ///     getNewMinigame returns a subset of one of the ECSs. The selection is to be
    ///     decided by the AL algorithm.
    /// </summary>
    public abstract Minigame GetNewMinigame();

    /// <summary>
    ///     GetNextQuestionIndex decides which question will be asked next.
    /// </summary>
    /// <returns>
    ///     - a positive (including 0) integer, representing a question index like
    ///     in the Minigame difficulties field.
    ///     - -1 if there are no more questions to be asked
    /// </returns>
    public abstract int GetNextQuestionIndex();

    /// <summary>
    ///     AnswerQuestion should be called after each answered question, along with whether or not
    ///     it was answered correctly and how long it took (in seconds).
    ///     This function can be used to change the speed parameter of the minigame on a per-question basis.
    /// </summary> 
    public abstract void AnswerQuestion(int index, bool correct, float time);

    /// <summary>
    ///     Score should be called after GetNextQuestionIndex() returned -1, i.e., the minigame is over.
    ///     This function calculates a score of the students' performance, between 0 and 1.
    ///     It can also be used to update some weights etc. for next minigame selection round.
    /// </summary>
    public abstract float Score();
}
}
