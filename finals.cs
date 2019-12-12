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
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using WMPLib;

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
         

        String[] ArraySocialCommands;
        String[] ArraySocialResponse;
       
        
            public static string socpath ; //These strings will be used to refer to the Social Command text document
        public static string sorpath ; //These strings will be used to refer to the Social Response text document
        public static string shlpath;
        public static string shlres;
        double f = 0, sn=0; char p;
        bool tmp=false;

        bool g = true;
        public Form1()
        {
             InitializeComponent();

             if (CheckForInternetConnection() == true)
                 NetworkAvailabilityToolStripLabel.Text = "System Online";
             else
                 this.NetworkAvailabilityToolStripLabel.ForeColor=System.Drawing.Color.Red;
                 NetworkAvailabilityToolStripLabel.Text = "System Offline";

                

            Settings.Default.SocC = @"sc.txt";
            Settings.Default.SocR = @"sr.txt";
            Settings.Default.SosL = @"shell.txt";
       
            socpath = Settings.Default.SocC;
            sorpath = Settings.Default.SocR;
            shlpath = Settings.Default.SosL;

             



            ArraySocialCommands = File.ReadAllLines(socpath);
            ArraySocialResponse = File.ReadAllLines(sorpath);
            
          
            string[] commands = { "hi jarvis", "hello","out of the way"
            ,"whats the date","whats todays date","what is the date",
            "how are you", "what's the time", "hi","what are you doing", "what time is it","goodbye", "what is the time","come back","jarvis"
            ,"what day is it","switch window","Stop it","Stop saying","Enough Jarvis","Read this"
            



            };

            reg.SetInputToDefaultAudioDevice();
            reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("link.txt")))));
            reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(commands))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("social.txt")))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("sc.txt")))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("sr.txt")))));
             reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("shell.txt")))));
              reg.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("dc1.txt")))));
              
             
             reg.RecognizeAsync(RecognizeMode.Multiple);
            reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(stcd);
            reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(rec);
             reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(stop);
             reg.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(level);

             reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(test);
          reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(search);

            reg.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(readthis);
           
        }


        
        void search(object sender, SpeechRecognizedEventArgs f)
        {
            if (tmp != true)
            {
                string Speech = f.Result.Text;
                if (Speech == "yes")
                {
                    QEvent = Speech;
                    s.SpeakAsync("Um?");
                    Speech = string.Empty;
                }
                else if (Speech != string.Empty && QEvent == "yes")
                {
                    System.Diagnostics.Process.Start("http://google.com/search?q=" + Speech);
                    QEvent = string.Empty;

                    s.Speak("here you go.");
                    Speech = string.Empty;

                }
            }
        }

        void readthis(object sender, SpeechRecognizedEventArgs t) {
            
           if(tmp!=true)  {
            string k = t.Result.Text;
           
          if(k=="Read this"){
             
            SendKeys.Send("^c");
            string text = Convert.ToString(Clipboard.GetText());
            s.Speak(text);
          }
        }}

         


       void test(object sender, SpeechRecognizedEventArgs d)
        {

            string o = d.Result.Text;
            switch (o)
            {
                case "stop listening": s.Speak("As You Wish."); s.Pause(); tmp = true; break;

                case "come back jarvis": s.Resume(); s.Speak("I m Back Online"); tmp = false; break;
            
            }
        
        }

        void level(object sender, AudioLevelUpdatedEventArgs e)
        {
            progressBar1.Value = e.AudioLevel;
        }

         

        void stop(object sender, SpeechRecognizedEventArgs r) {

            if (tmp != true)
            {
                string k = r.Result.Text;
                switch (k)
                {
                    case "Stop it":
                    case "Stop saying":
                    case "Enough Jarvis":
                        var cur = s.GetCurrentlySpokenPrompt();
                        if (cur != null)
                            s.SpeakAsyncCancel(cur);


                        break;

                }
            }
      
        }

        void stcd(object sender, SpeechRecognizedEventArgs e)
        {
            if (tmp != true)
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
            
             if(tmp!=true){

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
                    s.Speak(DateTime.Now.ToString("h:mm tt"));
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
                


                case "open notepad":
                    System.Diagnostics.Process.Start("notepad.exe");
                    s.Speak("Here you go ");
                    break;

                case "open calculator":
                    System.Diagnostics.Process.Start("calc.exe");
                    break;

                case "go to the top of the page":
                    s.Speak("Alright");
                    SendKeys.SendWait("{HOME}");
                    break;

                case "go to the bottom of the page":
                    s.Speak("Alright");
                    SendKeys.SendWait("{END}");
                    break;

                case "scroll up":
                    SendKeys.SendWait("{PGUP}");
                    break;

                case "scroll down":
                    SendKeys.SendWait("{PGDN}");
                    break;

                case "copy":
                     
                        s.Speak("Copying");
                        System.Windows.Forms.SendKeys.Send("^c");
                         
                    break;

                case "paste":
                    
                        s.Speak("Pasting");
                        SendKeys.Send("^v");
                       
                     
                    break;

                case "open this":
                case "play this song":
                    if (ranNum < 3) { s.Speak("Here You Go"); }
                    else if (ranNum > 3 && ranNum < 6) { s.Speak("Alright"); }
                    else if (ranNum > 6 && ranNum < 8) { s.Speak("Okay"); }
                    else  s.Speak("Roger That");
                    SendKeys.Send("{ENTER}");
                    break;

                case "select all":
                    SendKeys.Send("^a");                                      
                    break;

                case "back":
                case "go back":
                    if (ranNum < 3) { s.Speak("Here You Go"); }
                   
                    else if (ranNum > 3 && ranNum < 8) { s.Speak("Okay"); }

                    SendKeys.Send("{BKSP}");
                    break;

                case "zoom in":
                    SendKeys.Send("^{ADD 2}");
                    break;


                case "zoom out":
                    SendKeys.Send("^{SUBTRACT 2}");
                    break;

                case "close this":
                    SendKeys.Send("%{F4}");
                    s.Speak("Okay");
                    break;

                case "maximize window":
                    
                    break;

            }

            }
        }

      



        public string userName { get; set; }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

         
        
         
       


      



      

      

        }

    }

