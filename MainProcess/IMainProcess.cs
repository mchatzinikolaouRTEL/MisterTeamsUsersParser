using RtelLibrary.Enums;
using System;
using System.Threading.Tasks;

namespace MisterTeamsUsersParserParser.MainProcess
{
    public interface IMainProcess
    {        
        public DateTime LastTimeExecuted { get; }
        public ProcessStatus Status { get; }
        public Task Start();
        public void Stop();
        public void MainProcess();

    }
}
