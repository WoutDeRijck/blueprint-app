using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using OpenAI;
using System;
using Newtonsoft.Json;
using GameData;

namespace Platform.Controllers
{

/// <summary>
/// This class implements the UI functionality and handles the messages sent/received to/from the OpenAI API
/// </summary>
public class ChatGPT : MonoBehaviour
{
    private Minigame minigame;

    private OpenAIApi openai = new OpenAIApi();

    private string prompt = "Act as a teacher assistant, the teacher will give some context where she wants to output some questions from. \n" +
        "Your job is to respond in ONLY CSV format, that way I can process the data and use the questions you generate. \n" +
        "Here is the context: \n";
    private string context;
    private ChatMessage contextMessage;
    private string instruction;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Generate random amount of questions of different subtypes
    /// </summary>
    public async Task<Minigame> GenerateQuestions(Minigame minigame, string prompt)
    {
        UnityEngine.Debug.Log("Sending to GPT...");
        SetContext(prompt);

        this.minigame = minigame;

        Subtype subtype = minigame.subtype;

        // Generate questions depending on selected subtype
        switch (subtype)
        {
            case Subtype.MultipleChoice:
                GenerateMultipleChoice(30);
                await SendReply<MultipleChoice>();
                break;
            case Subtype.Grouping:
                GenerateGrouping(5);
                await SendReply<Grouping>();
                break;
            case Subtype.Ranking:
                GenerateRanking(10);
                await SendReply<Ranking>();
                break;
            default:
                break;
        }
        return this.minigame;
    }

    /// <summary>
    /// Set the initial context and clear inputField
    /// </summary>
    private void SetContext(string input)
    {
        context = input;
        instruction = input;

        // For the first message, prompt is added to let the system know what its task is.
        // No query to GPT is necessary
        var newMessage = GetChatMessage(instruction, "user");
        contextMessage = newMessage;
        contextMessage.Content = prompt + "\n" + context;
    }

    /// <summary>
    /// Generate multiple choice questions
    /// </summary>
    private void GenerateMultipleChoice(int amount)
    {
        instruction = "Generate " +
            amount.ToString() +
            " MultipleChoice questions and format it in this EXACT CSV format: \n" +
            "Difficulty Level;Question;Correct;Wrong Answer;Wrong Answer;Wrong Answer\n1;6 + 7;13;6;6;10\n1;7 + 8;15;18;10;14\n1;6 + 8;14;8;6;10\n1;9 + 1;10;6;6;12 \n" +
            "DO NOT USE THESE QUESTIONS, they are just examples. \n" +
            "Provide three difficulty levels of questions: 1-should be easier, 2-shuld be able to solve, 3-challenge question";
        
    }

    /// <summary>
    /// Geberate grouping questions
    /// </summary>
    private void GenerateGrouping(int amount)
    {
        instruction = "Make 4 groups and provide " +
            amount.ToString() +
            " elements in each group and format it in this EXACT CSV format, seperated with ; like these examples\n" +
            "Difficulty;Even numbers;Multiple of 3;Primary numbers;Multiples of 5\n2;4;9;1;10\n2;6;21;2;15\n2;8;27;3;20\n2;10;33;5;25 \n" +
            "Difficulty;Les Animaux;Les passe temps;Famille;Vetements\n1;le rat;la danse;le fr�re;le sac\n1;le chien;la peinture;le p�re;le pull\n2;le singe;le dessin;le grand-p�re;le blouson\n3;le kangourou;la natation synchronis�e;la belle-s�ur;la casquette\n3;l'hippopotame;faire de l'escalade;l'oncle;la chemise \n" +
            "DO NOT USE THESE EXAMPLES, they are just examples. \n";
        
    }

    /// <summary>
    /// Geberate ranking questions
    /// </summary>
    private void GenerateRanking(int amount)
    {
        instruction = "Generate " +
            amount.ToString() +
            " Ranking questions and format it to this EXACT CSV format, seperated with ; like these examples \n" +
            "Difficulty Level;Question;Order element 1;Order element 2;Order element 3;Order element 4\n1;Dalende volgorde;km;m;cm;mm\n1;Dalende volgorde;hm;m;dm;nm\n1;Dalende volgorde;L;dL;cL;mL\n2;Dalende volgorde;km2;m2;dm2;cm2 \n" +
            "DO NOT USE THESE QUESTIONS, they are just examples. \n " +
            "There are maximum 4 elements per ranking questions, but do not place the elements in the questions";
        
    }

    /// <summary>
    /// Get a ChatMessage which can be used by the gpt-model
    /// </summary>
    private ChatMessage GetChatMessage(string message, string role = "user")
    {
        return new ChatMessage()
        {
            Role = role,
            Content = message
        };
    }

    /// <summary>
    /// Make the call to GPT-API and handle the response
    /// </summary>
    private async Task<T> SendReply<T>() where T : Minigame
    {
        // Chat history to send to GPT
        var newMessage = GetChatMessage(instruction, "user");
        List<ChatMessage> messages = new List<ChatMessage>() { contextMessage, newMessage };

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo-0301",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            string result = message.Content.Trim();

            message.Content = result;
            Debug.Log(result);

            try
            {
                minigame.data = result;
                minigame = (T) minigame.GetMinigameFromData();
                return (T) minigame;
            }
            catch (JsonReaderException ex)
            {
                Debug.Log("A minigame could not be reconstructed from the json: " + ex);
            }

            messages.Add(message);
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }
        return null;
    }
}

}
