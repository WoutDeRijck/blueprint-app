using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameData;
using Newtonsoft.Json;

using Platform.Interfacing.Systems;
using GameManagement;

namespace AL
{

/// <summary>
///     Used to serialize the returned served json
/// </summary>
[Serializable]
public class BackendResponse {
    public int id;
    public int minigameId;
    public int studentId;
    public string data;

    public static BackendResponse FromJson(string jsonString) {
        return JsonConvert.DeserializeObject<BackendResponse>(jsonString);
    } 
}

/// <summary>
///     Used to gather backend data about one particular ECS, for this student specifically.
/// </summary>
[Serializable]
public class BackendData {
    public float score;
    public float weight;

    public static BackendData FromJson(string jsonString) {
        return JsonConvert.DeserializeObject<BackendData>(jsonString);
    } 
}

/// <summary>
///     This class implements a rule-based algorithm for adaptive learning.
///     The student's performance is used to adapt the amount of questions of different levels of difficutly.
///     After each game, the current performance is uploaded to the backend, to be used again when the game is played next time.
/// </summary>
public class RuleBasedAL : AdaptiveLearning
{
    /// <summary>
    ///     Keeps track of the scores per ECS. Length will be the # of ECSs.
    /// </summary>
    private List<float> scores = new List<float>();

    /// <summary>
    ///     Keeps a list of weights for ECS selection. Length will be the # of ECSs.
    /// </summary>
    private List<float> weights = new List<float>();

    /// <summary>
    ///     Keeps track of the indexes of questions that don't need to be asked anymore.
    /// </summary>
    private List<int> dontAsk = new List<int>();

    /// <summary>
    ///     List of correctness of answered questions, used for score calculation.
    /// </summary>
    private List<bool> correctness = new List<bool>();

    /// <summary>
    ///     List of times (seconds) of answered questions, used for score calculation.
    /// </summary>
    private List<float> times = new List<float>();

    /// <summary>
    ///     These keep track of the indexes where questions of one of the three difficulty level are stored.
    /// </summary>
    List<int> low_index = new List<int>();
    List<int> med_index = new List<int>();
    List<int> high_index = new List<int>();

    /// <summary>
    ///     Alpha used in moving average of scores.
    ///     if alpha == 1: fully use new value 
    /// </summary>
    private float alpha = 0.5f;

    /// <summary>
    ///     If score is higher than this value, the weight of this ECS is decreased.
    /// </summary>
    private float scoreWeightThreshold = 0.40f;

    /// <summary>
    ///     Keeps track of the amount of times a wrong answer was given, to end game after some time.
    /// </summary>
    private int consecutiveWrong = 0;

    public RuleBasedAL(List<Minigame> ECSList) {
        this.ECSList = ECSList;

        Debug.Log("MIEL: Starting rulebased: " + ECSList.Count);

        // Initialize the scores based on the last games
        
        // here, call the function to get the backend scores for this ECS
        // -> userID and ECSID need to be available here
        try {
            int ID = AuthenticationSystem.Instance.student.id;
            Debug.Log("MIEL ID: " + ID);
            for ( int i = 0; i < ECSList.Count; i++ ) {
                // for every ECS, get the backend-stored weight and score
                DataSystem.Instance.GetALData(ID, ECSList[i].id, 
                (jsonString) => {
                    try {
                        Debug.Log("MIEL got this json: " + jsonString);
                        BackendResponse response = BackendResponse.FromJson(jsonString);
                        BackendData data = BackendData.FromJson(response.data);
                        Debug.Log("MIEL Got backend scores data" + data);
                        Debug.Log("MIEL Got backend weight "+ data.weight);
                        Debug.Log("MIEL Got backend score "+ data.score);
                        weights.Add(data.weight);
                        scores.Add(data.score);
                    } catch (Exception e) {
                        Debug.Log("MIEL Couldn't get scores data" + e);
                        // the data was of a different format (possibly another algorithm!)
                        weights.Add(1.0f);
                        scores.Add(0.0f);
                    }
                },
                (error) => {
                    Debug.Log("MIEL ERROR in getting ");
                    weights.Add(1.0f);
                    scores.Add(0.0f);
                });
            }
        } catch (Exception) {
            // there is no student object maybe
            for ( int i = 0; i < ECSList.Count; i++ ) {
                weights.Add(1.0f);
                scores.Add(0.0f);
            }
        }
    }

    public override Minigame GetNewMinigame()
    {
        // select one ECS based on weights
        SelectECS();

        // make subselection of questions based on score
        FilterECS();

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
        float boundTime;
        boundTime = time > minigame.fastTime ? time : minigame.fastTime;
        boundTime = boundTime < minigame.slowTime ? boundTime : minigame.slowTime;

        // increment is the amount to increment speed when correct
        // incr in range [0.05, 0.15]
        double incr = 0.10 + 0.05 * (10.0 - (double)boundTime) / 5.0;

        if (correct) {
            dontAsk.Add(index); // add correct questions to dontAsk to not ask again.

            minigame.speed = minigame.speed + incr <= 1.0 ? minigame.speed + incr : 1.0;
            consecutiveWrong = 0;
        } else {
            consecutiveWrong++;
            // always decrease with the same amount 
            // -> don't penalty quick mistakes more than slow ones
            minigame.speed = minigame.speed > 0.0 ? minigame.speed - 0.10 : 0.0; 

        }

        if (consecutiveWrong >= 4) {
            for (int i = 0; i < mask.Count; i++) {
                dontAsk.Add(i); // when dontAsk.Count == mask.Count, the minigame is finished
                // -> early quit
            }
        }      

        correctness.Add(correct);
        times.Add(time);
    }

    /// <summary>
    ///     Calculate the score on this minigame based on the gathered results.
    /// </summary>
    /// <returns>
    ///     A score value for this minigame in range [0, 1].
    /// </returns>
    public override float Score()
    {

        float score = 0.0f;
        float boundTime;

        for (int i = 0; i < correctness.Count; i++) {
            // reasoning: if correct, score is centered around 1 depending on the time it took
            boundTime = times[i] > minigame.fastTime ? times[i] : minigame.fastTime;
            boundTime = boundTime < minigame.slowTime ? boundTime : minigame.slowTime;

            // normalized [-1; 1]
            float normalizedTime = 2.0f * (boundTime - (minigame.slowTime + minigame.fastTime) / 2.0f) / (minigame.slowTime - minigame.fastTime);
            
            // range: [0] if false, [0.5; 1.5] if correct
            score += correctness[i] ? 1.0f - normalizedTime * 0.5f : 0.0f;
        }

        // normalization
        if (correctness.Count != 0) {
            score = score / (correctness.Count * (float) 1.5);
        } else {
            score = 0;
        }


        // clear for next minigame
        dontAsk.Clear();
        correctness.Clear();
        times.Clear();
        
        consecutiveWrong = 0;

        UpdateScoresAndWeights(score);

        storeData();

        return score;
    }

    /// <summary>
    ///     Send the current minigame's weight and score to the backend for next time.
    /// </summary>
    private void storeData() {
        // send weights and scores to backend
        try {
            int ID = AuthenticationSystem.Instance.student.id;

            BackendData data = new BackendData{};
            data.score = scores[minigameIndex] * 0.8f; // a bit easier next time
            data.weight = weights[minigameIndex];
            string jsonData = JsonUtility.ToJson(data, false);

            Debug.Log("MIEL jsonData to store: " + jsonData);
            
            DataSystem.Instance.SetALData(ID, ECSList[minigameIndex].id, jsonData);
        
        } catch (Exception) {
            Debug.Log("MIEL some error in setting");
        }
    }

    private float movingAvg(float oldScore, float newScore) {
        return (1 - alpha) * oldScore + alpha * newScore;
    }

    private void UpdateScoresAndWeights(float score) {
        scores[minigameIndex] = movingAvg(scores[minigameIndex], score);

        OnScoreUpdated?.Invoke(GameManager.Instance.hotspotOpened, score);

        if (score > scoreWeightThreshold) 
        {
            float newWeight = weights[minigameIndex] * 0.7f; // lower prob. that this ECS is chosen again
            weights[minigameIndex] = newWeight > 0.5f ? newWeight : 0.5f; // keep weights within bounds
        } 
        else if (weights[this.minigameIndex] < 5) 
        {
            float newWeight = weights[minigameIndex] * 1.42f; // increase prob. of getting ECS again.
            weights[minigameIndex] = newWeight <= 5 ? newWeight : 5; // keep weights within bounds
        }
    }

    /// <summary>
    ///     Select one Minigame (ecs) from the game object. This is done using a
    ///     weighted lottery.
    /// </summary>
    private void SelectECS() {
        int selectedIndex = 0;

        // determine sum for range of ticket.
        float sumOfWeights = weights.Sum();

        // pull a random ticket out of weighted jar of tickets.
        float lottery_ticket = UnityEngine.Random.Range(0, sumOfWeights);

        // find the owner of the ticket and return that minigame.
        float currentWeight = 0.0f;
        for (int i = 0; i < weights.Count; i ++) {
            currentWeight += weights[i];

            if (currentWeight >= lottery_ticket) {
                selectedIndex = i;
                break;
            }
        }

        this.minigameIndex = selectedIndex;
    }

    /// <summary>
    ///     Select questions from the full Minigame that suit this player's skill level according to AL.
    ///     The result is put into this.minigame
    /// </summary>
    private void FilterECS() {
        int n_low = 0;
        int n_medium = 0;
        int n_high = 0;
        float temp;

        float score = scores[minigameIndex];

        // here, a new minigame from the right type is created!
        
        minigame = ECSList[minigameIndex].GetMinigameFromData(); 

        int amount = minigame.amountOfQuestions;

        // interpolate between the closest difficulty levels (easy - medium or medium-hard)
        if (score > 0.50f) {
            temp   = (score - 0.50f) * 2f * amount;
            n_high = (int) Math.Round((double)temp);
            n_medium  = amount - n_high;
        } else {
            temp   = score * 2f * amount;
            n_medium = (int) Math.Round(temp);
            n_low  = amount - n_medium;
        }

        SetMask(n_low, n_medium, n_high);
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
    
    /// <summary>
    ///     Get the score value of the current minigame. This is used by testing code.
    /// </summary>
    public float getCurrentMinigameScore() {
        return scores[minigameIndex];
    }
}
}
