 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Cyber_Security_Awareness_Chatbot_Part2
{//start of namespace
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {//start of class
        //creating an instance for the class Array
        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();
        user_name check_name = new user_name();
        chatbot_delegate chats_delegate = new chatbot_delegate();
        sentiment_detection detect = new sentiment_detection();

        // variables
        string username = string.Empty;
        string pre_question = string.Empty;
        int counting = 0;
        public MainWindow()
        {//start of main window
            InitializeComponent();
            chats_delegate.chatbot_reply = error_method;


            voice_greeting greet = new voice_greeting();
            greet.greet();

            new respond(reply, ignore) { };
        }
        private void proceed(object sender, RoutedEventArgs e)
        {
            //Hide home page grid and set Username grid visible
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }
        //submit name  event handler
        private void submit_name(object sender, RoutedEventArgs e)
        {
            //get username input
            string entered_name = usernames_input.Text.Trim();


            //check if username is empty
            if (string.IsNullOrWhiteSpace(entered_name))
            {
                MessageBox.Show("Please enter a username before continuing.", "Username Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            //save username
            username = check_name.submit_name(usernames_input, chats);


            //Hide username page grid and set chats grid visible
            username_grid.Visibility = Visibility.Hidden;

            chat_grid.Visibility = Visibility.Visible;


        }


        //send event handler
        private void send(object sender, RoutedEventArgs e)
        {
            // Get the question from the design and sanitize it
            string rawQuestion = question.Text.ToString().Trim();

            if (string.IsNullOrWhiteSpace(rawQuestion))
            {
                chats_delegate.send_message("Lumina", "Please enter a question.");
                return;
            }

            // Remove special characters and clean the question
            string questions = RemoveSpecialCharacters(rawQuestion);

            // Show what the user typed 
            chats_delegate.send_message(username, rawQuestion);
            //detect user emotion
            string emotion_reply =
                detect.detect_sentiment(rawQuestion);


            //show emotion response
            if (!string.IsNullOrWhiteSpace(emotion_reply))
            {
                chats_delegate.send_message("Lumina", emotion_reply);
            }


            //ai chats and auto_show_interest
            auto_show_interest();
            ai_check(questions);
        }

        //end for the username submit

        //start of ai_chat method
        private void ai_check(string questions)
        {
            if((questions.ToLower().Contains("more") || questions.ToLower().Contains("another") || questions.ToLower().Contains("explain")) && !string.IsNullOrWhiteSpace(pre_question))
            {
                questions += " " + pre_question;
            }


            // Check if user entered anything meaningful
            if (string.IsNullOrWhiteSpace(questions))
            {
                chats_delegate.send_message("Lumina", "Please enter a valid question.");
                question.Clear();
                return;
            }



            // Check if the question contains only special characters or empty after cleaning
            if (questions.Length == 0 || string.IsNullOrWhiteSpace(questions))
            {
                chats_delegate.send_message("Lumina", "I couldn't understand that.");
                question.Clear();
                return;
            }

            // Variables for processing
            string[] words = questions.ToLower().Split(new char[] { ' ', ',', '.', '?', '!', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            bool found = false;
            string message = string.Empty;
            Random indexer = new Random();
            List<string> per_word = new List<string>();
            List<string> answers_found = new List<string>();






            // Process each word
            foreach (string word in words)
            {
                // Skip very short words or ignored words
                if (word.Length < 3 || ignore.Contains(word.ToLower()))
                    continue;

                per_word.Clear();





                //start of interests

                if (word.Contains("interested"))
                {
                    string store_interests = string.Empty;
                    bool found_interest = false;

                    HashSet<string> currentInterests = new HashSet<string>();

                    foreach (string interest in words)
                    {
                        // CLEAN INPUT
                        string clean = interest.ToLower().Trim();
                        clean = Regex.Replace(clean, @"[^a-zA-Z0-9\s]", "");

                        // FILTER NOISE WORDS
                        if (!ignore.Contains(clean) && clean != "interested" && clean != "and" && clean != "in" && clean.Length >= 3)
                        {
                            found_interest = true;
                            currentInterests.Add(clean);
                        }
                    }


                    // prepare interests
                    store_interests = string.Join(", ", currentInterests);

                    if (found_interest && !string.IsNullOrWhiteSpace(store_interests))
                    {
                        string filename = "interested_topic.txt";
                        bool userFound = false;

                        if (File.Exists(filename))
                        {
                            string[] lines = File.ReadAllLines(filename);

                            for (int i = 0; i < lines.Length; i++)
                            {
                                if (lines[i].StartsWith(username))
                                {
                                    userFound = true;

                                    //get all the interests
                                    string existing = lines[i].Replace(username + " interested in:", "").ToLower();

                                    HashSet<string> existingSet = new HashSet<string>(existing.Split(',').Select(x => x.Trim()).Where(x => x != ""));

                                    // remove dumplicates
                                    foreach (string item in currentInterests)
                                    {
                                        existingSet.Add(item);
                                    }

                                    string finalList = string.Join(", ", existingSet);

                                    lines[i] = username + " interested in: " + finalList;
                                    File.WriteAllLines(filename, lines);

                                    message += "great, i added " + store_interests + " to your interests and ";
                                    break;
                                }
                            }
                        }

                        if (!userFound)
                        {
                            File.AppendAllText(
                                filename,
                                username + " interested in: " + store_interests + "\n"
                            );

                            message += "great, i will remember that you are interested in " + store_interests + " and ";
                        }
                    }
                    else
                    {
                        message += "Please specify what you're interested in (e.g., 'I am interested in cybersecurity')";
                    }
                }



                //end of interests




                // Search for matching answers
                bool wordFound = false;
                foreach (string answer in reply)
                {
                    if (answer.ToLower().Contains(word))
                    {
                        wordFound = true;
                        per_word.Add(answer);
                    }
                }

                if (wordFound && per_word.Count > 0)
                {
                    found = true;
                    pre_question = word;

                    int indexing = indexer.Next(0, per_word.Count);
                    answers_found.Add(per_word[indexing]);
                    break;
                }
            }

            // Show responses or error message
            if (answers_found.Count > 0)
            {
                // Remove duplicate answers
                answers_found = answers_found.Distinct().ToList();

                foreach (string per_answer in answers_found)
                {
                    message += per_answer + "\n";
                }

                chats_delegate.send_message("Lumina", message.TrimEnd('\n'));


                chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
            }
            else
            {
                // when nothing is found
                string[] fallbackMessages = {
            "I'm so sorry but i could not understand that. Could you please rephrase your question?",
            "I didn't really get that. Try asking about cybersecurity topics.",
            "I'm not really sure how to respond to that. Can you please ask something else?",
            "I couldn't find an answer for that. Please ask about cybersecurity terms.",
            "My apologies but i  don't have information on that topic yet."
        };

                Random random = new Random();
                string fallbackMessage = fallbackMessages[random.Next(fallbackMessages.Length)];
                chats_delegate.send_message("Lumina", fallbackMessage);
            }

            // Clear the input box
            question.Clear();


        }

        //end of ai_chat method


        //method to remove special characters
        private string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            StringBuilder sanitized = new StringBuilder();

            foreach (char c in input)
            {
                // Keep letters, numbers, spaces, and basic punctuation
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '\'' || c == '-')
                {
                    sanitized.Append(c);
                }
                else
                {
                    // Replace other special characters with space
                    sanitized.Append(' ');
                }
            }

            // Clean up extra spaces and trim
            string result = sanitized.ToString();
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ").Trim();

            return result;
        }


        //end of method to remove special characters

        //method count to show interests randomly
        private void auto_show_interest()
        {
            //check if three times
            if (counting == 3)
            {
                //read the user's interests from file
                string filename = "interested_topic.txt";

                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);

                    //find the user's line
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(username))
                        {
                            //get the interests part
                            int colonIndex = line.IndexOf("interested in:");
                            if (colonIndex >= 0)
                            {
                                string interests = line.Substring(colonIndex + 14).Trim();

                                //show reminder of interests
                                chats_delegate.send_message("Lumina", "Just a reminder, you are interested in " + interests + " and ");
                                ai_check(interests);
                                break;
                            }
                        }
                    }
                }

                //reset counting
                counting = 0;
            }
            else
            {
                //incrementing
                counting += 1;
            }
        }
        //end of count interest method

        // Updated error method with better formatting
        private void error_method(string name, string message)
        {
            // Create a border for chats
            Border messageBorder = new Border
            {
                Margin = new Thickness(0, 2, 0, 2),
                Padding = new Thickness(5, 3, 5, 3),
                CornerRadius = new CornerRadius(5)
            };

            // Set different background for user vs bot
            if (name.ToLower().Contains("chatbot") || name.ToLower().Contains("chat"))
            {// Light blue
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(173, 216, 230));
            }
            else
            {    // Light gray
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(211, 211, 211));
            }
            messageBorder.BorderThickness = new Thickness(1);

            TextBlock messageText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(2)
            };

            // Set color based on sender
            Brush nameColor = (name.ToLower().Contains("chatbot") || name.ToLower().Contains("chat")) ?
                              Brushes.DarkBlue : Brushes.DarkGreen;

            Brush messageColor = Brushes.Black;

            messageText.Inlines.Add(new Run
            {
                Text = name + ": ",
                Foreground = nameColor,
                FontWeight = FontWeights.Bold
            });

            messageText.Inlines.Add(new Run
            {
                Text = message,
                Foreground = messageColor
            });

            messageBorder.Child = messageText;
            chats.Items.Add(messageBorder);

            chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }//end of error method

        private void Enter_Send(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                send(sender, e);
            }
        }
    }
    }

