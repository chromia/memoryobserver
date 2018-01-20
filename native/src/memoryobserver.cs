using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace memoryobserver
{
    class CommandInfo
    {
        public string command;
        public List<string> commandargs;
        public CommandInfo( string command, List<string> commandargs )
        {
            this.command = command;
            this.commandargs = commandargs;
        }
    }

    class Program
    {
        static bool debugmode = false;
        static string targetprocess = "firefox";
        static readonly string ResultOk = "ok";

        public static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (args[0] == "--debug")
                {
                    debugmode = true;
                }
            }

            bool exit_flag = false;
            while (!exit_flag)
            {
                //Receive a message from browser
                string message;
                if (debugmode)
                {
                    message = ReadDebug();
                }
                else
                {
                    message = Read();
                }

                //Interpreting of message
                if (message.Length > 0)
                {
                    CommandInfo ci = ParseMessage(message);

                    string response = "";
                    if (ci.command == "memory")
                    {
                        //get memory amount
                        response = MakeResponse(ci.command, Getmemory(ci));
                    }
                    else if(ci.command == "settarget")
                    {
                        //set processname of browser( chrome, firefox, edge, waterfox, palemoon... )
                        targetprocess = ci.commandargs[0];
                        response = MakeResponse(ci.command, ResultOk);
                    }

                    //Return command response to browser
                    if( debugmode)
                    {
                        WriteDebug(response);
                    }
                    else
                    {
                        Write(response);
                    }
                }
            }
        }

        public static string MakeResponse(string command, string response)
        {
            //JSON format
            return "{ \"command\": " + "\""+ command + "\"" + ", \"result\": " + "\"" + response + "\"" + " }";
        }

        public static CommandInfo ParseMessage(string message)
        {
            //Parsing JSON is quite bother. So use more simple format...
            //"command[,arg1][,arg2],..."  ( this is also formal format as JSON )
            char[] delimiters = { ',' };
            message = message.Trim('"');
            string[] tokens = message.Split(delimiters);
            List<string> commandargs = new List<string>();
            for (int i = 1; i < tokens.Length; i++)
            {
                commandargs.Add(tokens[i]);
            }
            string command = tokens[0];
            return new CommandInfo(command, commandargs);
        }

        //reference:
        //https://stackoverflow.com/questions/30880709/c-sharp-native-host-with-chrome-native-messaging
        public static string Read()
        {
            const int maxlength = 100;
            var stdin = Console.OpenStandardInput();
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);
            if (length > maxlength)
            {
                throw new ApplicationException("invalid message length");
            }

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }

            string command = new string(buffer);
            return command;
        }

        public static string ReadDebug()
        {
            return Console.ReadLine();
        }

        public static void Write(string command)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(command);
            var stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }

        public static void WriteDebug(string command)
        {
            Console.WriteLine(command);
        }

        public static string Getmemory(CommandInfo commandinfo)
        {
            List<string> args = commandinfo.commandargs;
            string type = (args.Count != 0) ? args[0] : "";

            Process[] ps = Process.GetProcesses();
            Int64 totalmemory = 0;

            foreach (Process p in ps)
            {
                try
                {
                    if (p.ProcessName == targetprocess)
                    {
                        if (type == "privateworkingset")
                        {
                            totalmemory += p.PrivateMemorySize64;
                        }
                        else
                        {
                            totalmemory += p.WorkingSet64;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return totalmemory.ToString();
        }
    }
}
