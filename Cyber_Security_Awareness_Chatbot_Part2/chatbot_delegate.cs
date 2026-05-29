namespace Cyber_Security_Awareness_Chatbot_Part2
{//start of namespace
    public delegate void chatbot_message(string sender, string message);
    public class chatbot_delegate
    {//start of class
     //the delegate variable
        public chatbot_message chatbot_reply;

        // Method to trigger/display messages
        public void send_message(string sender, string message)
        {
            // Check if delegate has a method
            if (chatbot_reply != null)
            {
                chatbot_reply(sender, message);
            }
        }
    }//end of class
}//end of namespace