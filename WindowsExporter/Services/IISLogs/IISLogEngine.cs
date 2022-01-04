using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using WindowsExporter.Core.Helper;
using WindowsExporter.Models.Internal;

namespace WindowsExporter.Services.IISLogs
{
    internal class IISLogEngine : IDisposable
    {
        public string FilePath { get; set; }
        public int CurrentFileRecord { get; private set; }
        //private readonly StreamReader _logfile;
        private string[] _headerFields;
        Hashtable dataStruct = new Hashtable();
        private readonly int _mbSize;

        public IISLogEngine(string filePath)
        {
            if (File.Exists(filePath))
            {
                FilePath = filePath;
                _mbSize = (int)new FileInfo(filePath).Length / 1024 / 1024;
            }
            else
            {
                throw new Exception($"Could not find File {filePath}");
            }
        }

        public IEnumerable<IISLogEvent> ParseLog()
        {
            if (_mbSize < 50)
            {
                return QuickProcess();
            }
            else
            {
                return LongProcess();
            }
        }

        private IEnumerable<IISLogEvent> QuickProcess()
        {
            List<IISLogEvent> events = new List<IISLogEvent>();
            var lines = FileHelper.ReadAllLines(FilePath);
            foreach (string line in lines)
            {
                ProcessLine(line, events);
            }
            return events;
        }

        private IEnumerable<IISLogEvent> LongProcess()
        {
            List<IISLogEvent> events = new List<IISLogEvent>();
            using (FileStream fileStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (streamReader.Peek() > -1)
                    {
                        ProcessLine(streamReader.ReadLine(), events);
                    }
                }
            }
            return events;
        }

        private void ProcessLine(string line, List<IISLogEvent> events)
        {
            if (line.StartsWith("#Fields:"))
            {
                _headerFields = line.Replace("#Fields: ", string.Empty).Split(' ');
            }
            if (!line.StartsWith("#") && _headerFields != null)
            {
                string[] fieldsData = line.Split(' ');
                FillDataStruct(fieldsData, _headerFields);
                events?.Add(NewEventObj());
                CurrentFileRecord++;
            }
        }

        private IISLogEvent NewEventObj()
        {
            return new IISLogEvent
            {
                DateTimeEvent = GetEventDateTime(),
                sSitename = dataStruct["s-sitename"]?.ToString(),
                sComputername = dataStruct["s-computername"]?.ToString(),
                sIp = dataStruct["s-ip"]?.ToString(),
                csMethod = dataStruct["cs-method"]?.ToString(),
                csUriStem = dataStruct["cs-uri-stem"]?.ToString(),
                csUriQuery = dataStruct["cs-uri-query"]?.ToString(),
                sPort = dataStruct["s-port"] != null ? int.Parse(dataStruct["s-port"]?.ToString()) : null,
                csUsername = dataStruct["cs-username"]?.ToString(),
                cIp = dataStruct["c-ip"]?.ToString(),
                csVersion = dataStruct["cs-version"]?.ToString(),
                csUserAgent = dataStruct["cs(User-Agent)"]?.ToString(),
                csCookie = dataStruct["cs(Cookie)"]?.ToString(),
                csReferer = dataStruct["cs(Referer)"]?.ToString(),
                csHost = dataStruct["cs-host"]?.ToString(),
                scStatus = dataStruct["sc-status"] != null ? int.Parse(dataStruct["sc-status"]?.ToString()) : null,
                scSubstatus = dataStruct["sc-substatus"] != null ? int.Parse(dataStruct["sc-substatus"]?.ToString()) : null,
                scWin32Status = dataStruct["sc-win32-status"] != null ? long.Parse(dataStruct["sc-win32-status"]?.ToString()) : null,
                scBytes = dataStruct["sc-bytes"] != null ? int.Parse(dataStruct["sc-bytes"]?.ToString()) : null,
                csBytes = dataStruct["cs-bytes"] != null ? int.Parse(dataStruct["cs-bytes"]?.ToString()) : null,
                timeTaken = dataStruct["time-taken"] != null ? int.Parse(dataStruct["time-taken"]?.ToString()) : null
            };
        }

        private DateTime GetEventDateTime()
        {
            DateTime finalDate = DateTime.Parse($"{dataStruct["date"]} {dataStruct["time"]}");
            return finalDate;
        }
        private void FillDataStruct(string[] fieldsData, string[] header)
        {
            dataStruct.Clear();
            for (int i = 0; i < header.Length; i++)
            {
                dataStruct.Add(header[i], fieldsData[i] == "-" ? null : fieldsData[i]);
            }
        }
        public void Dispose()
        { }
    }
}
