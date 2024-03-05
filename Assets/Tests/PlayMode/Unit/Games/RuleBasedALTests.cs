using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using GameData;
using AL;

public class RuleBasedALTests {
    // MPC minigame (as an example, the type doesn't matter for AL if correctly implemented)
    private MultipleChoice minigame_full_one;
    private MultipleChoice minigame_full_one_copy;
    private MultipleChoice minigame_full_two;

    // empty minigame for testing purposes
    private MultipleChoice minigame_empty;


    // this AL has two minigames with questions
    private RuleBasedAL AL_Full; 
    // this AL has one minigame with questions
    private RuleBasedAL AL_Full_one;
    private RuleBasedAL AL_Empty; 

    // used to create mockup MPC minigame
    private MultipleChoice createMockupMPC(string question, string correctAnswer, List<string> wrongAnswers) {
        MultipleChoice mg = new MultipleChoice(Subject.Mathematics, "tag");
        int difficulty = 1;

        // fill in some data for the object
        for (int i = 0; i < 10; i++) {
            // change difficulty along the way
            if (i == 3) {
                difficulty = 2;
            } else if (i == 7) {
                difficulty = 3;
            }
            
            mg.difficulties.Add(difficulty);
            mg.questions.Add(question);
            mg.correctAnswers.Add(correctAnswer);
            mg.wrongAnswers.Add(wrongAnswers);
        }

        return mg;
    }

    private void PlayPerfect(Minigame mg, RuleBasedAL AL) {
        int nextIndex;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // got a new question, answer this one

                // very fast, always correct answering
                AL.AnswerQuestion(nextIndex, true, 0.1f); 
            }
        }
    }

    private void PlayTerrible(Minigame mg, RuleBasedAL AL) {
        int nextIndex;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // got a new question, answer this one

                // very fast, always correct answering
                AL.AnswerQuestion(nextIndex, false, 0.1f); 
            }
        }
    }

    private void PlayMedium(Minigame mg, RuleBasedAL AL) {
        int nextIndex;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // got a new question, answer this one

                // very fast, always correct answering
                AL.AnswerQuestion(nextIndex, (i % 2 == 1) ? false : true, 2f); 
            }
        }
    }

    private void PlayCorrectSlow(Minigame mg, RuleBasedAL AL) {
        int nextIndex;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // got a new question, answer this one

                // very fast, always correct answering
                AL.AnswerQuestion(nextIndex, true, 20f); 
            }
        }
    }

    private void PlayDecent(Minigame mg, RuleBasedAL AL) {
        int nextIndex;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // got a new question, answer this one

                // very fast, always correct answering
                AL.AnswerQuestion(nextIndex, (i % 3 == 0) ? false : true, 5f); 
            }
        }
    }

    [SetUp]
    public void SetUp()
    {
        // data to put into the mockup minigame, doesn't matter,
        // except for difficulty
        string question = "Zero?";
        string correctAnswer = "0";
        List<string> wrongAnswers = new List<string>();
        wrongAnswers.Add("1");
        wrongAnswers.Add("2");
        wrongAnswers.Add("3");
        wrongAnswers.Add("4");

        minigame_full_one = createMockupMPC(question, correctAnswer, wrongAnswers);
        minigame_full_one_copy = createMockupMPC(question, correctAnswer, wrongAnswers);

        // data to put into the mockup minigame, doesn't matter,
        // except for difficulty
        question = "Five?";
        correctAnswer = "5";
        wrongAnswers.Clear();
        wrongAnswers.Add("1");
        wrongAnswers.Add("2");
        wrongAnswers.Add("3");
        wrongAnswers.Add("4");

        minigame_full_two = createMockupMPC(question, correctAnswer, wrongAnswers);

        // create ECSList to put into AL
        List<Minigame> ECSListFull = new List<Minigame>();
        ECSListFull.Add(minigame_full_one);
        ECSListFull.Add(minigame_full_two);

        // now create the Rule Based Adaptive Learning Object
        AL_Full = new RuleBasedAL(ECSListFull);

        List<Minigame> ECSListFull_one = new List<Minigame>();
        ECSListFull_one.Add(minigame_full_one_copy);
        AL_Full_one = new RuleBasedAL(ECSListFull_one);

        List<Minigame> ECSListEmpty = new List<Minigame>();
        ECSListEmpty.Add(new MultipleChoice(Subject.Mathematics, "tag"));

        AL_Empty = new RuleBasedAL(ECSListEmpty);
    }

    [Test]
    public void NewMinigame_First_GivesMinigame()
    {
        Minigame mg = AL_Full.GetNewMinigame();

        Assert.NotNull(mg);
    }

    [Test]
    public void NextQuestion_FirstQuestionFullMinigame_PositiveInteger()
    {
        // set the first minigame
        Minigame mg = AL_Full.GetNewMinigame();
        
        int nextIndex = AL_Full.GetNextQuestionIndex();

        Assert.True(nextIndex > 0);
    }

    [Test]
    public void NextQuestion_AllQuestionsFullMinigame_MinusOne()
    {
        Minigame mg = AL_Full.GetNewMinigame();
        int nextIndex = 1;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL_Full.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                AL_Full.AnswerQuestion(nextIndex, true, 4f);
            }
        }

        Assert.True(nextIndex == -1);
    }

    [Test]
    public void AnswerQuestion_Correct_SpeedHigher()
    {
        Minigame mg = AL_Full.GetNewMinigame();
        mg.speed = 0.1;
        int nextIndex = 1;

        double initialSpeed = 0.1;

        nextIndex = AL_Full.GetNextQuestionIndex();
        AL_Full.AnswerQuestion(nextIndex, true, 1f);

        Assert.True(mg.speed > initialSpeed);
    }

    [Test]
    public void AnswerQuestion_Wrong_SpeedLower()
    {
        Minigame mg = AL_Full.GetNewMinigame();
        mg.speed = 0.1;
        int nextIndex = 1;

        double initialSpeed = 0.1;

        nextIndex = AL_Full.GetNextQuestionIndex();
        AL_Full.AnswerQuestion(nextIndex, false, 1f);

        Assert.True(mg.speed < initialSpeed);
    }

    [Test]
    public void Score_MaxPerformance_One() 
    {
        Minigame mg = AL_Full.GetNewMinigame();
        int nextIndex = 1;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL_Full.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // very fast, always correct answering
                AL_Full.AnswerQuestion(nextIndex, true, 0.1f); 
            }
        }

        float score = AL_Full.Score();

        Assert.True(score == 1.0f);
    }

    [Test]
    public void Score_MinPerformance_Zero() 
    {
        Minigame mg = AL_Full.GetNewMinigame();
        int nextIndex = 1;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL_Full.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // very fast, always correct answering
                AL_Full.AnswerQuestion(nextIndex, false, 0.1f); 
            }
        }

        float score = AL_Full.Score();

        Assert.True(score == 0.0f);
    }

    [Test]
    public void Score_MediumPerformance_BetweenZeroAndOne() 
    {
        Minigame mg = AL_Full.GetNewMinigame();
        int nextIndex = 1;

        for (int i = 0; i < 6; i++)
        {
            nextIndex = AL_Full.GetNextQuestionIndex();
            if (nextIndex != -1) 
            {
                // very fast, always correct answering
                AL_Full.AnswerQuestion(nextIndex, (i % 2 == 0) ? false : true, 5f); 
            }
        }

        float score = AL_Full.Score();

        Assert.True(0.0f < score && score < 1.0f);
    }

    [Test]
    public void IntraGame_PerfectScoring_ALLIncrease()
    {
        // one minigame: only scores matter! 
        // (deterministic minigame selection if there is only one)
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 8; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayPerfect(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_perfect.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void IntraGame_MediumScoring_ALLIncrease()
    {
        // one minigame: only scores matter! 
        // (deterministic minigame selection if there is only one)
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 8; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayMedium(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_medium.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void IntraGame_PerfectThenBad_ALLChange()
    {
        // one minigame: only scores matter! 
        // (deterministic minigame selection if there is only one)
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayPerfect(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }
        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayTerrible(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_perfectTerrible.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void IntraGame_PerfectThenMedium_ALLChange()
    {
        // one minigame: only scores matter! 
        // (deterministic minigame selection if there is only one)
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayPerfect(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }
        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayMedium(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_perfectMedium.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void IntraGame_ExampleScenario_ALLChange()
    {
        // one minigame: only scores matter! 
        // (deterministic minigame selection if there is only one)
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayCorrectSlow(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }
        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayDecent(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }
        // get new minigame
        for (int i = 0; i < 4; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayPerfect(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_slowDecentPerfect.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void IntraGame_CorrectSlow_ALLIncrease()
    {
        // one minigame: only scores matter! 
        // (deterministic minigame selection if there is only one)
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 8; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayCorrectSlow(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_correctSlow.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void IntraGame_PerfectThenMedium_ALLIncrThenDecr()
    {
        List<float> alls = new List<float>();
        List<float> scores = new List<float>();

        Minigame mg;

        // get new minigame
        for (int i = 0; i < 8; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayPerfect(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }
        // get new minigame
        for (int i = 0; i < 8; i++) 
        {
            mg = AL_Full_one.GetNewMinigame();
            alls.Add(AL_Full_one.getCurrentMinigameScore());
            // play minigame perfectly
            PlayMedium(mg, AL_Full_one);
            // end this minigame
            scores.Add(AL_Full_one.Score());
        }

        StreamWriter writer = new StreamWriter(@"Assets/GraphData/AL_intra_perfectMedium.csv");

        writer.WriteLine("ALL, Score");

        for (int i = 0; i < scores.Count; i ++) {
            writer.WriteLine(alls[i].ToString() + "," + scores[i].ToString());
        }

        writer.Close();
    }

    [Test]
    public void Json_Shit_Test()
    {
        string test = "{\"score\": 0.54555, \"weight\": 0.6333}";

        BackendData bd = BackendData.FromJson(test);

        Assert.True(bd.score > 0.5);
        Assert.True(bd.weight > 0.5);
    }
}
