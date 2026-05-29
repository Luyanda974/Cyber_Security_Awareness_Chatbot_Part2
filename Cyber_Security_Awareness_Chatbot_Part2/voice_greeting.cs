using System;
using System.IO;
using System.Media;

namespace Cyber_Security_Awareness_Chatbot_Part2
{//start of namespace
    public class voice_greeting
    {
        //void method to play the sound named greet
        public void greet()
        { //star of greet method

            //replace the \bin\Debug\ from the path with greeting.wav

            string auto_path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", @"\voice_greeting2.wav");

            //create an instance for the soundPlayer class
            SoundPlayer greetMe = new SoundPlayer(auto_path);
            //then greet
            greetMe.Play();


        }//end of greet method
    }
}//end of namespace