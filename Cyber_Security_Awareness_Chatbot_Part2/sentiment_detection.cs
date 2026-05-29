using System;
using System.Collections.Generic;

namespace Cyber_Security_Awareness_Chatbot_Part2
{//start of namespace
    public class sentiment_detection
    {//start of class
     //dictionary for emotion responses
        Dictionary<string, List<string>> emotions = new Dictionary<string, List<string>>();

        //constructor
        public sentiment_detection()
        {
            //sad responses
            emotions.Add("sad", new List<string>()
            {
                   "I'm sorry you're feeling a little sad.",
                "I hope that things will get better soon.",
                "Take it one step at a time."
            });

            //happy responses
            emotions.Add("happy", new List<string>()
            {
                "That is so great to hear!",
                 "Awesome! I'm so happy for you.",
                  "Glad to hear that you're feeling good today."
            });

            //angry responses
            emotions.Add("angry", new List<string>()
            {
              "I truly understand your frustration.",
                "Let us solve the problem together.",
                "Breathe and let us work through it together."
            });

            //worried responses
            emotions.Add("worried", new List<string>()
            {
                "Don't worry, I'm here to help.",
                "Most cybersecurity problems can be solved so dont worry.",
                "Let's make sure your information stays safe together."
            });

            //confused responses
            emotions.Add("confused", new List<string>()
            {
                "Let me explain it differently so that you can understand it better.",
                "I'll break it down step by step so it is easier to understand.",
                "No worries, confusion is normal.I will help you nderstand better"
            });
        }



        //themethod to detect emotion
        public string detect_sentiment(string input)
        {
            input = input.ToLower();

            Random random = new Random();

            foreach (var emotion in emotions)
            {
                if (input.Contains(emotion.Key))
                {
                    int index =
                        random.Next(emotion.Value.Count);

                    return emotion.Value[index];
                }
            }

            return string.Empty;
        }

    }//end of class
}//end of namespace