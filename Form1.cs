using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using System.Globalization;
using Jarv.Properties;

namespace Jarv
{
    public partial class Form1 : Form
    {
        string QEvent;
        string ProcWindow;
        double timer = 10;
        int count = 1;
        Random rnd = new Random();
        SpeechSynthesizer s = new SpeechSynthesizer();
        SpeechRecognitionEngine reg = new SpeechRecognitionEngine();
        StreamWriter sw;

        String[] ArraySocialCommands;
        String[] ArraySocialResponse;
        public static string socpath ; //These strings will be used to refer to the Social Command text document
        public static string sorpath ; //These strings will be used to refer to the Social Response text document


        bool tmp;


        public Form1()
        {

            InitializeComponent();

            Settings.Default.SocC = @"sc.txt";
            Settings.Default.SocR = @"sr.txt";
            socpath = Settings.Default.SocC;
            sorpath = Settings.Default.SocR;

            ArraySocialCommands = File.ReadAllLines(socpath);
            ArraySocialResponse = File.ReadAllLines(sorpath);
            string[] commands = { "hi jarvis", "hello","out of the way"
            ,"whats the date","whats todays date","what is the date",
            "how are you", "what's the time", "hi","what are you doing", "what time is it" , "goodbye" , "quit" ,"close jarvis", "what is the time","come back","jarvis"
            ,"what day is it","switch window","Stop it","Stop saying","Enough Jarvis"
            

            };

            reg.SetInputToDefaultAudioDevice();
            reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("link.txt")))));
            reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(commands))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("social.txt")))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("sc.txt")))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("sr.txt")))));
             reg.RecognizeAsync(RecognizeMode.Multiple);
            reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(stcd);
            reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(rec);
             reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(stop);
           
        

             
        }


        void stop(object sender, SpeechRecognizedEventArgs r) { 
        string k=r.Result.Text;
        switch (k) { 
            case "Stop it":
            case "Stop saying":
            case "Enough Jarvis":
                var cur = s.GetCurrentlySpokenPrompt();
                if(cur !=null)
                s.SpeakAsyncCancel(cur);
                
               
            break;
        }
      
        }

        void stcd(object sender, SpeechRecognizedEventArgs e)
        {
            
            
            string speech = e.Result.Text;
            int i = 0;
            
            try
            {
                foreach (string line in ArraySocialCommands)
                {
                    if (line == speech)
                    {
                        s.SpeakAsync(ArraySocialResponse[i]);
                    }
                    i += 1;
                }
            }
            catch
            {
                i += 1;
                s.SpeakAsync("Please check the " + speech + " social command on line " + i + ". It appears to be missing a proper response");
            }
             
             
        }



        private void Form1_Load(object sender, EventArgs e)
        {

            greet();
             
           




            

             


        }

        public void greet()
        {
            DateTime n = DateTime.Now;

            if (n.Hour >= 5 && n.Hour < 12)
            { s.Speak("Good morning "); }
            if (n.Hour >= 12 && n.Hour < 18)
            { s.Speak("Good afternoon "); }
            if (n.Hour >= 18 && n.Hour < 24)
            { s.Speak("Good evening "); }
            if (n.Hour < 5)
            { s.Speak("Hello, it is getting late "); }

        }


        public void rec(object sender, SpeechRecognizedEventArgs x)
        {
             

            string[] scl = File.ReadAllLines("social.txt");
            string recString = x.Result.Text;
            
                             

            int ranNum = rnd.Next(1, 10);
            
            switch (recString)
            {

                //Greet
                case "hello":
                case "hi":
                case "hi jarvis":
                    if (ranNum < 6) { s.Speak("Hello"); }
                    else if (ranNum > 5) { s.Speak("Hi"); }
                    break;

                case "goodbye":
                case "quit":
                case "close jarvis":
                    s.Speak("Until next Time");
                    Close();
                    break;


                case "jarvis":
                    if (ranNum < 5) { QEvent = ""; s.Speak("How may i be of Assistance ?"); }
                    else if (ranNum > 4) { QEvent = ""; s.Speak("Yes?"); }
                    break;

                case "thank you":
                    if (ranNum < 4) { s.Speak("Welcome "); }
                    else if (ranNum < 8 && ranNum > 4) { s.Speak("Anything for you Sir"); }
                    else { s.Speak("Your Welcome"); }
                    break;

                //Social
                case "how are you":
                    s.Speak("I'm good, how are you?");
                    break;

                case "what are you doing":
                    {
                        s.Speak("Let me Think. I m talking to you.");
                        break;
                    }


                case "can you listen":
                    s.Speak("yes");
                    break;

                case "tell about yourself":
                case "who are you":
                case "what are you ":
                    s.Speak("I m Jarvis Your Personal A I a k a Artificial Intelligence.");
                    break;


                //net
                case "open google":
                    s.Speak("okay");
                    System.Diagnostics.Process.Start("www.google.com");

                    break;
                case "open facebook":
                    s.Speak("alright");
                    System.Diagnostics.Process.Start("www.facebook.com");

                    break;
                case "open yahoo":
                    s.Speak("okay");
                    System.Diagnostics.Process.Start("www.yahoo.com");
                    break;




                //funtions
                case "what's the time":
                case "what is the time":
                case "what time is it":
                    s.Speak(DateTime.Now.ToString("h:mm:ss tt"));
                    break;



                case "what day is it":
                    {
                        s.Speak(DateTime.Today.ToString("dddd"));
                        break;

                    }


                case "whats the date":
                case "whats todays date":
                case "what is the date":
                    s.Speak(DateTime.Today.ToString("dd-MM-yyyy"));
                    break;



                case "out of the way":
                    if (WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized)
                    {
                        WindowState = FormWindowState.Minimized;
                        s.Speak("My apologies");
                    }
                    break;

                case "come back":
                    if (WindowState == FormWindowState.Minimized)
                    {
                        s.Speak("Alright?");
                        WindowState = FormWindowState.Normal;
                    }
                    break;


                case "switch window":
                    SendKeys.Send("%{TAB " + count + "}");
                    count += 1;
                    break;



          


            }
        }





        public string userName { get; set; }
    }
       
 
        




    }

