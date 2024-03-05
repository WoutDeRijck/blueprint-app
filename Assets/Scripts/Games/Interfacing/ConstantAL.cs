using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData;

namespace AL
{
/// <summary>
///     Implements the AdaptiveLearning abstract class with constant, uniform probability output.
/// </summary>
public class ConstantAL : AdaptiveLearning
{
    /// <summary>
    ///     Keeps track of the indexes of questions that don't need to be asked anymore.
    /// </summary>
    private List<int> dontAsk = new List<int>();

    /// <summary>
    ///     List of correctness of answered questions, used for score calculation.
    /// </summary>
    private List<bool> correctness = new List<bool>();

    private int consecutiveWrong = 0;

    /// <summary>
    ///     These keep track of the indexes where questions of one of the three difficulty level are stored.
    /// </summary>
    List<int> low_index = new List<int>();
    List<int> med_index = new List<int>();
    List<int> high_index = new List<int>();
    
    public ConstantAL(List<Minigame> ECSList) {
        this.ECSList = ECSList;
    }
    
    /// <summary>
    ///     Make a new minigame based on the ECSList.
    /// </summary>
    /// <returns>New minigame.</returns>
    public override Minigame GetNewMinigame()
    {
        // get a random minigame from the ECS List
        Random rnd = new Random();
        minigameIndex = rnd.Next(1, ECSList.Count);

        minigame = ECSList[minigameIndex].GetMinigameFromData(); 

        int amount = minigame.amountOfQuestions;

        SetMask(amount/3, amount/3, amount/3);

        return minigame;
    }

    /// <summary>
    ///     Gets the index of the next question, to be answered in the minigame.
    /// </summary>
    /// <returns>
    ///     - An integer corresponding with a question
    ///     - -1 when there are no questions to be asked left.
    /// </returns>
    public override int GetNextQuestionIndex()
    {   
        int random;
        // check if there are still questions to be asked or not.
        if (dontAsk.Count < mask.Count)
        {
            do
            {
                random = UnityEngine.Random.Range(0, mask.Count);
            }
            while (dontAsk.Contains(mask[random])); // keep trying until not asked
        }
        else
        {
            return -1;
        }
        return mask[random];
    }

    /// <summary>
    ///     Call this function to inform the Minigame object about the results
    ///     of any answered questions, for Adaptive Learning updates.
    /// </summary>  
    /// <param name="correct">Whether or not the question was answered correctly</param>
    /// <param name="time">The time it took to answer the question.</param>
    public override void AnswerQuestion(int index, bool correct, float time)
    {
        if (correct) {
            dontAsk.Add(index); // add correct questions to dontAsk to not ask again.
            consecutiveWrong = 0;
        } else {
            consecutiveWrong++;
        }

        if (consecutiveWrong >= 4) {
            for (int i = 0; i < mask.Count; i++) {
                dontAsk.Add(i); // when dontAsk.Count == mask.Count, the minigame is finished
                // -> early quit
            }
        }      

        correctness.Add(correct);
    }

    /// <summary>
    ///     Calculate the score on this minigame based on the gathered results.
    /// </summary>
    /// <returns>
    ///     A score value for this minigame in range [0, 1].
    /// </returns>
    public override float Score()
    {
        float score = 0.0f ;
        
        for (int i = 0; i < correctness.Count; i++) {
            // range: [0] if false, [0.5; 1.5] if correct
            score += correctness[i] ? 1.0f : 0.0f;
        }

        // normalization
        if (correctness.Count != 0) {
            score = score / (correctness.Count);
        } else {
            score = 0;
        }

        // clear for next minigame
        dontAsk.Clear();

        return score;
    }

    /// <summary>
    ///     Set the mask according to the asked number of low, medium, high questions
    /// </summary>  
    private void SetMask(int n_low, int n_med, int n_high) {
        low_index = minigame.GetDifficultyIndeces(1);
        med_index = minigame.GetDifficultyIndeces(2);
        high_index = minigame.GetDifficultyIndeces(3);

        mask.Clear(); 

        int availableAmount = low_index.Count + med_index.Count + high_index.Count;

        while (n_low + n_med + n_high > availableAmount) {
            int[] n = LowerAmount(n_low, n_med, n_high, availableAmount);
            n_low = n[0];
            n_med = n[1];
            n_high = n[2];
        }

        while (!AmountsSatisfied(n_low, n_med, n_high)) {
            int[] n = MoveAmount(n_low, n_med, n_high);
            n_low = n[0];
            n_med = n[1];
            n_high = n[2];
        }
        
        mask.AddRange(SampleAmount(low_index, n_low));
        mask.AddRange(SampleAmount(med_index, n_med));
        mask.AddRange(SampleAmount(high_index, n_high));
    }

    private int[] LowerAmount(int n_low, int n_med, int n_high, int availableAmount) {
        int total = n_low + n_high + n_med;
        double scale = (double)availableAmount / (double)total;
        n_low = (int)Math.Round(n_low * scale);
        n_med = (int)Math.Round(n_med * scale);
        n_high = (int)Math.Round(n_high * scale);

        return new int[] {n_low, n_med, n_high};
    }

    private bool AmountsSatisfied(int n_low, int n_med, int n_high) {

        if (low_index.Count < n_low) {
            return false;
        }
        if (med_index.Count < n_med) {
            return false;
        }
        if (high_index.Count < n_high) {
            return false;
        }

        return true;
    }

    private int[] MoveAmount(int n_low, int n_med, int n_high) {

        if (low_index.Count < n_low) {
            // not enough low questions available, move to other difficulty
            (n_low)--;
            if (med_index.Count > n_med) {
                // move one question to medium
                (n_med)++;
            } else {
                // move one question to high
                (n_high)++;
            }
        }
        if (med_index.Count < n_med) {
            (n_med)--;
            if (low_index.Count > n_low) {
                // move one question to low
                (n_low)++;
            } else {
                // move one question to high
                (n_high)++;
            }
        }
        if (high_index.Count < n_high) {
            (n_high)--;
            if (med_index.Count > n_med) {
                // move one question to medium
                (n_med)++;
            } else {
                // move one question to low
                (n_low)++;
            }
        }
        return new int[] { n_low, n_med, n_high };
    }

    /// <param name="amount">Cannot be higher than the length of the inputted list!</param>
    private List<int> SampleAmount(List<int> indeces, int amount) {
        int randomStart = UnityEngine.Random.Range(0, indeces.Count);

        List<int> lst = new List<int>();

        for (int number = 0 ; number < amount; number ++) {
            lst.Add(indeces[(randomStart + number) % indeces.Count]);
        }

        return lst;
    }
}
}
