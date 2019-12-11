using System;
using System.Diagnostics;
using System.Text;

namespace DjuQBox.MPC
{
    public class MpcWrapper
    {

        Process process;
        ProcessStartInfo processStartInfo = null;
        String MPC_PATH = @"C:\DjQbox\mpd\mpc-0.22";
        string arguments = "";
        MpcCommand mpcCommand;

        public MpcWrapper(string aMpcPath)
        {
            MPC_PATH = aMpcPath;
        }

        private void Do()
        {
            arguments = mpcCommand.ToString() + " " + arguments;
            processStartInfo = new ProcessStartInfo
            {
                FileName = MPC_PATH + "mpc",
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                // UseShellExecute = false
            };


            process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
            process.Exited += (sender, args) => KillStandardEventHandlers();
            //process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);


            //ThreadPool.QueueUserWorkItem(ydl.StandardOutput, ydl.stdOutputTokenSource.Token);
            //ThreadPool.QueueUserWorkItem(ydl.StandardError, ydl.stdErrorTokenSource.Token);

            //if (ydl.Info != null)
            //{
            //    ydl.StandardOutputEvent += (sender, output) => ydl.Info.ParseOutput(sender, output.Trim());
            //    ydl.StandardErrorEvent += (sender, output) => ydl.Info.ParseError(sender, output.Trim());
            //}

            process.Start();
            int ProcessID = process.Id;
            //string s = this.process.StandardOutput.ReadLine();
            process.WaitForExit();

            arguments = ""; // gia na trejei swsta h epomenh entolh
        }

        private void KillStandardEventHandlers()
        {
            //throw new NotImplementedException();
        }

        public void Play(int aPosition = 0)
        {
            mpcCommand = MpcCommand.play;// "play";
            arguments = aPosition.ToString();
            Do();
        }

        public void Update(bool aWaitToFinish = false, String pathToUpdate = "")
        {
            mpcCommand = MpcCommand.update;
            arguments = aWaitToFinish ? " --wait " : "";
            arguments = pathToUpdate;
            Do();

        }

        public void PlaylistClear()
        {
            mpcCommand = MpcCommand.clear;            
            Do();
        }

        public void PlaylistAddSong(String aSongLibraryPath)
        {
            mpcCommand = MpcCommand.add;
            arguments = aSongLibraryPath; //8elei katallhlo escaping
            Do();
        }

        public void PlaylistSave(string title)
        {
            mpcCommand = MpcCommand.save;

            arguments = " \"" + title.Normalize() + "\"";
            //Encoding.Convert()
            // arguments = @" """ + Encoding.UTF8.GetString(Encoding.GetEncoding("windows-1253").GetBytes(title)) + "" ; //8elei katallhlo escaping

            Do();
        }
    }

    internal enum MpcCommand
    {
        add,
        clearerror,
        current,
        find,
        insert,
        ls,
        move,
        play,
        repeat,
        search,
        sendmessage,
        status,
        toggle,
        volume,
        cdprev,
        consume,
        del,
        findadd,
        list,
        lsplaylists,
        next,
        playlist,
        replaygain,
        searchadd,
        shuffle,
        sticker,
        toggleoutput,
        waitmessage,
        channels,
        crop,
        disable,
        idle,
        listall,
        mixrampdb,
        outputs,
        prev,
        rm,
        searchplay,
        single,
        stop,
        update,
        clear,
        crossfade,
        enable,
        idleloop,
        load,
        mixrampdelay,
        pause,
        random,
        save,
        seek,
        stats,
        subscribe,
        version,
    }

    //internal   enum MpcOptions :
    //{
    //    format = "-f"
    //}
}
